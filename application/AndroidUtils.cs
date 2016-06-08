#if UNITY_ANDROID && !UNITY_EDITOR
#define USE_ANDROID
#endif


/**
 * @author zeh fernando
 */
using System;
using UnityEngine;

class AndroidUtils {

	// Use:
	// runOnAndroidUiThread(someMethod)
	public static void runOnAndroidUiThread(Action target) {
		// Calls an Android activity on the UI Thread (needed for some interactions with Views)
		#if USE_ANDROID
			using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
				using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
					activity.Call("runOnUiThread", new AndroidJavaRunnable(target));
					// Other examples:
					//string a = activity.Call<string>("someNomw", param);
					//window.Call("setFlags", flagsValue, -1); // (int)0x7FFFFFFF
					//window.Call("setStatusBarColor", unchecked((int)_statusBarColor)); // for an uint
				}
			}
		#else
			Debug.Log("[Android] Would call [" + target + "] later on the UI thread");
		#endif
	}
}
