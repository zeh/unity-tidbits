using System;
using UnityEngine;
using System.Collections;
using SimpleJSON;

namespace Newgrounds.Services {

	public class PostScoreService:BasicService {

		// Data
		private string scoreBoardName;
		private int numericScore;
		private string tag;


		// ================================================================================================================
		// CONSTRUCTOR ----------------------------------------------------------------------------------------------------

		public PostScoreService(ServiceCallback __callback = null) : base(__callback) {

		}


		// ================================================================================================================
		// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

		public void setData(string __scoreBoardName, int __numericScore, string __tag = null) {
			scoreBoardName = __scoreBoardName;
			numericScore = __numericScore;
			tag = __tag;
		}


		// ================================================================================================================
		// EXTENDED INTERFACE ---------------------------------------------------------------------------------------------

		protected override void setDefaultData() {
			// Extend
			commandId = "postScore";
			isSecure = true;
		}

		protected override void addCustomFields(Hashtable __hash) {
			__hash.Add("board", scoreBoardName);
			__hash.Add("value", numericScore);
			if (tag != null) __hash.Add("tag", tag);
		}

		protected override void parseSuccessResult(JSONNode __result) {
			// Extend

			// ==> {"command_id":"postScore","board":-4970,"tag":"","value":100,"success":1}
		}
	}
}
