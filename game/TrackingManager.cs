using UnityEngine;
using UnityEngine.Analytics;
using System.Collections.Generic;
using System;

public class TrackingManager:MonoBehaviour {

	/*
	 * A MonoBehavior to track using different systems.
	 */

	// https://github.com/googleanalytics/google-analytics-plugin-for-unity

	public GoogleAnalyticsV3 googleAnalyticsTracker;
	public bool useGoogleAnalytics;

	public bool useUnityAnalytics;

	public bool loggingEnabled;
	public bool trackingEnabled;

	private static TrackingManager instance;


	// ================================================================================================================
	// MAIN EVENT INTERFACE -------------------------------------------------------------------------------------------

	void Awake() {
		instance = this;
	}
	
	void Start() {
		if (!trackingEnabled) return;

		// Google Analytics
		if (useGoogleAnalytics && googleAnalyticsTracker != null) {
			// Start session
			googleAnalyticsTracker.StartSession();
		}

		// Start session
		// The id is set in "Edit > Project Settings > Player" (as the cloud id)
			
		// Custom tracking: platform
		trackCustomEventPlatform();
			
		// Custom tracking: application
		trackCustomEventApplication();

		// Custom tracking: device
		trackCustomEventDevice();
			
		// Custom tracking: user
		trackCustomEventUser();

		// TODO: performance (average fps, lowest fps)
	}

	void Update() {
	}

	void OnDestroy() {
		if (!trackingEnabled) return;

		if (useGoogleAnalytics && googleAnalyticsTracker != null) {
			googleAnalyticsTracker.StopSession();
		}
	}

	/* 
	public void DispatchHits();
	 */

	private void trackCustomEventApplication() {
		trackCustomEvent("info.application",
			"version", "~" + Application.version.ToString(),				// Float
			"uri", Application.absoluteURL,									// "" on editor and while testing on Android
			"bundle", Application.bundleIdentifier,							// "com.zehfernando.KanaGenius"
			"install_mode", Application.installMode							// "Editor" (testing on editor), "DeveloperBuild" (testing on Android)
		);
	}

	private void trackCustomEventPlatform() {
		#if UNITY_EDITOR
		string platformId = "editor-" + Application.platform;
		#elif UNITY_IPHONE
		string platformId = "ios";
		#elif UNITY_ANDROID
		string platformId = "android";
		#elif UNITY_STANDALONE_OSX
		string platformId = "osx";
		#elif UNITY_STANDALONE_WIN
		string platformId = "windows";
		#elif UNITY_WEBPLAYER
		string platformId = "web";
		#elif UNITY_WII
		string platformId = "wii";
		#elif UNITY_PS3
		string platformId = "ps3";
		#elif UNITY_XBOX360
		string platformId = "xbox360";
		#elif UNITY_FLASH
		string platformId = "flash";
		#elif UNITY_BLACKBERRY
		string platformId = "blackberry";
		#elif UNITY_WP8
		string platformId = "windows-phone-8";
		#elif UNITY_METRO
		string platformId = "windows-metro";
		#else
		string platformId = "unknown-" + Application.platform;
		#endif

		trackCustomEvent("info.platform",
			"id", platformId,												// "editor-WindowsEditor", "android"
			"os", SystemInfo.operatingSystem,								// "Windows 7 Service Pack 1 (6.1.7601) 64bit", "Android OS 5.1 / API-22 (LMY47D/1743759)"
			"unity_runtime_version", Application.unityVersion,				// "5.0.1f1"
			"internet", Application.internetReachability					// "ReachableViaLocalAreaNetwork"
		);
	}

	private void trackCustomEventDevice() {
		trackCustomEvent("info.device",
			"device_model", SystemInfo.deviceModel,							// "AMD Phenom(tm) II X4 955 Processor (8191 MB)", "LGE Nexus 5"
			// "device_name", SystemInfo.deviceName,						// "ZEH-PC7"
			"device_type", SystemInfo.deviceType,							// "Desktop", "Handheld"
			"system_memory", "~" + SystemInfo.systemMemorySize.ToString() + "MB",		// 8191 as number
			"graphics_memory", "~" + SystemInfo.graphicsMemorySize.ToString(),
			"is_mobile", Application.isMobilePlatform,
			"is_console", Application.isConsolePlatform,
			// TODO: SystemInfo.processorType, processorCount
			"dpi", "~" + Screen.dpi.ToString(),
			"screen_width", "~" + Screen.width.ToString(),
			"screen_height", "~" + Screen.height.ToString(),
			"screen_size", "~" + (Math.Round(Mathf.Sqrt(Screen.width*Screen.width+Screen.height*Screen.height)/Screen.dpi*10f)/10f).ToString() + "in"
		);
	}

