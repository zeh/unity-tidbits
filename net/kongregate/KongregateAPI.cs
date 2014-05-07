using System;
using UnityEngine;

public class KongregateAPI:MonoBehaviour {

	// Properties
	private bool _isConnected;
	private int _userId;
	private string _userName;
	private string _gameAuthToken;


	// ================================================================================================================
	// MAIN EVENT INTERFACE -------------------------------------------------------------------------------------------

	void Start() {
		_isConnected = false;
		_userId = 0;
		_userName = "Guest";
		_gameAuthToken = "";
	}

	void Awake() {
		// Instructs the game object to survive level changes
		DontDestroyOnLoad(this);

		// Begin the API loading process if available
		Application.ExternalEval(
			"if (typeof(kongregateUnitySupport) != 'undefined') {" +
			"    kongregateUnitySupport.initAPI('" + gameObject.name + "', 'OnKongregateAPILoaded');" +
			"}"
		);
	}


	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	public static KongregateAPI Create() {
		// Create a game object with a reference to the API
		GameObject newGameObject = new GameObject("KongregateAPIObject-" + (Time.realtimeSinceStartup));
		KongregateAPI instance = newGameObject.AddComponent<KongregateAPI>();
		return instance;
	}

	public void OnKongregateAPILoaded(string __userInfoString) {
		// Is connected
		_isConnected = true;
 
		// Splits the user info parameter
		string[] userParams = __userInfoString.Split('|');
		_userId = int.Parse(userParams[0]);
		_userName = userParams[1];
		_gameAuthToken = userParams[2];
	}

	public void SubmitStats(string __name, int __value) {
		Application.ExternalCall("kongregate.stats.submit", __name, __value);
	}

	public bool isConnected {
		get { return _isConnected; }
	}

	public int userId {
		get { return _userId; }
	}

	public string userName {
		get { return _userName; }
	}

	public string gameAuthToken {
		get { return _gameAuthToken; }
	}
}

