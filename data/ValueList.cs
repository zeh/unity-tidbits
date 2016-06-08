using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

class ValueList {
	
	/* A list of strings and other values, for localization */

	/*
	{
		"id1" : "value1",
		"group" : {
			"id2" : "value2"
		}
	}

	Use:
	getString("id1"); // "value1"
	getString("group.id2"); // "value2"
	
	TODO:
	 * Proper processed string
	 * Insert from JSON
	 * Other native types: bool, number, etc
	 * Other Unity types: prefab, texture, audio clip
	 * Actual language filtering

	* For proper language later:
	Values.json
	{
		id: "en",
		name: "English",
		values: {
			name: "App name"
		}
	}

	*/

	// Constants
	private static string ID_HYERARCHY_SEPARATOR = ".";

	// Default values
	//private static string VALUE_STRING_DEFAULT = "[null]";

	// Static properties
	private static Dictionary<string, ValueList> instances;

	// Properties
	private string _key;									// Instance name
	private ValueGroup root;


	// ================================================================================================================
	// STATIC ---------------------------------------------------------------------------------------------------------

	static ValueList() {
		instances = new Dictionary<string, ValueList>();
	}
	

	// ================================================================================================================
	// CONSTRUCTOR ----------------------------------------------------------------------------------------------------

	public ValueList(string key = "") {
		_key = key;

		ValueList.addInstance(this);
		root = new ValueGroup();
	}


	// ================================================================================================================
	// STATIC INTERFACE -----------------------------------------------------------------------------------------------

	private static void addInstance(ValueList instance) {
		if (instances.ContainsKey(instance.key)) instances.Remove(instance.key);
		instances.Add(instance.key, instance);
	}

	public static ValueList getInstance(string key = "") {
		if (instances.ContainsKey(key)) return instances[key];

		// Doesn't exist, create a new one and return it
		return new ValueList(key);
	}


	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	/*
	public function setValues(__object:Object):void {
		// Set the values; __object must be a JSON-like object
		values = __object;
		// TODO: read string data replacing unix/windows line feed?
	}
	 * */

	public string GetString(string keyPath) {
		return getValueInternal<string>(keyPath);
	}

	public T GetValue<T>(string keyPath) {
		return getValueInternal<T>(keyPath);
	}

	public void SetString(string keyPath, string value) {
		setValueInternal(keyPath, new ValueItemString(value));
	}

	public bool HasKey(string keyPath) {
		return getValueInternal<object>(keyPath) != null;
	}

	public void SetFromJSON(string jsonSource) {
		// Parses a JSON file and sets the value data
		// { "key" : "value", "group" : { "key": "value" }}
		SetFromDictionary(JSON.parseAsDictionary(jsonSource, JSON.FLAG_REMOVE_COMMENTS));
	}

	public void SetFromDictionary(IDictionary dictionary, string parentRoot = "") {
		// Set values from a dictionary, using a parent root if any
		string thisPath;
		foreach (DictionaryEntry entry in dictionary) {
			thisPath = (parentRoot.Length > 0 ? parentRoot + ID_HYERARCHY_SEPARATOR : "") + entry.Key;
			if (entry.Value is string) {
				// Normal string
				SetString(thisPath, entry.Value as string);
			} else if (entry.Value is IDictionary) {
				// Group
				SetFromDictionary(entry.Value as Dictionary<string, object>, thisPath);
			} else {
				Debug.LogError("Error! Cannot add from dictionary with object [" + entry.Value + "]");
			}
		}
	}

	/*
	public string getProcessedString(string text) {
		return getProcessedStringInternal(text);
	}
	*/


	// ================================================================================================================
	// ACCESSOR INTERFACE ---------------------------------------------------------------------------------------------

	public string key {
		get { return _key; }
	}


	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

