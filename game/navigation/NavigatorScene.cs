using System.Collections.Generic;
using UnityEngine;

public class NavigatorScene:MonoBehaviour {

	// Parameters
	public GameObject cameraTarget;
	public bool preferFullscreen;
	public string trackingId;

	// For tracking
	public delegate void NavigatorSceneEvent();

	public event NavigatorSceneEvent OnStartedShowing;
	public event NavigatorSceneEvent OnFinishedShowing;
	public event NavigatorSceneEvent OnPause;
	public event NavigatorSceneEvent OnResume;
	public event NavigatorSceneEvent OnStartedHiding;
	public event NavigatorSceneEvent OnFinishedHiding;

	// Properties
	private bool isActivated = true;
	private bool disableRendering = false;

	private Dictionary<string, object> bundle;


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


	// ================================================================================================================
	// PUBLIC INTERFACE ------------------------------------------------------------------------------------------------
	
	public string getBundleParameterAsString(string key, string defaultValue = "") {
		object value;
		return bundle != null && bundle.TryGetValue(key, out value) ? (string)value : defaultValue;
	}

	public int getBundleParameterAsInt(string key, int defaultValue = 0) {
		object value;
		return bundle != null && bundle.TryGetValue(key, out value) ? (int)value : defaultValue;
	}

	public T getBundleParameterAs<T>(string key, T defaultValue = default(T)) {
		object value;
		return bundle != null && bundle.TryGetValue(key, out value) ? (T)value : defaultValue;
	}


	// ================================================================================================================
	// EVENT INTERFACE ------------------------------------------------------------------------------------------------

	public virtual void initialize(Dictionary<string, object> newBundle) {
		bundle = newBundle;
	}
	
	public virtual void onStartedShowing() {
		// Changes screen
		ApplicationChrome.statusBarState = ApplicationChrome.navigationBarState = ApplicationChrome.States.Visible;
		ApplicationChrome.dimmed = preferFullscreen;

		activate();

		// Track page
		//Debug.Log("Trying to track ::: [" + "navigation:screen:" + trackingId + "]");
		TrackingManager.getInstance().trackScreen(trackingId);

		if (OnStartedShowing != null) OnStartedShowing();
	}

	public virtual void onFinishedShowing() {
		if (OnFinishedShowing != null) OnFinishedShowing();
	}

	public virtual void onPause() {
		if (OnPause != null) OnPause();
	}

	public virtual void onResume() {
		if (OnResume != null) OnResume();
	}

	public virtual void onStartedHiding() {
		if (OnStartedHiding != null) OnStartedHiding();
	}

	public virtual void onFinishedHiding() {
		deActivate();
		if (OnFinishedHiding != null) OnFinishedHiding();
	}


	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

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
