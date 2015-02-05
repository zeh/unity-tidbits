using UnityEngine;
using UnityEngine.Cloud.Analytics;
using System.Collections.Generic;
using System;

public class TrackingManager:MonoBehaviour {

	/*
	 * A MonoBehavior to track using different systems.
	 */


	// https://github.com/googleanalytics/google-analytics-plugin-for-unity

	public GoogleAnalyticsV3 googleAnalyticsTracker;
	public bool useGoogleAnalytics;

	public string unityAnalyticsAppId;
	public bool useUnityAnalytics;

	private static TrackingManager instance;


	// ================================================================================================================
	// MAIN EVENT INTERFACE -------------------------------------------------------------------------------------------

	void Awake() {
		instance = this;
	}
	
	void Start() {
		// Google Analytics
		if (useGoogleAnalytics && googleAnalyticsTracker != null) {
			// Start session
			googleAnalyticsTracker.StartSession();
		}

		// Unity Analytics
		if (useUnityAnalytics) {
			// Start session
			UnityAnalytics.StartSDK(unityAnalyticsAppId);
			
			// Custom tracking: platform
			#if UNITY_EDITOR
			trackUACustomEventPlatform("editor");
			#elif UNITY_IPHONE
			trackUACustomEventPlatform("ios");
			#elif UNITY_ANDROID
			trackUACustomEventPlatform("android");
			#elif UNITY_STANDALONE_OSX
			trackUACustomEventPlatform("osx");
			#elif UNITY_STANDALONE_WIN
			trackUACustomEventPlatform("windows");
			#elif UNITY_WEBPLAYER
			trackUACustomEventPlatform("web");
			#elif UNITY_WII
			trackUACustomEventPlatform("wii");
			#elif UNITY_PS3
			trackUACustomEventPlatform("ps3");
			#elif UNITY_XBOX360
			trackUACustomEventPlatform("xbox360");
			#elif UNITY_FLASH
			trackUACustomEventPlatform("flash");
			#elif UNITY_BLACKBERRY
			trackUACustomEventPlatform("blackberry");
			#elif UNITY_WP8
			trackUACustomEventPlatform("windows-phone-8");
			#elif UNITY_METRO
			trackUACustomEventPlatform("windows-metro");
			#else
			trackUACustomEventPlatform("unknown-" + Application.platform);
			#endif
			
			// Custom tracking: user
			trackUACustomEvent("user", "os_language", Application.systemLanguage);
		}
	}

	void Update() {
	}

	void OnDestroy() {
		if (useGoogleAnalytics && googleAnalyticsTracker != null) {
			googleAnalyticsTracker.StopSession();
		}
	}

	/* 
	public void DispatchHits();
	 */

	private void trackUACustomEventPlatform(string platformId) {
		trackUACustomEvent("platform",
			"id", platformId, 
			"os", SystemInfo.operatingSystem,
			"device_model", SystemInfo.deviceModel,
			"device_name", SystemInfo.deviceName,
			"device_type", SystemInfo.deviceType,
			"memory", SystemInfo.systemMemorySize,
			"unity_runtime_version", Application.unityVersion,
			"is_mobile", Application.isMobilePlatform,
			"is_console", Application.isConsolePlatform,
			"internet", Application.internetReachability
			// TODO: SystemInfo.processorType
			// TODO: average fps, lowest fps
		);
	}

	private void trackUACustomEvent(string eventType, params object[] paramValues) {
		// Track a custom event using Unity Analytics
		//https://analytics.cloud.unity3d.com/docs
		// Max 10 params per custom event

		var parametersDict = new Dictionary<string, object>();
		for (var i = 0; i < paramValues.Length; i += 2) {
			parametersDict.Add(paramValues[i] as string, paramValues[i+1]);
		}
		
		UnityAnalytics.CustomEvent(eventType, parametersDict);
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
		if (useGoogleAnalytics && googleAnalyticsTracker != null) {
			googleAnalyticsTracker.LogScreen(screenId);

			// //Builder Hit with all App View parameters (all parameters required):
			//googleAnalytics.LogScreen(new AppViewHitBuilder() .SetScreenName("Main Menu"));
		}
		if (useUnityAnalytics) {
			trackUACustomEvent("navigation_screen", "id", screenId);
		}
	}

	// Events
	/*
    googleAnalytics.LogEvent("Achievement", "Unlocked", "Slay 10 dragons", 5);

    //Builder Hit with all Event parameters
    googleAnalytics.LogEvent(new EventHitBuilder()
        .SetEventCategory("Achievement")
        .SetEventAction("Unlocked")
        .SetEventLabel("Slay 10 dragons")
        .SetEventValue(5));

    //Builder Hit with minimum required Event parameters
    googleAnalytics.LogEvent(new EventHitBuilder()
        .SetEventCategory("Achievement")
        .SetEventAction("Unlocked"));
	 **/

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
		if (useGoogleAnalytics && googleAnalyticsTracker != null) {
			googleAnalyticsTracker.LogTransaction(productId, purchaseMethod, price, 0.0, 0.0, currency);
		}
		if (useUnityAnalytics) {
			UnityAnalytics.Transaction(purchaseMethod + "-" + productId, Convert.ToDecimal(price), currency);
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
