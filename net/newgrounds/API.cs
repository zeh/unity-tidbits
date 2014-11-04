using System;
using UnityEngine;
using System.Collections;
using Newgrounds.Services;

namespace Newgrounds {

	public class API:MonoBehaviour {

		// Properties
		private static string _apiId;
		private static string _encryptionKey;

		private static bool _isConnected;

		private static GameObjectSurrogate _surrogate;

		// Passed user data
		private static string _connectionUserName;
		private static string _connectionUserId;
		private static string _connectionPublisherId;
		private static string _connectionUserpageFormat;
		private static string _connectionSessionId;
		private static string _connectionSaveGroupId;
		private static string _connectionSaveFileId;


		// ================================================================================================================
		// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

		public static void Connect(string __apiId, string __encryptionKey = null, string __version = "") {
			_apiId = __apiId;
			_encryptionKey = __encryptionKey;

			// This is an odd combination: the static class creates a GameObject that has an instance of the class.
			// The instance just forwards everything to the static class.
			GameObject newGameObject = new GameObject("NewgroundsAPISurrogate-" + (int)(Time.realtimeSinceStartup * 1000f));
			_surrogate = newGameObject.AddComponent<GameObjectSurrogate>();

			Debug.Log("Attempting to connect to newgrounds API");

			if (Application.isEditor) {
				// Running in editor: use fake data
				// REPLACE THIS WITH THE URL OF YOUR GAME WHEN TESTING! It needs the session id!
				setContainerURLStatic("http://uploads.ungrounded.net/alternate/999999/999999_alternate_9999.zip/?NewgroundsAPI_PublisherID=9&NewgroundsAPI_SandboxID=Abc999&NewgroundsAPI_SessionID=Abc999&NewgroundsAPI_UserName=john&NewgroundsAPI_UserID=999999&ng_username=john");
			} else {
				// Need the container URL first (for user parameters)
				Application.ExternalEval(
					"document.getElementById('unityPlayer').children[0].SendMessage('" + newGameObject.name + "', 'setContainerURL', document.location.toString());"
				);
			}
		}

		internal static void setContainerURLStatic(string __url) {
			// Now that the container is known, continue connecting
			// Example URL passed: http://uploads.ungrounded.net/alternate/999999/999999_alternate_9999.zip/?NewgroundsAPI_PublisherID=9&NewgroundsAPI_SandboxID=Abc999&NewgroundsAPI_SessionID=Abc999&NewgroundsAPI_UserName=john&NewgroundsAPI_UserID=999999&ng_username=john

			Debug.Log("Container URL is " + __url);

			// Dirty querystring parsing (no access to C#'s System.Web)
			string[] pairs = __url.Substring(__url.IndexOf("?") + 1).Split('&');
			string[] pair;
			for (int i = 0; i < pairs.Length; i++) {
				pair = pairs[i].Split('=');
				switch (pair[0]) {
					case "NewgroundsAPI_UserName":
						_connectionUserName = pair[1];
						break;
					case "NewgroundsAPI_UserID":
						_connectionUserId = pair[1];
						break;
					case "NewgroundsAPI_PublisherID":
						_connectionPublisherId = pair[1];
						break;
					case "NewgroundsAPI_UserpageFormat":
						_connectionUserpageFormat = pair[1];
						break;
					case "NewgroundsAPI_SessionID":
						_connectionSessionId = pair[1];
						break;
					case "NewgroundsAPI_SaveGroupID":
						_connectionSaveGroupId = pair[1];
						break;
					case "NewgroundsAPI_SaveFileID":
						_connectionSaveFileId = pair[1];
						break;
				}
			}

			Debug.Log("connectionUserName       => " + _connectionUserName); // 'zehfernando'
			Debug.Log("connectionUserId         => " + _connectionUserId); // int
			Debug.Log("connectionPublisherId    => " + _connectionPublisherId); // 1
			Debug.Log("connectionUserpageFormat => " + _connectionUserpageFormat); // Empty
			Debug.Log("connectionSessionId      => " + _connectionSessionId); // Long hash
			Debug.Log("connectionSaveGroupId    => " + _connectionSaveGroupId); // Empty
			Debug.Log("connectionSaveFileId     => " + _connectionSaveFileId); // Empty

			_isConnected = true;
		}

		public static void UnlockMedal(string __medalName) {

		}

		public static void PostScore(string __scoreBoardName, int __numericScore, string __tag = null) {
			PostScoreService service = new PostScoreService(eResult);
			service.setData(__scoreBoardName, __numericScore, __tag);
			service.execute();
		}

		private static void eResult(BasicService __service = null) {
			if (__service.resultSuccess) {
				Debug.Log("==============> success posting " + __service);
			} else {
				Debug.Log("==============> error posting: " + __service.resultErrorMessage);
			}
		}


		// ================================================================================================================
		// ACCESSOR INTERFACE ---------------------------------------------------------------------------------------------

		public static string apiId {
			get { return _apiId; }
		}

		public static string encryptionKey {
			get { return _encryptionKey; }
		}

		public static bool isConnected {
			get { return _isConnected; }
		}

		public static GameObjectSurrogate surrogate {
			get { return _surrogate; }
		}

		public static string connectionUserName {
			get { return _connectionUserName; }
		}

		public static string connectionUserId {
			get { return _connectionUserId; }
		}

		public static string connectionPublisherId {
			get { return _connectionPublisherId; }
		}

		public static string connectionUserpageFormat {
			get { return _connectionUserpageFormat; }
		}

		public static string connectionSessionId {
			get { return _connectionSessionId; }
		}

		public static string connectionSaveGroupId {
			get { return _connectionSaveGroupId; }
		}

		public static string connectionSaveFileId {
			get { return _connectionSaveFileId; }
		}

		/*
Return Object
command_id = getMedals
medals - An array of medal objects (if the game uses them) with the following properties:
medal_id - The numeric id of the medal
medal_name - The name of the medal
medal_value - The point value of the medal
medal_difficulty - 1=easy, 2=moderate, 3=challenging, 4=difficult, 5=brutal
medal_unlocked - true/false (if publisher_id and user_id were passed)
medal_icon - The URL of the icon for this medal
secret - 1 if this is a secret medal
medal_description - The description of this medal
success = 1
{"command_id":"getMedals","medals":
[
{"medal_id":-32827,"medal_name":"Completed Level 1 (Ambush)","medal_value":0,"medal_difficulty":1,"medal_unlocked":false,"medal_icon":"http:\/\/apifiles.ngfiles.com\/medals\/36000\/36957\/32827_completedlevel1ambush.jpg","secret":0,"medal_description":"Completed Level 1 (Ambush)"}
],"success":1}

UnityEngine.Debug:Log(Object)
Debug.Log(Object) (at Assets/Scripts/Libraries/logging/D.cs:25)
<doPostWithYield>c__Iterator9:MoveNext() (at Assets/Scripts/Libraries/net/newgrounds/NewgroundsAPI.cs:186)

	 */
	}
}