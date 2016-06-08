using System;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode()]
#endif

public class AutoResizer:AutoLayoutElement {

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

	public enum FitTypes {
		FREE_DISTORT,
		FIT_INSIDE,
		FIT_OUTSIDE,
		FIT_X,
		FIT_Y
	}

	public GameObject parentLeft;
	public AlignmentsX parentLeftAnchor;
	public float marginLeft;

	public GameObject parentRight;
	public AlignmentsX parentRightAnchor;
	public float marginRight;

	public bool autoMoveX;

	public GameObject parentTop;
	public AlignmentsY parentTopAnchor;
	public float marginTop;

	public GameObject parentBottom;
	public AlignmentsY parentBottomAnchor;
	public float marginBottom;

	public bool autoMoveY;

	public FitTypes fitType;
	public float fitAspectRatio = 1;

	public float multiplyScale = 1;

	public bool updateAutomatically;
	public bool flipX;
	public bool flipY;


	// ================================================================================================================
	// MAIN EVENT INTERFACE -------------------------------------------------------------------------------------------

	void Awake() {
		shouldAutoUpdate = updateAutomatically;
		OnShouldApplyLayout += checkAllParents;
		OnShouldApplyLayout += applyLayout;
	}


	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

	#if UNITY_EDITOR
	private void onUpdate() {
		applyLayout();
	}
	#endif

	private void checkAllParents() {
		// Check if the parents were changed
		checkParents(parentLeft, parentRight, parentTop, parentBottom);
	}

	private void applyLayout() {
		// Applies the alignment position to this object

		// TODO: use screen coordinates if null
		// TODO: allow some axis to be free

		// Reset size for better calculation (in case any scale is set to 0)
		gameObject.transform.localScale = new Vector3(1, 1, 1);

		// Find own bounding box
		var bounds = getBoundsOrPoint(gameObject);

		// Find actual mapping positions
		var left	= findPosition(parentLeft,		(int)parentLeftAnchor,		bounds.min.x, Axis.X) + marginLeft;
		var top		= findPosition(parentTop,		(int)parentTopAnchor,		bounds.max.y, Axis.Y) - marginTop;
		var right	= findPosition(parentRight,		(int)parentRightAnchor,		bounds.max.x, Axis.X) - marginRight;
		var bottom	= findPosition(parentBottom,	(int)parentBottomAnchor,	bounds.min.y, Axis.Y) + marginBottom;

		// Set positions
		var scaleX = (right - left) / (bounds.max.x - bounds.min.x);
		var scaleY = (top - bottom) / (bounds.max.y - bounds.min.y);
		var scaleZ = gameObject.transform.localScale.z;

		var scaleAspectRatio = scaleX / scaleY;
		var objectIsWider = scaleAspectRatio < fitAspectRatio;

		var oldScaleX = scaleX;
		var oldScaleY = scaleY;

		if (fitType == FitTypes.FIT_X || (fitType == FitTypes.FIT_INSIDE && objectIsWider) || (fitType == FitTypes.FIT_OUTSIDE && !objectIsWider)) {
			// Keep ratio and use X as base
			scaleY = scaleX / fitAspectRatio;
		} else if (fitType == FitTypes.FIT_Y || (fitType == FitTypes.FIT_INSIDE && !objectIsWider) || (fitType == FitTypes.FIT_OUTSIDE && objectIsWider)) {
			// Keep ratio and use Y as base
			scaleX = scaleY * fitAspectRatio;
		} else {
			// Free distort, do nothing
		}

		scaleX *= multiplyScale * (flipX ? -1 : 1);
		scaleY *= multiplyScale * (flipY ? -1 : 1);

		var newX = gameObject.transform.position.x;
		var newY = gameObject.transform.position.y;
		var newZ = gameObject.transform.position.z;

		if (autoMoveX) newX = left + (gameObject.transform.position.x - bounds.min.x) * oldScaleX;
		if (autoMoveY) newY = bottom + (gameObject.transform.position.y - bounds.min.y) * oldScaleY;

		gameObject.transform.position = new Vector3(newX, newY, newZ);
		gameObject.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);

	}

	private float findPosition(GameObject parent, int parentAnchorType, float currentPos, Axis axis) {

		// Check if it's valid
		if ((Alignments)parentAnchorType != Alignments.NONE) {

			var parentBoundsTry = getBounds(parent);

			if (parentBoundsTry == null) {
				// Invalid parent
				Debug.LogWarning("Error: GameObject must have a renderer or a collider attached to work as an AutoResizer parent");
			} else {
				// Valid parent
				var parentBounds = (Bounds)parentBoundsTry;
				var parentAnchor = parent == null ? parentBounds.center : parent.transform.position;

				// Find boundaries
				float parentPos = 0;
				float parentMinPos = 0;
				float parentMaxPos = 0;

				switch (axis) {
					case Axis.X:
						parentPos = parentAnchor.x;
						parentMinPos = parentBounds.min.x;
						parentMaxPos = parentBounds.max.x;
						break;
					case Axis.Y:
						parentPos = parentAnchor.y;
						parentMinPos = parentBounds.min.y;
						parentMaxPos = parentBounds.max.y;
						break;
					case Axis.Z:
						parentPos = parentAnchor.z;
						parentMinPos = parentBounds.min.z;
						parentMaxPos = parentBounds.max.z;
						break;

				}

				// Find base
				switch ((Alignments)parentAnchorType) {
					case Alignments.ANCHOR:
						return parentPos;
					case Alignments.START:
						return parentMinPos;
					case Alignments.MIDDLE:
						return (parentMinPos + parentMaxPos) / 2;
					case Alignments.END:
						return parentMaxPos;
				}
			}
		}

		return currentPos;
	}
}