using System;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode()]
#endif

public class GameObjectAligner:MonoBehaviour {

	// Enums
	public enum AlignType {
		TOP_LEFT, TOP_CENTER, TOP_RIGHT,
		MIDDLE_LEFT, MIDDLE_CENTER, MIDDLE_RIGHT,
		BOTTOM_LEFT, BOTTOM_CENTER, BOTTOM_RIGHT
	}

	public AlignType alignType;
	public float margin;

	// Properties
	private float lastPixelHeight = -1;
	private float lastPixelWidth = -1;


	// ================================================================================================================
	// MAIN EVENT INTERFACE -------------------------------------------------------------------------------------------

	void Awake() {
	}

	void Start() {
		resize();
	}

	void Update() {
		if (Camera.main.pixelHeight != lastPixelHeight || Camera.main.pixelWidth != lastPixelWidth || (Application.isEditor && !Application.isPlaying)) resize();
	}

	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

	private void resize() {
		float ph = Camera.main.pixelHeight;
		float pw = Camera.main.pixelWidth;
		//float ch = Camera.main.orthographicSize;

		float w = 0;
		float h = 0;
		float cx = 0;
		float cy = 0;

		// Use collider w/h if found
		var colliders = GetComponentsInChildren<Collider>();
		if (colliders.Length > 0) {
			w = ((BoxCollider)colliders[0]).size.x;
			h = ((BoxCollider)colliders[0]).size.y;
			cx = ((BoxCollider)colliders[0]).center.x;
			cy = ((BoxCollider)colliders[0]).center.y;
		}

		lastPixelWidth = pw;
		lastPixelHeight = ph;

		// Find corner positions
		var tl = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight, 1));
		var br = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, 0, 1));

		var t = tl.y;
		var b = br.y;
		var l = tl.x;
		var r = br.x;

		float newX = 0, newY = 0;

		// Align X
		switch (alignType) {
			case AlignType.TOP_LEFT:
			case AlignType.MIDDLE_LEFT:
			case AlignType.BOTTOM_LEFT:
				newX = l + margin + w * 0.5f - cx;
				break;
			case AlignType.TOP_CENTER:
			case AlignType.MIDDLE_CENTER:
			case AlignType.BOTTOM_CENTER:
				newX = (l + r) * 0.5f;
				break;
			case AlignType.TOP_RIGHT:
			case AlignType.MIDDLE_RIGHT:
			case AlignType.BOTTOM_RIGHT:
				newX = r - margin - w * 0.5f - cx;
				break;
		}

		// Align Y
		switch (alignType) {
			case AlignType.TOP_LEFT:
			case AlignType.TOP_CENTER:
			case AlignType.TOP_RIGHT:
				newY = t - margin - h * 0.5f - cy;
				break;
			case AlignType.MIDDLE_LEFT:
			case AlignType.MIDDLE_CENTER:
			case AlignType.MIDDLE_RIGHT:
				newY = (b + t) * 0.5f;
				break;
			case AlignType.BOTTOM_LEFT:
			case AlignType.BOTTOM_CENTER:
			case AlignType.BOTTOM_RIGHT:
				newY = b + margin + h * 0.5f - cy;
				break;
		}

		// Set positions
		gameObject.transform.localPosition = new Vector3(newX, newY, gameObject.transform.position.z);

	}


}