	private void trackCustomEventUser() {
		trackCustomEvent("info.user",
			"os_language", Application.systemLanguage						// "English"
		);
	}

	private void trackUACustomEvent(string eventType, Dictionary<string, object> parameters) {
		// Track a custom event using Unity Analytics
		//https://analytics.cloud.unity3d.com/docs
		// Max 10 params per custom event
		
		var result = Analytics.CustomEvent(eventType, parameters);
		if (result != AnalyticsResult.Ok) {
			Debug.LogWarning("TRACKING :: Error: tracking custom event [" + eventType + "] didn't work, returned [" + result + "]");
		}
	}

	private void trackGACustomEvent(string eventType, Dictionary<string, object> parameters) {
		foreach (var entry in parameters) {
			googleAnalyticsTracker.LogEvent(new EventHitBuilder()
				.SetEventCategory(eventType)
				.SetEventAction(entry.Key)
				.SetEventLabel(Convert.ToString(entry.Value)));
				//.SetEventValue(5));
		}
	}


	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	public static TrackingManager getInstance() {
		return instance;
	}

	public void trackScreen(string screenId) {
		/*
		if (useGameAnalytics) {
			GA.API.Design.NewEvent("navigation:screen:" + screenId);
		}
		*/

		if (!trackingEnabled) return;

		if (loggingEnabled) {
			Debug.Log("TRACKING :: Screen : Screen id [" + screenId + "]");
		}
		if (useGoogleAnalytics && googleAnalyticsTracker != null) {
			googleAnalyticsTracker.LogScreen(screenId);

			// //Builder Hit with all App View parameters (all parameters required):
			//googleAnalytics.LogScreen(new AppViewHitBuilder() .SetScreenName("Main Menu"));
		}
		if (useUnityAnalytics) {
			trackUACustomEvent("event.navigation.screen", new Dictionary<string, object>{{"id", screenId}});
		}
	}

	private void trackCustomEvent(string eventType, params object[] paramValues) {
		if (!trackingEnabled) return;

		var parametersDict = new Dictionary<string, object>();
		for (var i = 0; i < paramValues.Length; i += 2) {
			parametersDict.Add(paramValues[i] as string, Convert.ToString(paramValues[i+1]));
		}

		if (loggingEnabled) {
			string paramList = "";
			for (var i = 0; i < paramValues.Length; i += 2) {
				if (i > 0) paramList += ",";
				paramList += (paramValues[i] as string) + ":" + paramValues[i + 1];
			}

			Debug.Log("TRACKING :: Custom Event :: Event [" + eventType + "], parameters [" + paramList + "]");
		}

		if (useGoogleAnalytics && googleAnalyticsTracker) {
			trackGACustomEvent(eventType, parametersDict);
		}

		if (useUnityAnalytics) {
			trackUACustomEvent(eventType, parametersDict);
		}
	}

	/* Exceptions
	 * public void LogException(string exceptionDescription, bool isFatal);
	 * 
	 * googleAnalytics.LogException("Incorrect input exception", true);

    //Builder Hit with all Exception parameters
    googleAnalytics.LogException(new ExceptionHitBuilder()
        .SetExceptionDescription("Incorrect input exception")
        .SetFatal(true));

    //Builder Hit with minimum required Exception parameters
    googleAnalytics.LogException(new ExceptionHitBuilder());
	 * */

	/*
	 * Timings
	 * 
	 * googleAnalytics.LogTiming("Loading", 50L, "Main Menu", "First Load");

    //Builder Hit with all Timing parameters
    googleAnalytics.LogTiming(new TimingHitBuilder()
        .SetTimingCategory("Loading")
        .SetTimingInterval(50L)
        .SetTimingName("Main Menu")
        .SetTimingLabel("First load"));

    //Builder Hit with minimum required Timing parameters
    googleAnalytics.LogTiming(new TimingHitBuilder()
        .SetTimingCategory("Loading"));
	 * */

