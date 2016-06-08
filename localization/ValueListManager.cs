using UnityEngine;
using System.IO;
using System.Collections.Generic;

#if UNITY_EDITOR
[ExecuteInEditMode()]
#endif

public class ValueListManager:MonoBehaviour {

	// Static properties
	private static Dictionary<string, ValueListManager> instances;

	// Parameters
	public string key;

	public ValueListWithQualifiers[] valueListsWithQualifiers;

	// Properties
	private List<ValueList> valueLists;                             // Valid value lists, already filtered and ordered

	// TODO: add tester interface. You can enter an ID, and it shows the selected value, and where it's coming from. Also add a simulator?
	// TODO: add a "comparer", so two language xmls can be compared and the missing ids can be listed.
	// TODO: create a better inspector? http://answers.unity3d.com/questions/26207/how-can-i-recreate-the-array-inspector-element-for.html

	// ================================================================================================================
	// MAIN EVENT INTERFACE -------------------------------------------------------------------------------------------

	void Awake() {
		// Load all value lists, creating a pre-filtered list
		valueLists = new List<ValueList>();

		for (var i = 0; i < valueListsWithQualifiers.Length; i++) {
			// Filter
			if (doQualifiersApply(valueListsWithQualifiers[i])) {
				// Is valid, load
				var valueList = new ValueList("values_" + key + "_" + i);
				valueList.SetFromJSON(File.ReadAllText(valueListsWithQualifiers[i].fileName));
				valueLists.Add(valueList);
			}

		}

		// Add to the list of instances that can be retrieved later
		ValueListManager.addInstance(this);

		Debug.Log("Value lists read; current language is " + ValueListManager.GetInstance().GetString("generic.language"));
	}


	// ================================================================================================================
	// STATIC INTERFACE -----------------------------------------------------------------------------------------------

	static ValueListManager() {
		instances = new Dictionary<string, ValueListManager>();
	}

	private static void addInstance(ValueListManager instance) {
		if (instances.ContainsKey(instance.key)) instances.Remove(instance.key);
		instances.Add(instance.key, instance);
	}

	public static ValueListManager GetInstance(string key = "") {
		if (instances.ContainsKey(key)) return instances[key];
		Debug.LogError("Error! Tried reading a ValueListManager with key [" + key + "] that doesn't exist.");
		return null;
	}


	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	public string GetString(string keyPath) {
		return GetValue<string>(keyPath);
	}

	public T GetValue<T>(string keyPath) {
		List<ValueList> lists = getValidValueLists();
		if (lists != null) {
			for (int i = 0; i < lists.Count; i++) {
				if (lists[i].HasKey(keyPath)) return lists[i].GetValue<T>(keyPath);
			}
		}

		Debug.LogWarning("Trying reading object of value [" + keyPath + "] that was not found on value lists.");
		return default(T);
	}


	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

	private ValueList getFirstValidValueList() {
		for (var i = 0; i < valueListsWithQualifiers.Length; i++) {
			if (doQualifiersApply(valueListsWithQualifiers[i])) {
				return valueLists[i];
			}
		}

		return null;
	}

	private List<ValueList> getValidValueLists() {
		// Create a list of valueLists that qualify given their selectors

		// TODO: filter by other qualifiers that are not platform- or device-specific?
		return valueLists;
	}

	private bool doQualifiersApply(ValueListWithQualifiers valueListWithQualifiers) {
		// Checks whether a value list's qualifiers are valid

		if (!valueListWithQualifiers.enabled) return false;

		bool isValid = true;

		// Check language
		if (isValid && valueListWithQualifiers.languageFilterEnabled) {
			isValid = valueListWithQualifiers.language == Application.systemLanguage;
		}

		// Check OS
		if (isValid && valueListWithQualifiers.platformFilterEnabled) {
			isValid = valueListWithQualifiers.platform == Application.platform;
		}

		return isValid;
	}

}

[System.Serializable]
public class ValueListWithQualifiers {

	public string fileName;
	public bool enabled;

	public bool languageFilterEnabled;
	public SystemLanguage language;

	public bool platformFilterEnabled;
	public RuntimePlatform platform;

	/*
		* http://developer.android.com/guide/topics/resources/providing-resources.html
		* Qualifiers:
		* . Country (mcc, mnc)
		* . Language/locale (en, en-rUS) ("r" is to differentiate the region portion)
		* . Width/height/smallest width
		* . Layout direction
		* . Screen size (small, normal, large, xlarge)
		* . Aspect ratio (long, not long)
		* . Round or not
		* . Screen orientation
		* . UI mode (car, desktop, appliance, etc)
		* . Screen pixel density (ldpi, mdpi, hdpi, xhdpi, xxhdpi, xxxhdpi, nodpi, tvdpi)
		* . Touchscreen type (notouch, finger)
		* . Keyboard availability (keysexposed, keyshidden, keyssoft)
		*/

}