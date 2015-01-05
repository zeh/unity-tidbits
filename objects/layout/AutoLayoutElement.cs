using System;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
[ExecuteInEditMode()]
#endif

public class AutoLayoutElement:MonoBehaviour {

	// Delegates
	protected delegate void SimpleAction();

	// Events
	protected event SimpleAction OnShouldApplyLayout;

	// Properties
	private List<GameObject> dependents = new List<GameObject>();
	private List<GameObject> parents = new List<GameObject>();

	private float lastCameraHeight;
	private float lastCameraWidth;

	protected bool shouldAutoUpdate;													// If true, this instance should self-update when a change was detected (otherwise, it only updates when told so by parents)

	private bool needsAdditionalUpdate;


	// ================================================================================================================
	// MAIN EVENT INTERFACE -------------------------------------------------------------------------------------------

	void Awake() {
	}

	void Start() {
		applyLayoutRules();
	}

	void Update() {
		if (needsAdditionalUpdate) {
			// This should not be needed, but it's always missing 1 frame at the end
			applyLayoutRules();
			needsAdditionalUpdate = false;
		}
	}

	void LateUpdate() {
		if (shouldAutoUpdate) {
			// Should auto update: checks if necessary
			if (Camera.main.pixelHeight != lastCameraHeight || Camera.main.pixelWidth != lastCameraWidth) {
				// Needs to update
				//Debug.Log("=============> Updating starting @ " + transform.parent.gameObject.name + "." + gameObject.name);
				lastCameraHeight = Camera.main.pixelHeight;
				lastCameraWidth = Camera.main.pixelWidth;
				applyLayoutRules();
				needsAdditionalUpdate = true;
			}
			applyLayoutRules();
		}
	}

	/*
	void OnEnable() {
		if (shouldAutoUpdate) needsUpdate = true;
	}
	*/


	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	public void requestUpdate() {
		applyLayoutRules();
	}

	public void addDependent(GameObject dependent) {
		if (!dependents.Contains(dependent)) dependents.Add(dependent);
	}

	public void removeDependent(GameObject dependent) {
		if (dependents.Contains(dependent)) dependents.Remove(dependent);
	}


	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

	protected void checkParents(params GameObject[] parentsToCheck) {
		// Checks if a parent has changed, adding or removing itself from the parent as a dependent

		var addedParents = new List<GameObject>();
		var removedParents = new List<GameObject>();

		// Checks differences

		// Checks for new parents
		foreach (var parentToCheck in parentsToCheck) {
			if (parentToCheck == gameObject) {
				Debug.LogWarning("Trying to add itself as a parent!");
				continue;
			}

			if (parentToCheck == null) continue;

			if (!parents.Contains(parentToCheck) && !addedParents.Contains(parentToCheck)) {
				addedParents.Add(parentToCheck);
			}
		}

		// Checks for removed parents
		foreach (var parent in parents) {
			if (Array.IndexOf(parentsToCheck, parent) < 0 && !removedParents.Contains(parent)) {
				removedParents.Add(parent);
			}
		}

		// Finally, adds and removes all parent
		foreach (var parent in addedParents) {
			addToParent(parent);
			parents.Add(parent);
		}

		foreach (var parent in removedParents) {
			removeFromParent(parent);
			parents.Remove(parent);
		}
	}

	protected Bounds getBoundsOrPoint(GameObject gameObject) {
		// Gets the best bounding box from a game object, using a placeholder box if none can be found
		var bounds = getBounds(gameObject);
		return bounds == null ? new Bounds(gameObject.transform.position, new Vector3(0.0001f, 0.0001f, 0.0001f)) : (Bounds)bounds;
	}

	protected Bounds? getBounds(GameObject gameObject) {
		// Gets the best bounding box from a game object
		if (gameObject == null) {
			// No object, use the screen size instead
			var tl = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight, 0.1f));
			var br = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, 0, 0.1f));
			return new Bounds((tl+br)/2, br-tl);
		} else if (gameObject.collider != null) {
			// Use collider
			return gameObject.collider.bounds;
		} else if (gameObject.renderer != null) {
			// Use renderer
			return gameObject.renderer.bounds;
		}

		// None found!
		return null;
	}


	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

	private void removeFromParent(GameObject parent) {
		if (parent != null && parent.GetComponent<AutoLayoutElement>() != null) {
			var autoLayoutElements = parent.GetComponents<AutoLayoutElement>();
			foreach (var autoLayoutElement in autoLayoutElements) autoLayoutElement.removeDependent(gameObject);
		}
	}

	private void addToParent(GameObject parent) {
		if (parent != null && parent.GetComponent<AutoLayoutElement>() != null) {
			var autoLayoutElements = parent.GetComponents<AutoLayoutElement>();
			foreach (var autoLayoutElement in autoLayoutElements) autoLayoutElement.addDependent(gameObject);
		}
	}

	private void applyLayoutRules() {
		//needsUpdate = false;

		//Debug.Log("Applying update @ frame " + Time.frameCount + ", " + transform.parent.gameObject.name + "." + gameObject.name + ", with " + dependents.Count + " dependents");

		// Applies layout to self
		if (OnShouldApplyLayout != null) {
			OnShouldApplyLayout();
		}

		// Propagates to dependents
		var destroyedDependents = new List<GameObject>();
		foreach (var dependent in dependents) {
			if (dependent != null && dependent.activeInHierarchy) {
				var autoLayoutElements = dependent.GetComponents<AutoLayoutElement>();
				foreach (var autoLayoutElement in autoLayoutElements) autoLayoutElement.requestUpdate();
			} else {
				// Object has been destroyed!
				Debug.LogWarning("Dependent object was destroyed during gameplay, will remove it from list of dependents");
				destroyedDependents.Add(dependent);
			}
		}

		foreach (var dependent in destroyedDependents) {
			dependents.Remove(dependent);
		}
	}
}
