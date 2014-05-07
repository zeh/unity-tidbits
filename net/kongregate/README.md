Kongregate API
==============

A pure code implementation of the Kongregate statistics API for Unity as a single C# file. The statistics API is used for counting items (such as level completions, enemies killed, high score, etc) on Kongregate games. Games implementing the API also [get a bigger share of ad revenue](http://www.kongregate.com/pages/help#ad-revenue-share-q-3) from the website, where eligible.

While the Kongregate API is relatively simple to implement, most of the implementation examples out there use JavaScript or require you to manually create GameObjects. This implementation minimizes the amount of work needed for setup and use.

Usage
-----

1. Copy the **KongregateAPI.cs** file to your Unity project's "Scripts" folder
2. Anywhere in your code (e.g. your `Main` game class), create an instance of the KongregateAPI:
<pre>KongregateAPI kongregate = KongregateAPI.Create();</pre>
3. Whenever you need to submit statistics, do:
<pre>kongregate.SubmitStats(name, value);</pre>
For example:
<pre>kongregate.SubmitStats("high-score", 1000);
kongregate.SubmitStats("tanks-destroyed", 1);</pre>


Full Interface
--------------

 * `Create()` (Static): creates a `KongregateAPI` instance and returns it (automatically creating a `GameObject` for itself so it can receive events).
 * `SubmitStats(string name, int value)`: submit statistics to the Kongregate API.
 * bool `isConnected`: returns whether the user is properly connected to the Kongregate site API. Read-only.
 * int `userId`: id of the current user. Read-only.
 * string `userName`: name of the current user. Read-only.
 * string `gameAuthToken`: game authorization token. Read-only.

Debugging
---------

In general, you don't need to do anything special for your code to work on Kongregate. Just implement the above code and you're good.

Notice, however, that:

 * This won't work when testing your game inside Unity; the Kongregate API depends on JavaScript located on their website.
 * Testing on the pre-publish screen won't work either (the `kongregate` object is not instantiated). The API only works after your game is properly published.
 * If you want to make sure your game is working with the API, go to its published page, add `?debug_level=4` to the URL of the game, and reload the page. This makes the JavaScript console (F12 on Chrome) output what the API is receiving from your game.

Caveats
-------

 * This implementation doesn't support [callbacks](http://developers.kongregate.com/docs/api-overview/unity-api).

More information
----------------

 * [Kongregate Statistics API](http://www.kongregate.com/developer_center/docs/en/statistics-api)
