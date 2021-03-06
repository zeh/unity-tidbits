﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NavigatorManager:MonoBehaviour {

	// Parameters
	public NavigatorScene startingScene;

	// Properties
	private bool isNavigating;
	private NavigatorScene currentScene;


	// ================================================================================================================
	// MAIN EVENT INTERFACE -------------------------------------------------------------------------------------------

	void Start() {
		isNavigating = false;
		if (startingScene != null) navigateTo(startingScene, null, true);
	}

	void Update() {

	}


	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	public void navigateTo(string gameObjectName) {
		GameObject gameObject = GameObject.Find(gameObjectName);
		if (gameObject != null && gameObject.GetComponent<NavigatorScene>() != null) {
			navigateTo(gameObject.GetComponent<NavigatorScene>());
		} else {
			Debug.LogError("Game object with name [" + gameObjectName + "] and a NavigatorScene component not found!");
		}
	}

	public void navigateTo(NavigatorScene newScene, Dictionary<string, object> newBundle = null, bool immediate = false) {
		// Navigates to a gameobject with the camera
		//Debug.Log("navigating to " + gameObject + " @ " + gameObject.transform.position);

		if (!isNavigating && newScene != null && newScene.cameraTarget != null && newScene != currentScene) {
			//* .call(() => logDone("over"))

			// Initializations
			newScene.initialize(newBundle);

			if (immediate) {
				// Immediately show it
				if (currentScene != null) currentScene.onStartedHiding();
				newScene.onStartedShowing();
				Camera.main.gameObject.transform.position = newScene.cameraTarget.transform.position;
				if (currentScene != null) currentScene.onFinishedHiding();
				newScene.onFinishedShowing();
			} else {
				// Animate to it
				// Create tween
				var tween = ZTween.use(Camera.main.gameObject);

				// Call start functions
				if (currentScene != null) tween.call(currentScene.onStartedHiding);
				tween.call(newScene.onStartedShowing);

				// Animate
				tween.moveTo(newScene.cameraTarget.transform.position, 0.4f, Easing.backInOut);

				// Call ending functions
				if (currentScene != null) tween.call(currentScene.onFinishedHiding);
				tween.call(newScene.onFinishedShowing);

				// Play the tween
				tween.play();//.wait(1).call(Func).set("visible", false).play();
			}

			currentScene = newScene;
		}
	}
}
