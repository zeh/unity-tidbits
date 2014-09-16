using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleButton:MonoBehaviour {

	/*
	A button with simple actions; syntax sugar
	*/

	// Properties
	private bool _isPointerOver;
	private bool _isPressed;

	// ================================================================================================================
	// MAIN EVENT INTERFACE -------------------------------------------------------------------------------------------

	// http://docs.unity3d.com/ScriptReference/MonoBehaviour.html

	void OnMouseDown() {
		// Called when the user has pressed the mouse button while over the GUIElement or Collider.
		_isPressed = true;
		animatePress();
	}

	void OnMouseDrag() {
		// Called every frame when the user has clicked on a GUIElement or Collider and is still holding down the mouse (inside or not)
	}

	void OnMouseEnter() {
		// Called when the mouse entered the GUIElement or Collider.
		_isPointerOver = true;
		animateOver();
	}

	void OnMouseExit() {
		// Called when the mouse is not any longer over the GUIElement or Collider.
		_isPointerOver = false;
		animateOut();
	}

	void OnMouseOver() {
		// Called every frame while the mouse is over the GUIElement or Collider.
	}

	void OnMouseUpAsButton() {
		// Only called when the mouse is released over the same GUIElement or Collider as it was pressed. This is called BEFORE OnMouseUp
		if (_isPressed) {
			performAction();
		}
	}

	void OnMouseUp() {
		// Called when the user has released the mouse button
		if (_isPressed) {
			_isPressed = false;
			animateRelease();
		}
	}


	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	// ================================================================================================================
	// ACCESSOR INTERFACE ---------------------------------------------------------------------------------------------

	protected bool isPointerOver {
		get {
			return _isPointerOver;
		}
	}

	protected bool isPressed {
		get {
			return _isPressed;
		}
	}
	

	// ================================================================================================================
	// EXTENDABLE INTERFACE -------------------------------------------------------------------------------------------

	protected virtual void animatePress() {
		// Press
	}

	protected virtual void animateRelease() {
		// Release
	}

	protected virtual void animateOver() {
		// Pointer over
	}

	protected virtual void animateOut() {
		// Pointer out
	}

	protected virtual void performAction() {
		// Pressed and released without moving: execute action
	}


	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

}