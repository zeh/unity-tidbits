using System;
using UnityEngine;

namespace Newgrounds.Services {

	public class UnlockMedalService:BasicService {

		// ================================================================================================================
		// CONSTRUCTOR ----------------------------------------------------------------------------------------------------

		public UnlockMedalService(ServiceCallback __callback = null) : base(__callback) {

		}

		// ================================================================================================================
		// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

		// ================================================================================================================
		// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

		/*
	protected override WWW createService() {
		WWWForm form = new WWWForm();

		// The command
		form.AddField("command_id", "getMedals");

		// The tracker ID of your game
		form.AddField("tracker_id", apiId);

		// The ID of the publisher hosting the game (optional, used to mark unlocked medals)
		if (connectionPublisherId != null) form.AddField("publisher_id", connectionPublisherId);

		// The user ID of the current player (optional, used to mark unlocked medals)
		if (connectionUserId != null) form.AddField("user_id", connectionUserId);

		// Send
		WWW w = new WWW(API_URL, form);

		return w;
	}
		 * */

		/*
		 Posts a high score to a score board. This command is sent as a secure packet (see securePacket command)

	POST Values
	command_id = securePacket
	tracker_id - The tracker ID of your game
	secure - An encrypted object (see Encryption Process) containing the following properties:
	command_id = unlockMedal
	publisher_id - The ID of the publisher hosting the game
	session_id - The current player session ID
	medal_id - The ID of the medal

	Return Object
	command_id = unlockMedal
	medal_id - The ID of the medal
	medal_name - The name of the medal
	medal_value - The point value of the medal
	medal_difficulty - 1=Easy, 2=Moderate, 3=Challenging, 4=Difficult, 5=Brutal
	success = 1
		 **/
	}
}
/*
 * GetMedals(() => medalsDone());

		public static void GetMedals(Action __callback) {
			// http://docs.unity3d.com/Documentation/ScriptReference/WWWForm.html

			WWWForm form = new WWWForm();

			// The command
			form.AddField("command_id", "getMedals");

			// The tracker ID of your game
			form.AddField("tracker_id", _apiId);

			// The ID of the publisher hosting the game (optional, used to mark unlocked medals)
			if (_connectionPublisherId != null) form.AddField("publisher_id", _connectionPublisherId);

			// The user ID of the current player (optional, used to mark unlocked medals)
			if (_connectionUserId != null) form.AddField("user_id", _connectionUserId);

			// Send
			WWW w = new WWW(API_URL, form);

			Debug.Log("getting medals from " + API_URL + ", form = " + form);
			_surrogate.doPost(w, __callback);
		}*/