using System;
using UnityEngine;

class GameTimer {

	// Properties
	private bool isRunning;
	private bool useRealTime;
	private float timeElapsed;
	private float currentTime;
	private float lastTime;
	private float timeScale;


	// ================================================================================================================
	// EXTENDED INTERFACE ---------------------------------------------------------------------------------------------

	void Start() {
		isRunning = false;
		useRealTime = false;
		timeElapsed = 0;
		currentTime = 0;
		lastTime = 0;
		timeScale = 1;
	}

	void Update() {

	}


	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	public void startTimer() {
		if (!isRunning) {
			isRunning = true;
			lastTime = getSystemTime();
		}
	}

	public void stopTimer() {
		if (!isRunning) {
			isRunning = false;
		}
	}

	public void resetTimer() {
		timeElapsed = 0;
		currentTime = 0;
		lastTime = getSystemTime();
	}

	public float getTime() {
		// Return the time, in seconds
		updateTimer();
		return currentTime;
	}


	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

	private float getSystemTime() {
		// Get the system time, in seconds
		return useRealTime ? Time.realtimeSinceStartup : Time.time;
	}

	private void updateTimer() {
		// Time since last Update()
		float currTime = getSystemTime();

		if (currTime > lastTime) {
			timeElapsed = currTime - lastTime;

			// If running, add the time elapsed to advance the timer
			if (isRunning) currentTime += timeElapsed * timeScale;

			// Store time so we can use it on the next update
			lastTime = currTime;
		}
	}

}