	/*
	private string getProcessedStringInternal(string text) {
		// Returns a string with processed codes
		// For example "this is an ${examples/example}!" returns "this is an EXAMPLE!" (where the value of examples.example in strings.json is "EXAMPLE")

		var newString = "";

		var codes:RegExp = /\$\{(.+?)\}/ig;
		var result:Object = codes.exec(__string);

		var lastIndex:Number = 0;
		var newIndex:Number;

		while (Boolean(result)) {
			newIndex = result["index"];

			// What came before the tag
			newString += __string.substring(lastIndex, newIndex);

			// The tag tex
			newString += getString(result[1]);

			lastIndex = codes.lastIndex;

			result = codes.exec(__string);
		}

		// End text after last tag
		newString += __string.substring(lastIndex, __string.length);

		return newString;
	}
	 * */

	private T getValueInternal<T>(string keyPath) {
		// Get the full path to the value name
		var ids = new List<string>(keyPath.Split(ID_HYERARCHY_SEPARATOR.ToCharArray()));
		ValueGroup group;
		string itemKey;
		if (ids.Count > 0) {
			// Remove last (because it's the item)
			itemKey = ids[ids.Count - 1];
			ids.RemoveAt(ids.Count - 1);
			group = getGroupInternal(ids);
		} else {
			Debug.LogWarning("ValueList :: Value path [" + keyPath + "] is empty!");
			return default(T);
		}

		if (group == null) {
			return default(T);
		} else if (group.hasItem(itemKey)) {
			return (T)(group.getItem(itemKey).getValue());
		} else {
			Debug.LogWarning("ValueList :: Value path key [" + keyPath + "] doesn't exist!");
			return default(T);
		}
	}

	private void setValueInternal(string keyPath, ValueItem valueItem) {
		var ids = new List<string>(keyPath.Split(ID_HYERARCHY_SEPARATOR.ToCharArray()));
		ValueGroup group;
		string itemKey;
		if (ids.Count > 0) {
			// Remove last (because it's the item)
			itemKey = ids[ids.Count - 1];
			ids.RemoveAt(ids.Count - 1);
			group = getGroupInternal(ids, true);
			group.addItem(itemKey, valueItem);
		} else {
			Debug.LogWarning("ValueList :: Value path [" + keyPath + "] is empty!");
		}
	}

	private ValueGroup getGroupInternal(List<String> keyPath, bool canCreate = false) {
		var currentGroup = root;

		for (var i = 0; i < keyPath.Count; i++) {
			if (currentGroup.hasGroup(keyPath[i])) {
				// Found
				currentGroup = currentGroup.getGroup(keyPath[i]);
			} else if (canCreate) {
				// Doesn't exist, but can create
				currentGroup.addGroup(keyPath[i], new ValueGroup());
				currentGroup = currentGroup.getGroup(keyPath[i]);
			} else {
				// Doesn't exist, fail
				Debug.LogWarning("ValueList :: Value path [" + string.Join(ID_HYERARCHY_SEPARATOR, keyPath.ToArray()) + "] doesn't exist!");
				return null;
			}
		}

		return currentGroup;
	}


	// ================================================================================================================
	// AUXILIARY CLASSES ----------------------------------------------------------------------------------------------

	class ValueGroup {
		private Dictionary<string, ValueGroup> groups;
		private Dictionary<string, ValueItem> items;

		public ValueGroup() {
			groups = new Dictionary<string, ValueGroup>();
			items = new Dictionary<string, ValueItem>();
		}

		public bool hasItem(string key) {
			return items.ContainsKey(key);
		}

		public void addItem(string key, ValueItem item) {
			items[key] = item;
		}

		public ValueItem getItem(string key) {
			return items[key];
		}

		public bool hasGroup(string key) {
			return groups.ContainsKey(key);
		}

		public void addGroup(string key, ValueGroup group) {
			groups[key] = group;
		}

		public ValueGroup getGroup(string key) {
			return groups[key];
		}
	}

	abstract class ValueItem {
		private object value;

		public ValueItem(object value) {
			this.value = value;
		}

		public object getValue() {
			return value;
		}
	}

	class ValueItemString:ValueItem {
		public ValueItemString(string value):base(value) {
		}

		public string getValue() {
			return (string)getValue();
		}
	}

}
