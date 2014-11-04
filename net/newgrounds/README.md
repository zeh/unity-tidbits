Newgrounds API
==============

An *initial* implementation of the Newgrounds RESTful API in C# for Unity with no JavaScript bridges. It tries to mimic the syntax of the [AS3 version of the API](http://www.newgrounds.com/wiki/creator-resources/flash-api/connecting-to-the-api), although only a handful of methods are available.

This is currently used in my [Escape Drill](http://www.newgrounds.com/portal/view/638876) game for Ludum Dare #29, but missing a lot of features.

Usage
-----

1. Copy the contents of the `newgrounds` folder to your Unity project's "Scripts" folder
2. Anywhere in your code (e.g. your `Main` game class), connect to the API:
<pre>Newgrounds.API.Connect(apiId, encryptionKey);</pre>
3. Whenever you need to submit statistics, do:
<pre>Newgrounds.API.PostScore(name, value);</pre>
For example:
<pre>Newgrounds.API.PostScore("High Score (level 1)", 1000);
Newgrounds.API.PostScore("Tanks Destroyed", 10);</pre>


Full Interface
--------------

 * `Newgrounds.API.Connect(string apiId, string encriptionKey)` (Static): Connects to the API. You need to do this before anything is posted. You can find the API id and encryption key values in the [API Tools section of your project](http://www.newgrounds.com/projects).
 * `Newgrounds.API.PostScore(string name, int value)` (Static): submit a score to the Newgrounds API. The `name` is not an id, but rather how the score is represented in the website.
 * string `connectionUserName`: current user name. Read-only.
 * int `connectionUserId`: id of the current user. Read-only.
 * string `connectionSessionId`: a long hash with the id of the current session. Read-only.

Debugging
---------

The API will not work inside the editor by default. This is because it needs some kind of session ID that is passed by the website when the game is ran. However, the API does detect when you're running inside the editor and attempt to use a placeholder URL. To replace this placeholder URL with a real URL, go to your game's page, find the URL of the iframe used to host it, and replace the `setContainerURLStatic()` call in line 45 of API.cs with this url. It will look like this:

    http://uploads.ungrounded.net/alternate/999999/999999_alternate_9999.zip/?NewgroundsAPI_PublisherID=9&NewgroundsAPI_SandboxID=Abc999&NewgroundsAPI_SessionID=Abc999&NewgroundsAPI_UserName=john&NewgroundsAPI_UserID=999999&ng_username=john

Caveats
-------

 * A lot is missing. Actually it only posts high scores right now.

More information
----------------

 * [Newgrounds gateway API](http://www.newgrounds.com/wiki/creator-resources/newgrounds-apis/developer-gateway)
 * [Newgrounds AS3 API](http://www.newgrounds.com/wiki/creator-resources/flash-api/connecting-to-the-api)
