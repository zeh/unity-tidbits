using UnityEngine;

public class HoloLensStatusDisplay:MonoBehaviour {

	private int _frames = 0;
	private float _updateInterval = 0.10f; // Time to wait, in seconds
	private float _lastUpdate = 0;
	private TextMesh _textMesh;

	private GazeWatcher _gazeWatcher;

	void Start() {
		_textMesh = GetComponent<TextMesh>();
		_gazeWatcher = new GazeWatcher(Camera.main);
	}

	void Update() {
		_frames++;
		if (Time.time - _lastUpdate > _updateInterval) {
			updateValues();
		}
	}

	void updateValues() {
		float timePassed = Time.time - _lastUpdate;
		float msec = timePassed / (float)_frames * 1000f;
		float fps = _frames / timePassed;

		_lastUpdate += timePassed;
		_frames = 0;

		var caption = "";
		caption += string.Format("Time: {0:0.0}ms", msec);
		caption += "\n";
		caption += string.Format("FPS: {0:0.} ", fps);
		caption += "\n";
		caption += "Camera rot: " + _gazeWatcher.direction;
		caption += "\n";
		caption += "Camera pos: " + _gazeWatcher.position;
		_textMesh.text = caption;
	}
}