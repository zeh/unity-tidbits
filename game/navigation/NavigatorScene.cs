using UnityEngine;
using System.Collections;

public class NavigatorScene:MonoBehaviour {

	// Properties
	public GameObject cameraTarget;
	public bool preferFullscreen;
	public string trackingId;

	private bool isActivated = true;
	private bool disableRendering = false;

	// ================================================================================================================
	// EXTENDED INTERFACE ---------------------------------------------------------------------------------------------

	void Start() {
		deActivate();
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape) && isActivated) {
			// Look for all "back" buttons...
			var buttons = gameObject.GetComponentsInChildren<RoundedButtonNavigateTo>();
			var acted = false;
			foreach (var button in buttons) {
				if (button.isBackEquivalent) {
					acted = true;
					button.performActionPublic();
					break;
				}
			}
			if (!acted) {
				Debug.Log("No back, will quit!");
				Application.Quit();
			}
		}
	}

	public virtual void onStartedShowing() {
		// Changes screen
		ApplicationChrome.statusBarState = ApplicationChrome.navigationBarState = ApplicationChrome.States.Visible;
		ApplicationChrome.dimmed = preferFullscreen;

		activate();

		// Track page
		//Debug.Log("Trying to track ::: [" + "navigation:screen:" + trackingId + "]");
		TrackingManager.getInstance().trackScreen(trackingId);
	}

	public virtual void onFinishedShowing() {
	}

	public virtual void onStartedHiding() {
	}

	public virtual void onFinishedHiding() {
		deActivate();
	}

	private void deActivate() {
		//gameObject.SetActive(false);
		//renderer.enabled = false;
		//gameObject.SetActive(false);
		if (isActivated) {
			// Disable all rendering
			if (disableRendering) {
				var renderers = GetComponentsInChildren<Renderer>();
				foreach (var renderer in renderers) {
					renderer.enabled = false;
				}
			}

			isActivated = false;
		}
	}

	private void activate() {
		//gameObject.SetActive(true);
		//renderer.enabled = true;
		//gameObject.SetActive(true);
		if (!isActivated) {
			// Disable all rendering
			if (disableRendering) {
				var renderers = GetComponentsInChildren<Renderer>();
				foreach (var renderer in renderers) {
					renderer.enabled = true;
				}
			}

			isActivated = true;
		}
	}
}
