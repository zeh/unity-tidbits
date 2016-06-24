using UnityEngine;

public class FPSDisplay:MonoBehaviour {

	private int frames = 0;
	private float updateInterval = 0.5f; // Time to wait, in seconds
	private float lastUpdate = 0;
	private TextMesh textMesh;

	void Start() {
		textMesh = GetComponent<TextMesh>();
	}

	void Update() {
		frames++;
		if (Time.time - lastUpdate > updateInterval) {
			updateValues();
		}
	}

	void updateValues() {
		float timePassed = Time.time - lastUpdate;
		float msec = timePassed / (float)frames * 1000f;
		float fps = frames / timePassed;

		lastUpdate += timePassed;
		frames = 0;

		textMesh.text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
	}
}