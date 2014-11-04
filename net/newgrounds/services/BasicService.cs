using System;
using UnityEngine;
using SimpleJSON;
using System.Collections;

namespace Newgrounds.Services {

	public class BasicService {

		// Types
		public delegate void ServiceCallback(BasicService __service);

		// Constants
		private const string API_URL = "http://www.ngads.com/gateway_v2.php";
		private const string RADIX_SET = "/g8236klvBQ#&|;Zb*7CEA59%s`Oue1wziFp$rDVY@TKxUPWytSaGHJ>dmoMR^<0~4qNLhc(I+fjn)X";

		// Instances
		private ServiceCallback callback;

		private WWWForm formData;
		private WWW serviceRequest;

		// Request data
		protected string commandId;
		protected bool isSecure;

		// Result data
		private bool _resultSuccess;
		private string _resultErrorMessage;
		private int _resultErrorCode;

		// ================================================================================================================
		// CONSTRUCTOR ----------------------------------------------------------------------------------------------------

		public BasicService(ServiceCallback __callback = null) {
			callback = __callback;

			setDefaultData();
		}


		// ================================================================================================================
		// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

		public void execute() {
			// Creates the service

			Debug.Log("executing service");

			IDictionaryEnumerator myEnumerator;

			formData = new WWWForm();

			Hashtable formDataHash = new Hashtable();

			// The command
			if (isSecure) {
				string seed = StringUtils.getRandomAlphanumericString(13);
				string seedMD5 = StringUtils.MD5(seed);

				// Everything in an ecrypted field
				Hashtable formDataSecureHash = new Hashtable();

				// Generate the data first
				addBasicFields(formDataSecureHash);
				formDataSecureHash.Add("seed", seed);

				JSONObject j = new JSONObject(JSONObject.Type.OBJECT);
				myEnumerator = formDataSecureHash.GetEnumerator();
				while (myEnumerator.MoveNext()) {
					if (myEnumerator.Value is int) {
						j.AddField((string)myEnumerator.Key, (int)myEnumerator.Value);
//						Debug.Log("   SECURE adding int => " + myEnumerator.Key + " = " + myEnumerator.Value);
					} else {
						j.AddField((string)myEnumerator.Key, (string)myEnumerator.Value);
//						Debug.Log("   SECURE adding string => " + myEnumerator.Key + " = " + myEnumerator.Value);
					}
				}

				string secureParameter = j.Print();
				string encryptionKey = API.encryptionKey;

				// For some reason, the request never works the string length is a multiple of 6 (when the seed has a length of 1 or 13 or ...)
				// The documentation is conflicting in this regard and it's hard to know the cause (probably number padding and alignment)
				// So just add a space in that case
				// Sizes that don't work: 174, 180
				// Sizes that work: 175, 176, 177, 178
				if (secureParameter.Length % 3 == 0) secureParameter += " ";

				Debug.Log("Secure param => (" + secureParameter.Length + ") " + secureParameter);

				// Encode all data with RC4
				string secureParameterHex = seedMD5 + StringUtils.convertBytesToString(StringUtils.encodeRC4(secureParameter, encryptionKey));

				// Testingh

				// Convert to baseN
				int pos = 0;
				string secureParameterBaseN = "";
				string hexString;
				int hexNumber;
				while (pos < secureParameterHex.Length) {
					hexString = secureParameterHex.Substring(pos, Math.Min(pos + 6, secureParameterHex.Length)-pos);
					hexNumber = Convert.ToInt32(hexString, 16);
					secureParameterBaseN += StringUtils.convertIntToCustomBase(hexNumber, RADIX_SET, 4);
					//secureParameterBaseN += StringUtils.convertIntToCustomBase(hexNumber, RADIX_SET, hexString.Length / 3 * 2);
					pos += 6;
				}

				// Add tail length
				secureParameterBaseN = (secureParameterBaseN.Length % 6) + secureParameterBaseN;

				// Add real fields
				formDataHash.Add("secure", secureParameterBaseN);
				formDataHash.Add("command_id", "securePacket");
			} else {
				// Everything in main request
				addBasicFields(formDataHash);
			}

			// The tracker ID of your game
			formDataHash.Add("tracker_id", API.apiId);

			// Generate form data
			myEnumerator = formDataHash.GetEnumerator();
			while (myEnumerator.MoveNext()) {
				if (myEnumerator.Value is int) {
					formData.AddField((string)myEnumerator.Key, (int)myEnumerator.Value);
//					Debug.Log("   adding int => " + myEnumerator.Key + " = " + myEnumerator.Value);
				} else {
					formData.AddField((string)myEnumerator.Key, (string)myEnumerator.Value);
//					Debug.Log("   adding string => " + myEnumerator.Key + " = " + myEnumerator.Value);
				}
			}

			// Send
			serviceRequest = new WWW(API_URL, formData);

			API.surrogate.doPost(serviceRequest, onDone);
		}


		// ================================================================================================================
		// ACCESSOR INTERFACE ---------------------------------------------------------------------------------------------

		public bool resultSuccess {
			get { return _resultSuccess; }
		}

		public string resultErrorMessage {
			get { return _resultErrorMessage; }
		}

		public int resultErrorCode {
			get { return _resultErrorCode; }
		}


		// ================================================================================================================
		// EXTENDED INTERFACE ---------------------------------------------------------------------------------------------

		protected virtual void setDefaultData() {
			// Extend

			commandId = "";
			isSecure = false;
		}

		protected virtual void addCustomFields(Hashtable __hash) {
			// Extend
		}

		protected virtual void parseSuccessResult(JSONNode __result) {
			// Extend
		}


		// ================================================================================================================
		// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

		private void addBasicFields(Hashtable __hash) {
			__hash.Add("command_id", commandId);

			if (API.connectionPublisherId != null) __hash.Add("publisher_id", API.connectionPublisherId);
			if (API.connectionSessionId != null) __hash.Add("session_id", API.connectionSessionId);
			//if (API.connectionUserId != null) __hash.AddField("user_id", API.connectionSessionId);

			addCustomFields(__hash);
		}

		private void onDone() {
			JSONNode result = JSON.Parse(serviceRequest.text);
			// {"error_msg":"You must be logged in or provide an e-mail address to use postScore().","error_code":7,"success":0,"command_id":"postScore"}

			if (result["success"].AsInt == 1) {
				// Success!
				_resultSuccess = true;

				parseSuccessResult(result);
			} else {
				// Error!
				_resultSuccess = false;
				_resultErrorMessage = result["error_msg"];
				_resultErrorCode = result["error_code"].AsInt;
			}

			if (callback != null) callback(this);
		}
	}
}
