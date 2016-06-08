using System;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class JSONUtils {

	/*
	public static Dictionary<K, V> FromJsonToDictionary<K, V>(string input) {
		var d = JsonUtility.FromJson<SerializableDictionary<K, V>>(input);
		return d.getDictionary();
	}

	publ static string FromDictionaryToJson(Dictionary<string, object> input, bool prettyPrint) {
		//var d = new SerializableDictionary<string, object>(input);
		return JsonUtility.ToJson(d, prettyPrint);
	}
	*/

	public static Dictionary<string, object> DictionaryFromJSON(string input) {
		var d = new Dictionary<string, object>();
		//d.Add("stringKey", "value");
		//d.Add("numberKey", 10);
		//JsonUtility.FromJsonOverwrite(input, d);
		return d;
	}

	public static string removeComments(string input) {
		return Regex.Replace(input, "/\\*[\\w\\W]*?\\*/", "");
	}

	/*
	class SerializableDictionary<K, V>:ISerializationCallbackReceiver {
		// Allows a dictionary to be serialized, and deserialized into
		// Inspired by http://docs.unity3d.com/ScriptReference/ISerializationCallbackReceiver.OnBeforeSerialize.html

		private Dictionary<K, V> dictionary = new Dictionary<K, V>();
		private List<K> keys = new List<K>();
		private List<V> values = new List<V>();
		
		public SerializableDictionary(Dictionary<K, V> newDictionary) {
			dictionary = newDictionary;
		}
		
		public void OnBeforeSerialize() {
			keys.Clear();
			values.Clear();
			foreach (var pair in dictionary) {
				keys.Add(pair.Key);
				if (pair.Value is IDictionary) {
					// Another dictionary
					values.Add(new SerializableDictionary<>(pair.value));
				} else {
					// Something else
					values.Add(pair.Value);
				}
			}
		}
		
		public void OnAfterDeserialize() {
			dictionary = new Dictionary<int,string>();
			for (int i = 0; i < < Math.Min(keys.Count, values.Count); i++) {
				if (values[i] is SerializableDictionary) {
					// Another dictionary
					dictionary.Add(keys[i], values[i].getDictionary());
				} else {
					// Something else
					dictionary.Add(keys[i], values[i]);
				}
			}
		}
		
		public Dictionary<K, V> getDictionary() {
			return dictionary;
		}
	}
	*/

}