	/* Social
	 *
	 * googleAnalytics.LogSocial("twitter", "retweet", "twitter.com/googleanalytics/status/482210840234295296");

    //Builder Hit with all Social parameters (all parameters required)
    googleAnalytics.LogSocial(new SocialHitBuilder()
        .SetSocialNetwork("Twitter")
        .SetSocialAction("Retweet")
        .SetSocialTarget("twitter.com/googleanalytics/status/482210840234295296"));
	 * */
	 
	public void trackTransaction(string productId, string purchaseMethod, float price, string currency) {
		if (!trackingEnabled) return;

		if (loggingEnabled) {
			Debug.Log("TRACKING :: Transaction : Product [" + productId + "], purchase method [" + purchaseMethod + "], price [" + price + ", currency [" + currency + "]");
		}
		if (useGoogleAnalytics && googleAnalyticsTracker != null) {
			googleAnalyticsTracker.LogTransaction(productId, purchaseMethod, price, 0.0, 0.0, currency);
		}
		if (useUnityAnalytics) {
			Analytics.Transaction(purchaseMethod + "-" + productId, Convert.ToDecimal(price), currency);
		}
	}

	/* Ecommerce - transaction
	 * 
	 * googleAnalytics.LogTransaction("TRANS001", "Coin Store", 3.0, 0.0, 0.0);
    googleAnalytics.LogTransaction("TRANS001", "Coin Store", 3.0, 0.0, 0.0, "USD");

    //Builder Hit with all Transaction parameters
    googleAnalytics.LogTransaction(new TransactionHitBuilder()
        .SetTransactionID("TRANS001")
        .SetAffiliation("Coin Store")
        .SetRevenue(3.0)
        .SetTax(0)
        .SetShipping(0.0)
        .SetCurrencyCode("USD"));

    //Builder Hit with minimum required Transaction parameters
    googleAnalytics.LogTransaction(new TransactionHitBuilder()
        .SetTransactionID("TRANS001")
        .SetAffiliation("Coin Store"));
	 * 
	 * Ecommerce - item hit
	 * 
	 * googleAnalytics.LogItem("TRANS001", "Sword", "SWORD1223", "Weapon", 3.0, 2);
    googleAnalytics.LogItem("TRANS001", "Sword", "SWORD1223", "Weapon", 3.0, 2, "USD");

    //Builder Hit with all Item parameters
    googleAnalytics.LogItem(new ItemHitBuilder()
        .SetTransactionID("TRANS001")
        .SetName("Sword")
        .SetSKU("SWORD1223")
        .SetCategory("Weapon")
        .SetPrice(3.0)
        .SetQuantity(2)
        .SetCurrencyCode("USD"));

    //Builder Hit with minimum required Item parameters
    googleAnalytics.LogItem(new ItemHitBuilder()
        .SetTransactionID("TRANS001")
        .SetName("Sword")
        .SetSKU("SWORD1223"));
	 * 
	 */

	/* Custom dimensions, as part of an event
	 * 
	 * public T SetCustomDimension(int dimensionNumber, string value);
	 * 
	 * googleAnalytics.LogScreen(new AppViewHitBuilder()
        .SetScreenName("Another screen")
        .SetCustomDimension(1, "200"));
	 * */

	/* Custom metrics
	 * 
	 * public T SetCustomMetric(int metricNumber, string value);
Sample Hit:

    googleAnalytics.LogEvent(new EventHitBuilder()
        .SetEventCategory("Achievement")
        .SetEventAction("Unlocked")
        .SetEventLabel("Slay 10 dragons")
        .SetEventValue(5)
        .SetCustomMetric(3, "200"));
	 * */

	/* Campaign parameters, as part of an event
	 * 
	 * googleAnalytics.LogTiming(new TimingHitBuilder()
        .SetTimingCategory("Loading")
        .SetTimingInterval(50L)
        .SetTimingName("Main Menu")
        .SetTimingLabel("First load")
        .SetCampaignName("Summer Campaign")
        .SetCampaignSource("google")
        .SetCampaignMedium("cpc")
        .SetCampaignKeyword("games")
        .SetCampaignContent("Free power ups")
        .SetCampaignId("Summer1"));

    //Builder Hit with minimum required Campaign parameters
    googleAnalytics.LogTiming(new TimingHitBuilder()
        .SetTimingCategory("Loading")
        .SetTimingInterval(50L)
        .SetTimingName("Main Menu")
        .SetTimingLabel("First load")
        .SetCampaignSource("google");
	 * */

	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

}
