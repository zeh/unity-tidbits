using System;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode()]
#endif

public class TextMeshSharpener:MonoBehaviour {

	/*
	Makes TextMesh look sharp regardless of camera size/resolution
	Do NOT change character size or font size; use scale only
	*/

	// Properties
	private float lastPixelHeight = -1;
	private TextMesh textMesh;


	// ================================================================================================================
	// MAIN EVENT INTERFACE -------------------------------------------------------------------------------------------

	void Awake() {
	}

	void Start() {
		textMesh = GetComponent<TextMesh>();
		resize();
	}

	void Update() {
		if (Camera.main.pixelHeight != lastPixelHeight || (Application.isEditor && !Application.isPlaying)) resize();
	}

	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

	private void resize() {
		float ph = Camera.main.pixelHeight;
		float ch = Camera.main.orthographicSize;

		/*
		//Constant size:
		float pixelRatio = (ch * 2.0f) / ph;
		textMesh.characterSize = 1;
		transform.localScale = new Vector3(pixelRatio * 10.0f, pixelRatio * 10.0f, pixelRatio * 0.1f);
		*/

		float pixelRatio = (ch * 2.0f) / ph;
		float targetRes = 128f;

		textMesh.characterSize = pixelRatio * Camera.main.orthographicSize / Math.Max(transform.localScale.x, transform.localScale.y);
		textMesh.fontSize = (int)Math.Round(targetRes / textMesh.characterSize);

		lastPixelHeight = ph;
	}


}