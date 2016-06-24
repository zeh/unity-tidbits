using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
[ExecuteInEditMode()]
#endif

public class ModelResizer:MonoBehaviour {

	// Resizes a model to a desired maximum size
	// TODO: Actually get rid of this file.

	// Properties
	public float desiredHeight;
	public float desiredWidth;
	public float desiredDepth;

	// ================================================================================================================
	// MAIN EVENT INTERFACE -------------------------------------------------------------------------------------------

	#if UNITY_EDITOR
	void Start () {
		resize();
	}

	void Update() {
		if (!Application.isPlaying) resize();
	}
	#endif

	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

	private void resize() {
		foreach (Transform child in transform) {
			// Resize
			GameObjectUtils.resizeToFit(child.gameObject, desiredWidth, desiredHeight, desiredDepth);
			// Center in the bottom
			GameObjectUtils.alignToVector(child.gameObject, transform.position, 0.5f, 0, 0.5f);
		}
	}
}
