using UnityEngine;
using System.Collections.Generic;

public class GameObjectUtils {

	public static List<GameObject> getAllChildren(GameObject gameObject) {
		var children = new List<GameObject>();
		foreach (Transform child in gameObject.transform) {
			children.Add(child.gameObject);
		}
		return children;
	}

	public static void removeAllChildren(GameObject gameObject) {
		var children = getAllChildren(gameObject);
		foreach(GameObject child in children) {
			destroySafely(child.gameObject);
		}
	}

	// TODO: allow to set bounds, align inside, outside, ...?

	public static void resizeToFit(GameObject gameObject, float maxWidth = 0, float maxHeight = 0, float maxDepth = 0) {
		// Resize an object to the desired size

		// Reset size for better calculation (in case any scale is set to 0)
		gameObject.transform.localScale = Vector3.one;

		// Find bounding box
		var bounds = getBounds(gameObject);

		// Scale accordingly
		var width = bounds.max.x - bounds.min.x;
		var height = bounds.max.y - bounds.min.y;
		var depth = bounds.max.z - bounds.min.z;

		var newScale = 0.0f;
		var tempScale = 0.0f;
		var scaleWillChange = false;

		if (maxHeight != 0) {
			// Decide by height
			tempScale = maxHeight / height;
			newScale = scaleWillChange ? Mathf.Min(tempScale, newScale) : tempScale;
			scaleWillChange = true;
		}

		if (maxDepth != 0) {
			// Decide by depth
			tempScale = maxDepth / depth;
			newScale = scaleWillChange ? Mathf.Min(tempScale, newScale) : tempScale;
			scaleWillChange = true;
		}

		if (maxWidth != 0) {
			// Decide by width
			tempScale = maxWidth / width;
			newScale = scaleWillChange ? Mathf.Min(tempScale, newScale) : tempScale;
			scaleWillChange = true;
		}

		if (scaleWillChange) {
			// Finally, scale
			gameObject.transform.localScale = new Vector3(newScale, newScale, newScale);
		}
	}

	public static void alignToVector(GameObject gameObject, Vector3 targetPosition, float alignX, float alignY, float alignZ) {
		// Align is from 0 to 1 (min to max)

		// TODO: move this somewhere else
		// TODO: allow to set bounds, align inside, outside, ...?

		// Also, position the object so we only use the bottom
		/*
		var newX = gameObject.transform.localPosition.x;
		var newZ = gameObject.transform.localPosition.z;

		newX = 0; // left + (gameObject.transform.position.x - bounds.min.x) * oldScaleX;
		var newY = (gameObject.transform.localPosition.y - bounds.min.y) * newScale;
		newZ = 0;

		gameObject.transform.localPosition = new Vector3(newX, newY, newZ);
		*/

		// Find position and bounding box
		var bounds = getBounds(gameObject);
		var pivot = gameObject.transform.position;

		// Calculate and set new positions
		var newX = pivot.x - MathUtils.map(alignX, 0.0f, 1.0f, bounds.min.x, bounds.max.x);
		var newY = pivot.y - MathUtils.map(alignY, 0.0f, 1.0f, bounds.min.y, bounds.max.y);
		var newZ = pivot.z - MathUtils.map(alignZ, 0.0f, 1.0f, bounds.min.z, bounds.max.z);

		gameObject.transform.position = targetPosition + new Vector3(newX, newY, newZ);
	}

	public static Bounds getBounds(GameObject gameObject) {
		// Gets the best bounding box from a game object, using world space coordinates

		var bounds = new Bounds(gameObject.transform.position, new Vector3(0, 0, 0));
		var hasBounds = true;

		if (gameObject.GetComponent<Collider>() != null) {
			// Use collider
			bounds = gameObject.GetComponent<Collider>().bounds;
		} else if (gameObject.GetComponent<Renderer>() != null) {
			// Use renderer
			bounds = gameObject.GetComponent<Renderer>().bounds;
		} else {
			hasBounds = false;
		}

		// Find children if needed
		foreach (Transform child in gameObject.transform) {
			var childBounds = getBounds(child.gameObject);
			if (childBounds.extents.magnitude > 0) {
				if (!hasBounds) {
					bounds = childBounds;
					hasBounds = true;
				} else {
					bounds.Encapsulate(childBounds);
				}
			}
		}

		return bounds;
	}

	public static void createMeshColliders(GameObject gameObject, bool actOnChildren) {
		// Create mesh colliders on a specific gameObject

		// First, remove existing colliders
		removeComponents<Collider>(gameObject);

		// Now, create mesh colliders if meshes exist
		var mesh = gameObject.GetComponent<MeshFilter>();
		if (mesh != null) {
			var meshCollider = gameObject.AddComponent<MeshCollider>();
			meshCollider.convex = false;
			meshCollider.isTrigger = false;
		}

		// Recurse into children if desired
		if (actOnChildren) {
			foreach (Transform child in gameObject.transform) {
				createMeshColliders(child.gameObject, true);
			}
		}
	}

	public static void setRenderersVisibility(GameObject gameObject, bool visible) {
		var renderers = gameObject.GetComponentsInChildren<Renderer>();
		foreach (var renderer in renderers) {
			renderer.enabled = visible;
		}
	}

	public static void setCollidersEnabled(GameObject gameObject, bool visible) {
		var colliders = gameObject.GetComponentsInChildren<Collider>();
		foreach (var collider in colliders) {
			collider.enabled = visible;
		}
	}

	public static void setAlpha(GameObject gameObject, float alpha) {
		var renderers = gameObject.GetComponentsInChildren<Renderer>();
		foreach (var renderer in renderers) {
			foreach (var material in renderer.sharedMaterials) {
				material.color = new Color(material.color.r, material.color.g, material.color.b, alpha);
			}
		}
	}

	public static void removeComponents<T>(GameObject gameObject) where T:Component {
		var components = gameObject.GetComponents<T>();
		foreach (Component component in components) {
			destroySafely(component);
		}
	}

	public static void destroySafely(GameObject gameObject) {
		#if UNITY_EDITOR
			GameObject.DestroyImmediate(gameObject);
		#else
			GameObject.Destroy(gameObject);
		#endif
	}

	public static void destroySafely(Component component) {
		#if UNITY_EDITOR
			GameObject.DestroyImmediate(component);
		#else
			GameObject.Destroy(component);
		#endif
	}
}

