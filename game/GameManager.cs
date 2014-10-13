using UnityEngine;
using System.Collections;

class GameManager:MonoBehaviour {
			
	// Properties
	private GameTimer _timer;
	private SceneManager _sceneManager;

	private static GameManager instance;


	// ================================================================================================================
	// EXTENDED INTERFACE ---------------------------------------------------------------------------------------------

	void Awake() {
		DontDestroyOnLoad(gameObject);

		instance = this;
		_timer = new GameTimer();
		_sceneManager = new SceneManager();
	}

	void Start() {
	}

	void Update() {
	}


	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	public GameManager getInstance() {
		return instance;
	}


	// ================================================================================================================
	// ACCESSOR INTERFACE ---------------------------------------------------------------------------------------------

	public GameTimer timer {
		get {
			return _timer;
		}
	}

	public SceneManager sceneManager {
		get {
			return _sceneManager;
		}
	}


	// ================================================================================================================
	// EXTENDABLE INTERFACE -------------------------------------------------------------------------------------------

	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------


}
