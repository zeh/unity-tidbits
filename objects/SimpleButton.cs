using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleButton:MonoBehaviour {

	/*
	A button with simple actions; syntax sugar
	*/

	// ================================================================================================================
	// MAIN EVENT INTERFACE -------------------------------------------------------------------------------------------

	// http://docs.unity3d.com/ScriptReference/MonoBehaviour.html

	void OnMouseDown() {
		// Called when the user has pressed the mouse button while over the GUIElement or Collider.
		Debug.Log("down");
	}

	void OnMouseDrag() {
		// Called every frame when the user has clicked on a GUIElement or Collider and is still holding down the mouse (inside or not)
		Debug.Log("drag");
	}

	void OnMouseEnter() {
		// Called when the mouse entered the GUIElement or Collider.
		Debug.Log("enter");
	}

	void OnMouseExit() {
		// Called when the mouse is not any longer over the GUIElement or Collider.
		Debug.Log("exit");
	}

	void OnMouseOver() {
		// Called every frame while the mouse is over the GUIElement or Collider.
		//Debug.Log("over");
	}

	void OnMouseUp() {
		// Called when the user has released the mouse button
		Debug.Log("up");
	}

	void OnMouseUpAsButton() {
		// Only called when the mouse is released over the same GUIElement or Collider as it was pressed
		Debug.Log("up as button");
	}


	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	// ================================================================================================================
	// ACCESSOR INTERFACE ---------------------------------------------------------------------------------------------

	// ================================================================================================================
	// EXTENDABLE INTERFACE -------------------------------------------------------------------------------------------

	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

}