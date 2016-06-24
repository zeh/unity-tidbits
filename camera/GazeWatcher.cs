using System;
using System.Collections.Generic;
using UnityEngine;

public class GazeWatcher {

	/**
	 * Given a camera, tracks its position and which object is in its gaze
	 *
	 * Usage:
	 *
	 * var watcher = new GazeWatcher(Camera.main);
	 * watcher.position // Vector3 position of the camera
	 * watcher.direction // Vector3 direction normal of where the camera is looking
	 */

	// Handlers
	public delegate void GazeWatcherHandler(GazeWatcher gazeWatcher);

	// Properties
	private GameLooper _looper;
	private RaycastHit _lastHit;
	private Camera _camera;

	private Vector3 _position;
	private Vector3 _direction;
	private bool _isHitting;
	private Vector3 _hitPosition;
	private Vector3 _hitDirection;
	private GameObject _hitObject;

	// Public
	public event GazeWatcherHandler onObjectHitStart;
	public event GazeWatcherHandler onObjectHitMove;
	public event GazeWatcherHandler onObjectHitEnd;


	// ================================================================================================================
	// CONSTRUCTOR ----------------------------------------------------------------------------------------------------

	public GazeWatcher(Camera camera) {
		_camera = camera;
		_looper = new GameLooper();

		_looper.onUpdate += onUpdate;
	}


	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	public Vector3 position {
		get {
			return _position;
		}
	}

	public Vector3 direction {
		get {
			return _direction;
		}
	}

	public bool isHitting {
		get {
			return _isHitting;
		}
	}

	public Vector3 hitPosition {
		get {
			return _hitPosition;
		}
	}

	public Vector3 hitDirection {
		get {
			return _hitDirection;
		}
	}

	public GameObject hitObject {
		get {
			return _hitObject;
		}
	}


	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

	private void onUpdate(GameLooper gameLooper) {
		// Do a raycast into the world based on camera position and orientation
		if (_camera == null) return;

		_position = _camera.transform.position;
		_direction = _camera.transform.forward;

		bool didHit = Physics.Raycast(_position, _direction, out _lastHit);

		if ((!didHit || _lastHit.transform.gameObject != _hitObject) && _isHitting) {
			// Ended a hit
			if (onObjectHitEnd != null) onObjectHitEnd(this);

			_isHitting = false;
			_hitPosition = Vector3.zero;
			_hitDirection = Vector3.zero;
			_hitObject = null;
		}

		if (didHit) {
			// It is hitting
			_hitPosition = _lastHit.point;
			_hitDirection = _lastHit.normal;

			if (!_isHitting) {
				// Started a new hit
				_isHitting = true;
				_hitObject = _lastHit.transform.gameObject;
				if (onObjectHitStart != null) onObjectHitStart(this);
			}

			if (onObjectHitMove != null) onObjectHitMove(this);
		}
	}
}