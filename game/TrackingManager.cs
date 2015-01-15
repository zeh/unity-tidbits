using UnityEngine;
using System.Collections.Generic;

public class TrackingManager:MonoBehaviour {

	/*
	 * A MonoBehavior to track using different systems.
	 */


	// https://github.com/googleanalytics/google-analytics-plugin-for-unity

	public GoogleAnalyticsV3 googleAnalyticsTracker;
	public bool useGoogleAnalytics;

	public bool useGameAnalytics;

	private static TrackingManager instance;


	// ================================================================================================================
	// MAIN EVENT INTERFACE -------------------------------------------------------------------------------------------

	void Awake() {
		instance = this;
	}
	
	void Start() {
		if (useGoogleAnalytics && googleAnalyticsTracker != null) {
			googleAnalyticsTracker.StartSession();
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

	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	public static TrackingManager getInstance() {
		return instance;
	}

	public void trackScreen(string screenId) {
		if (useGameAnalytics) {
			GA.API.Design.NewEvent("navigation:screen:" + screenId);
		}
		if (useGoogleAnalytics && googleAnalyticsTracker != null) {
			googleAnalyticsTracker.LogScreen(screenId);

			// //Builder Hit with all App View parameters (all parameters required):
			//googleAnalytics.LogScreen(new AppViewHitBuilder() .SetScreenName("Main Menu"));
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
