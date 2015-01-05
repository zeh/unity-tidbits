using System;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode()]
#endif

public class AutoAligner:AutoLayoutElement {

	// Enums
	private enum Alignments:int {
		NONE = 0,
		ANCHOR,
		START,
		MIDDLE,
		END,
	}

	private enum Axis {
		X,
		Y,
		Z
	}

	public enum AlignmentsX:int {
		NONE = 0,
		ANCHOR,
		LEFT,
		CENTER,
		RIGHT,
	}

	public enum AlignmentsY:int {
		NONE = 0,
		ANCHOR,
		BOTTOM,
		MIDDLE,
		TOP,
	}

	public enum AlignmentsZ:int {
		NONE = 0,
		ANCHOR,
		FRONT,
		CENTER,
		BACK,
	}

	public GameObject parentX;
	public AlignmentsX anchorX;
	public AlignmentsX parentAnchorX;
	public float marginX;

	public GameObject parentY;
	public AlignmentsY anchorY;
	public AlignmentsY parentAnchorY;
	public float marginY;

	public GameObject parentZ;
	public AlignmentsZ anchorZ;
	public AlignmentsZ parentAnchorZ;
	public float marginZ;


	// ================================================================================================================
	// MAIN EVENT INTERFACE -------------------------------------------------------------------------------------------

	void Awake() {
		OnShouldApplyLayout += checkAllParents;
		OnShouldApplyLayout += applyLayout;
	}


	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

	private void checkAllParents() {
		// Check if the parents were changed
		checkParents(parentX, parentY, parentZ);
	}

	private void applyLayout() {
		// Applies the alignment position to this object

		// Find own bounding box
		var bounds = getBoundsOrPoint(gameObject);

		// Find actual mapping positions
		var newX = findAlignment((int)anchorX, (int)parentAnchorX, gameObject.transform.position.x, bounds.min.x, bounds.max.x, parentX, Axis.X, marginX);
		var newY = findAlignment((int)anchorY, (int)parentAnchorY, gameObject.transform.position.y, bounds.min.y, bounds.max.y, parentY, Axis.Y, marginY);
		var newZ = findAlignment((int)anchorZ, (int)parentAnchorZ, gameObject.transform.position.z, bounds.min.z, bounds.max.z, parentZ, Axis.Z, marginZ);

		// Set positions
		gameObject.transform.position = new Vector3(newX, newY, newZ);

	}

	private float findAlignment(int anchor, int parentAnchor, float currentPos, float minPos, float maxPos, GameObject parent, Axis axis, float margin) {

		// Check if it's valid
		if (parent != null && (Alignments)anchor != Alignments.NONE && (Alignments)parentAnchor != Alignments.NONE) {

			if (parent.renderer == null && parent.collider == null) {
				// Invalid parent
				Debug.LogWarning("Error: GameObject must have a renderer or a collider attached to work as an Aligner parent");
			} else {
				// Valid parent
				var parentBounds = parent.renderer == null ? parent.collider.bounds : parent.renderer.bounds;

				// Find boundaries
				float parentPos = 0;
				float parentMinPos = 0;
				float parentMaxPos = 0;

				switch (axis) {
					case Axis.X:
						parentPos = parent.transform.position.x;
						parentMinPos = parentBounds.min.x;
						parentMaxPos = parentBounds.max.x;
						break;
					case Axis.Y:
						parentPos = parent.transform.position.y;
						parentMinPos = parentBounds.min.y;
						parentMaxPos = parentBounds.max.y;
						break;
					case Axis.Z:
						parentPos = parent.transform.position.z;
						parentMinPos = parentBounds.min.z;
						parentMaxPos = parentBounds.max.z;
						break;

				}

				// Find base
				float basePos = 0;
				switch ((Alignments)parentAnchor) {
					case Alignments.ANCHOR:
						basePos = parentPos;
						break;
					case Alignments.START:
						basePos = parentMinPos;
						break;
					case Alignments.MIDDLE:
						basePos = (parentMinPos + parentMaxPos) / 2;
						break;
					case Alignments.END:
						basePos = parentMaxPos;
						break;
				}

				// Applies
				switch ((Alignments)anchor) {
					case Alignments.ANCHOR:
						return basePos + margin;
					case Alignments.START:
						return basePos + margin + (currentPos - minPos);
					case Alignments.MIDDLE:
						return basePos + margin + (currentPos - minPos - (maxPos - minPos) / 2);
					case Alignments.END:
						return basePos - margin + (currentPos - maxPos);
				}
			}
		}

		return currentPos;
	}

}