using UnityEngine;
using System.Collections;

public class SceneManager {

	// Properties
	public string[] levelNames;
	private int _currentLevel;
	

	// ================================================================================================================
	// EXTENDED INTERFACE ---------------------------------------------------------------------------------------------

	public void Start() {
		// keep this object alive
		_currentLevel = -1;
	}


	// ================================================================================================================
	// PRIVATE INTERFACE ----------------------------------------------------------------------------------------------

	private void loadLevel(string __sceneName) {
		Application.LoadLevel(__sceneName);
	}
		
	private void loadLevel(int __index) {
		loadLevel(levelNames[__index]);
	}


	// ================================================================================================================
	// ACCESSOR INTERFACE -----------------------------------------------------------------------------------------------

	public int currentLevel {
		get {
			return _currentLevel;
		}
	}


	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	public void loadNextLevel() {
		int nextLevel = (_currentLevel + 1) % levelNames.Length;
		loadLevel(nextLevel);
		_currentLevel = nextLevel;
	}

}
