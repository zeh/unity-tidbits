using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class GlobalConfigGO:MonoBehaviour {

	// Serializable properties
	[SerializeField]
	private string[] entryNames;

	[SerializeField]
	private int[] entryTypes;

	[SerializeField]
	private string[] entryValuesString;

	[SerializeField]
	private bool[] entryValuesBool;

	// Properties
	private List<ConfigEntry> entries = new List<ConfigEntry>();

	private bool hasStarted;
	private static GlobalConfigGO instance;


	// ================================================================================================================
	// MAIN EVENT INTERFACE -------------------------------------------------------------------------------------------

	void Awake() {
		instance = this;
		entries = new List<ConfigEntry>();
		hasStarted = true;
		load();
	}

	void Update() {
		if (Application.isEditor && !Application.isPlaying) {
			load();
		}
	}


	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	public static GlobalConfigGO getInstance() {
		return instance;
	}

	public void addEntry(ConfigEntry entry) {
		entries.Add(entry);
	}

	public ConfigEntry getEntryAt(int index) {
		return entries[index];
	}

	public int getNumEntries() {
		return entries.Count;
	}

	public void load() {
		//Debug.Log("loading serialized data, it has " + entryNames.Length + " entries");
		// Load from serialized properties
		entries.Clear();
		ConfigEntry entry;
		for (int i = 0; i < entryNames.Length; i++) {
			entry = new ConfigEntry();
			entry.name = entryNames[i];
			entry.type = (ConfigEntry.Types)entryTypes[i];
			entry.valueString = entryValuesString[i];
			entry.valueBool = entryValuesBool[i];
			entries.Add(entry);
		}
	}

	public void save() {
		// Save to serialized properties
		if (hasStarted) {
			entryNames = new string[entries.Count];
			entryTypes = new int[entries.Count];
			entryValuesString = new string[entries.Count];
			entryValuesBool = new bool[entries.Count];
			for (int i = 0; i < entries.Count; i++) {
				entryNames[i] = entries[i].name;
				entryTypes[i] = (int)entries[i].type;
				entryValuesString[i] = entries[i].valueString;
				entryValuesBool[i] = entries[i].valueBool;
			}
			//Debug.Log("saving all serialized data, it has " + entries.Count + " entries; " + entryNames.Length + " recorded");
		}
	}

	public string getString(string key, string defaultValue = "") {
		foreach (var entry in entries) {
			if (entry.name == key) return entry.valueString;
		}
		return defaultValue;
	}

	public bool getBool(string key, bool defaultValue = false) {
		foreach (var entry in entries) {
			if (entry.name == key) return entry.valueBool;
		}
		return defaultValue;
	}

	public float getFloat(string key, float defaultValue = 0.0f) {
		foreach (var entry in entries) {
			if (entry.name == key) return float.Parse(entry.valueString);
		}
		return defaultValue;
	}

	public int getInteger(string key, int defaultValue = 0) {
		foreach (var entry in entries) {
			if (entry.name == key) return int.Parse(entry.valueString);
		}
		return defaultValue;
	}


	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------


	// ================================================================================================================
	// AUXILIARY CLASSES

	public class ConfigEntry {

		public enum Types : int {
			String = 0,
			Int = 1,
			Float = 2,
			Boolean = 3
		}

		public string name = "";
		public Types type = Types.String;

		public string valueString = "";
		public bool valueBool = false;

		public ConfigEntry() {

		}
	}

}
