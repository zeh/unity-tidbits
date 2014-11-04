using System;
using UnityEngine;
using System.Collections;
using SimpleJSON;

namespace Newgrounds {

	public class GameObjectSurrogate:MonoBehaviour {

		// A script to serve as a GameObject surrogate/proxy

		// ================================================================================================================
		// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

		void Awake() {
			// Actual initialization
			// Instructs the game object to survive level changes
			DontDestroyOnLoad(this);
		}

		public void doPost(WWW __wwww, Action __doneCallback) {
			StartCoroutine(doPostWithYield(__wwww, __doneCallback));
		}

		public void setContainerURL(string __url) {
			// Receives the current URL with user data and passes everything to the static class
			// This is a private method, but JavaScript don't care; it is still called by SendMessage()
			API.setContainerURLStatic(__url);
		}


		// ================================================================================================================
		// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

		private IEnumerator doPostWithYield(WWW _www, Action __doneCallback) {
			yield return _www;

			Debug.Log("Service sent; response: " + _www.text);

			//JSONNode medals = JSON.Parse(_w.text);

			//Debug.Log("There are " + medals["medals"].Count + " medals loaded");
			//Debug.Log("The name of the first medal is: [" + medals["medals"][0]["medal_name"] + "]");

			if (__doneCallback != null) __doneCallback();
		}
	}
}