using UnityEngine;

/**
 * An object that continuously loops ("ticks") on every rendering frame, dispatching event calls every time
 * it does so.
 *
 * More information: http://zehfernando.com/2013/a-gamelooper-class-for-actionscript-3-projects/
 * Similar to https://github.com/zeh/as3/blob/master/com/zehfernando/models/GameLooper.as
 *
 * TODO:
 * * allow fixedUpdate, lateUpdate
 * * minFps, maxFps, minInterval, maxInterval? Might work with fixed update already
 */

/**
 * From GameLooper.as:
 * Using GameLooper is similar to creating an ENTER_FRAME event and watching for it, but with these differences:
 *  . Actual "tick" call rate is flexible: it can execute more than one call per frame, or skip frames as needed
 *  . It keeps track of relative time, so it gets passed time and frame data (for correct position calculation)
 *  . Time is flexible, so it can be multiplied/scaled, paused, and resumed
 *
 *
 * How to use:
 *
 * 1. Create a new instance of GameLooper. This will make the looper's onTicked() signal be fired once per frame
 * (same as ENTER_FRAME):
 *
 *     var looper:GameLooper = new GameLooper(); // Create and start
 *
 *     var looper:GameLooper = new GameLooper(true); // Creates and pauses
 *
 * 2. Create function callbacks to receive the signal (signals are like events, but simpler):
 *
 *     looper.onTicked.add(onTick);
 *
 *     private function onTick(currentTimeSeconds:Number, tickDeltaTimeSeconds:Number, currentTick:int):void {
 *         var speed:Number = 10; // Speed of the box, in pixels per seconds
 *         box.x += speed * tickDeltaTimeSeconds;
 *     }
 *
 *
 * You can also:
 *
 * 1. Pause/resume the looper to pause/resume the game loop:
 *
 *     looper.pause();
 *     looper.resume();
 *
 *
 * 2. Change the time scale to make time go "faster" (currentTimeSeconds, and tickDeltaTimeSeconds):
 *
 *     looper.timeScale = 2; // 2x original speed (faster motion)
 *     looper.timeScale = 0.5; // 0.5x original speed (slower motion)
 *
 * 3. Specify a minimum FPS as a parameter. When the minFPS parameter is used, the looper is always dispatched at
 * least that amount of times per second, regardless of the number of frames:
 *
 *     var looper:GameLooper = new GameLooper(false, 8);
 *
 * In the above example, on a SWF with 4 frames per second, onTicked would be fired twice per frame. On a SWF with
 * 6 frames per second, it would be fired once, and then twice every other frame.
 *
 * 4. Specify a maximum FPS as a parameter. When the maxFPS parameter is used, the looper is not dispatched more
 * that number of times per second:
 *
 *     var looper:GameLooper = new GameLooper(false, NaN, 10);
 *
 * In the above example, on a SWF with 30 frames per second, onTicked would be fired once every 3 frames.
 *
 */

public class GameLooper {

	/**
	 * Given a camera, tracks its position and which object is in its gaze
	 */

	// Handlers
	public delegate void GameLooperHandler(GameLooper looper);

	// Properties
	private bool _isRunning;
	private float _timeScale;
	private int _currentTick;				// Current absolute frame
	private float _currentTime;				// Current absolute time, in s
	private float _tickDeltaTime;			// Time since last tick, in s
	private float _lastTimeUpdated;
	private bool _useRealTime;				// If true, uses real (unscaled) system time first

	private GameObject _surrogateGameObject;
	private GameLooperSurrogate _surrogate;

	// Temp stuff to reduce garbage collection
	private float _now;
	private float _frameDeltaTime;
	private float _interval;

	// Public
	public event GameLooperHandler onResume;
	public event GameLooperHandler onPause;
	public event GameLooperHandler onUpdate;
	//public event GameLooperHandler FixedUpdate;
	//public event GameLooperHandler LateUpdate;


	// ================================================================================================================
	// CONSTRUCTOR ----------------------------------------------------------------------------------------------------

	public GameLooper() {
		_surrogateGameObject = new GameObject("GameLooper-" + (Time.realtimeSinceStartup));
		_surrogate = _surrogateGameObject.AddComponent<GameLooperSurrogate>();
		_surrogate.setGameLooper(this);
		_surrogate.enabled = false;

		_timeScale = 1;
		_currentTick = 0;
		_currentTime = 0;
		_tickDeltaTime = 0;
		_useRealTime = false;
		_isRunning = false;

		resume();
	}

	~GameLooper() {
		//GameObject.Destroy(_surrogateGameObject);
		_surrogateGameObject = null;
		_surrogate = null;
		onResume = null;
		onPause = null;
		onUpdate = null;
	}


	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	public void resume() {
		if (!_isRunning) {
			_isRunning = true;
			_lastTimeUpdated = getTime();
			_frameDeltaTime = 0;
			_tickDeltaTime = 0;
			_surrogate.enabled = true;

			if (onResume != null) onResume(this);
		}
	}

	public void pause() {
		if (_isRunning) {
			_isRunning = false;
			_surrogate.enabled = false;

			if (onPause != null) onPause(this);
		}
	}

	public float deltaTime {
		get {
			return _frameDeltaTime;
		}
	}

	public float time {
		get {
			return _currentTime;
		}
	}

	public int frameCount {
		get {
			return _currentTick;
		}
	}

	public float timeScale {
		get {
			return _timeScale;
		}
		set {
			_timeScale = value;
		}
	}


	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

	private float getTime() {
		return _useRealTime ? Time.realtimeSinceStartup : Time.time;
	}

	private void updateTime() {
		_now = getTime();
		_frameDeltaTime = _now - _lastTimeUpdated;
		_tickDeltaTime = _frameDeltaTime * _timeScale;

		_currentTick++;
		_currentTime += _tickDeltaTime;

		_lastTimeUpdated = _now;
	}

	private void dispatchUpdate() {
		updateTime();

		if (onUpdate != null) onUpdate(this);
	}

	/*
	// TODO: reactivate this later, once the time relationship is figured
	private void dispatchFixedUpdate() {
		updateTime();

		if (FixedUpdate != null) FixedUpdate(this);
	}

	private void dispatchLateUpdate() {
		updateTime();

		if (LateUpdate != null) LateUpdate(this);
	}
	*/


	// ================================================================================================================
	// AUX CLASSES ----------------------------------------------------------------------------------------------------

	class GameLooperSurrogate:MonoBehaviour {

		// Properies
		private GameLooper _gameLooper;


		// ================================================================================================================
		// MAIN EVENT INTERFACE -------------------------------------------------------------------------------------------

		void Update() {
			if (_gameLooper != null) _gameLooper.dispatchUpdate();
		}

		/*
		void FixedUpdate() {
			if (_gameLooper != null) _gameLooper.dispatchFixedUpdate();
		}

		void LateUpdate() {
			if (_gameLooper != null) _gameLooper.dispatchLateUpdate();
		}
		*/


		// ================================================================================================================
		// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

		public void setGameLooper(GameLooper gameLooper) {
			_gameLooper = gameLooper;
		}
	}
}