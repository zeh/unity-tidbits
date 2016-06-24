using UnityEngine;
using System.Collections;

class GameManager:MonoBehaviour {

	// Properties
	private GameLooper _looper;
	private SceneManager _sceneManager;

	private static GameManager instance;


	// ================================================================================================================
	// EXTENDED INTERFACE ---------------------------------------------------------------------------------------------

	void Awake() {
		DontDestroyOnLoad(gameObject);

		instance = this;
		_looper = new GameLooper();
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

	public GameLooper looper {
		get {
			return _looper;
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
