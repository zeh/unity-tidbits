#if UNITY_WEBPLAYER || UNITY_WEBGL
#define USE_PLAYER_PREFS
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

public class PersistentData {
	/*
	Saves and loads persistent user data in a simple but safer fashion, with more features.

	* Uses system file I/O when possible, or PlayerPrefs when running web version
	* Uses singleton-ish access (based on itds), no instantiation or references necessary

	Advantages over PlayerPrefs:
	* Allow defaults when reading
	* Also save/load bools, double, long
	* Allow byte arrays (as base64 encoded data in UserPrefs, normal byte array when using file I/O)
	* No collision of data when using across different objects (using own persistentdata instances)
	* faster?

	How to:
	// Create/get instance
	PersistentData pd = PersistentData.getInstance(); // global
	PersistentData pd = PersistentData.getInstance("something"); // Custom name
 
	// Read/write:
	pd.SetString("name", "dude");
	var str = pd.GetString("name");

 	pd.SetBytes("name", new byte[] {...});
	byte[] bytes = pd.GetBytes("name");
	
	pd.SetObject("name", myObject); // myObject must be Serializable
	SomeType myObject = pd.GetObject("name") as SomeType;

	// Options:
	pd.cacheValues = true;		// Save primitive values in memory for faster access (default true)
	pd.cacheByteArrays = true;	// Save byte array values in memory for faster access (default false) (untested)

	
	// Good example of another solution (that tries to replace playerprefs): http://www.previewlabs.com/wp-content/uploads/2014/04/PlayerPrefs.cs
	
	TODO/test:
	* Save/write serializable object (test)
	* More secure/encrypted writing (use StringUtils.encodeRC4 ?)
	* Save/load lists? test
	* First read may be too slow. Test
	* add date time objects?
	* Use SystemInfo.deviceUniqueIdentifier.Substring for key? would not allow carrying data over devices though
	* Make sure files are deleted on key removal/clear
	* Use compression? System.IO.Compression.GZipStream
	*/

	// Constant properties
	private static Dictionary<string, PersistentData> dataGroups;

	private const string SERIALIZATION_SEPARATOR = ",";
	private const string FIELD_NAME_KEYS = "keys";
	private const string FIELD_NAME_VALUES = "values";
	private const string FIELD_NAME_TYPES = "types";
	private const string FIELD_NAME_BYTE_ARRAY_KEYS = "byteArrayKeys";
	
	private const string FIELD_TYPE_BOOLEAN = "b";
	private const string FIELD_TYPE_FLOAT = "f";
	private const string FIELD_TYPE_DOUBLE = "d";
	private const string FIELD_TYPE_INT = "i";
	private const string FIELD_TYPE_LONG = "l";
	private const string FIELD_TYPE_STRING = "s";
	private const string FIELD_TYPE_BYTE_ARRAY = "y";
	//private static const string FIELD_TYPE_OBJECT = "o"; // Non-primitive

	// Properties
	private string namePrefix;						// Unique prefix for field names
	private string _name;							// Instance name

	private bool _cacheData;						// Whether primitives (ints, floats, etc) should be cached
	private bool _cacheByteArrays;					// Whether serializable objects should be cached

	private Hashtable dataToBeWritten;				// Data that is waiting to be written: ints, floats, strings, and objects as serialized strings; key, value
	private List<string> byteArraysToBeDeleted;			// Data that is waiting to be deleted from the system
	private Hashtable cachedData;					// All items that have been cached

	private List<string> dataKeys;					// Keys of all ids used
	private List<string> byteArrayKeys;				// Keys of all byte array ids used


	// ================================================================================================================
	// STATIC ---------------------------------------------------------------------------------------------------------

	static PersistentData() {
		dataGroups = new Dictionary<string, PersistentData>();
	}
	

	// ================================================================================================================
	// CONSTRUCTOR ----------------------------------------------------------------------------------------------------

	public PersistentData(string name = "") {
		_name = name;
		namePrefix = getMD5("p_" + _name) + "_";
		_cacheData = true;
		_cacheByteArrays = false;

		dataToBeWritten = new Hashtable();
		byteArraysToBeDeleted = new List<String>();
		cachedData = new Hashtable();
		PersistentData.addInstance(this);

		dataKeys = loadStringList(FIELD_NAME_KEYS);
		byteArrayKeys = loadStringList(FIELD_NAME_BYTE_ARRAY_KEYS);
	}

	public void Debug_Log() {
		// Temp!
		Debug.Log("Primitive keys ==> (" + dataKeys.Count + ") " + string.Join(",", dataKeys.ToArray()));
		Debug.Log("Byte array keys ==> (" + byteArrayKeys.Count + ") " + string.Join(",", byteArrayKeys.ToArray()));
		//foreach (var item in dataKeys) Debug.Log("     [" + item + "]");
		foreach (var key in byteArrayKeys) Debug.Log("     [" + key + "] = " + System.Text.Encoding.UTF8.GetString(GetBytes(key)));
	}


	// ================================================================================================================
	// STATIC INTERFACE -----------------------------------------------------------------------------------------------

	private static void addInstance(PersistentData dataGroup) {
		dataGroups.Add(dataGroup.name, dataGroup);
	}

	public static PersistentData getInstance(string name = "") {
		if (dataGroups.ContainsKey(name)) return dataGroups[name];

		// Doesn't exist, create a new one and return it
		return new PersistentData(name);
	}


	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	// Get

	public bool GetBool(string key, bool defaultValue = false) {
		return dataKeys.Contains(key) ? getValue<bool>(key) : defaultValue;
	}

	public int GetInt(string key, int defaultValue = 0) {
		return dataKeys.Contains(key) ? getValue<int>(key) : defaultValue;
	}

	public long GetLong(string key, long defaultValue = 0) {
		return dataKeys.Contains(key) ? getValue<long>(key) : defaultValue;
	}

	public float GetFloat(string key, float defaultValue = 0.0f) {
		return dataKeys.Contains(key) ? getValue<float>(key) : defaultValue;
	}

	public double GetDouble(string key, double defaultValue = 0.0) {
		return dataKeys.Contains(key) ? getValue<double>(key) : defaultValue;
	}

	public string GetString(string key, string defaultValue = "") {
		return dataKeys.Contains(key) ? getValue<string>(key) : defaultValue;
	}

	public byte[] GetBytes(string key) {
		return byteArrayKeys.Contains(key) ? getValueBytes(key) : null;
	}

	public T GetObject<T>(string key) {
		return byteArrayKeys.Contains(key) ? (T)deserializeObject(getValueBytes(key)) : default(T);
	}
	
	// Set

	public void SetBool(string key, bool value) {
		dataToBeWritten[key] = value;
		if (_cacheData) cachedData.Remove(key);
	}

	public void SetInt(string key, int value) {
		dataToBeWritten[key] = value;
		if (_cacheData) cachedData.Remove(key);
	}

	public void SetLong(string key, long value) {
		dataToBeWritten[key] = value;
		if (_cacheData) cachedData.Remove(key);
	}

	public void SetFloat(string key, float value) {
		dataToBeWritten[key] = value;
		if (_cacheData) cachedData.Remove(key);
	}

	public void SetDouble(string key, double value) {
		dataToBeWritten[key] = value;
		if (_cacheData) cachedData.Remove(key);
	}

	public void SetString(string key, string value) {
		dataToBeWritten[key] = value;
		if (_cacheData) cachedData.Remove(key);
	}

	public void SetBytes(string key, byte[] value) {
		dataToBeWritten[key] = value;
		if (_cacheData) cachedData.Remove(key);
	}

	public void SetObject(string key, object serializableObject) {
		SetBytes(key, serializeObject(serializableObject));
	}
	
	// Utils

	public void Clear() {
		dataKeys.Clear();
		dataToBeWritten.Clear();
		while (byteArrayKeys.Count > 0) {
			byteArraysToBeDeleted.Add(byteArrayKeys[0]);
			byteArrayKeys.RemoveAt(0);
		}
		ClearCache();
		Save(true);
	}

	public void ClearCache() {
		cachedData.Clear();
	}

	public void RemoveKey(string key) {
		dataKeys.Remove(key);
		dataToBeWritten.Remove(key);
		cachedData.Remove(key);
		if (byteArrayKeys.Contains(key)) {
			byteArrayKeys.Remove(key);
			byteArraysToBeDeleted.Add(key);
		}
	}

	public bool HasKey(string key) {
		return dataKeys.Contains(key) || byteArrayKeys.Contains(key) || dataToBeWritten.Contains(key);
	}
	
	public void Save(bool forced = false) {
		if (dataToBeWritten.Count > 0 || byteArraysToBeDeleted.Count > 0 || forced) {
			// Some fields need to be saved

			// Read all existing values
			List<string> dataValues = loadStringList(FIELD_NAME_VALUES);
			List<string> dataTypes = loadStringList(FIELD_NAME_TYPES);

			// Record new values
			string fieldKey;
			object fieldValue;
			string fieldType;
			int pos;

			IDictionaryEnumerator enumerator = dataToBeWritten.GetEnumerator();
			while (enumerator.MoveNext()) {
				fieldKey = enumerator.Key.ToString();
				fieldValue = enumerator.Value;
				fieldType = getFieldType(fieldValue);

				//Debug.Log("Saving => [" + fieldValue + "] of type [" + fieldType + "]");

				if (fieldType == FIELD_TYPE_BYTE_ARRAY) {
					//Debug.Log("  Saving as byte array");
					// Byte array
					if (!byteArrayKeys.Contains(fieldKey)) {
						// Adding a key
						byteArrayKeys.Add(fieldKey);
					}
					setSavedBytes(fieldKey, (byte[])fieldValue);
				} else {
					//Debug.Log("  Saving as native array");
					// Primitive data
					if (dataKeys.Contains(fieldKey)) {
						// Replacing a key
						pos = dataKeys.IndexOf(fieldKey);
						dataValues[pos] = Convert.ToString(fieldValue);
						dataTypes[pos] = fieldType;
					} else {
						// Adding a key
						dataKeys.Add(fieldKey);
						dataValues.Add(Convert.ToString(fieldValue));
						dataTypes.Add(fieldType);
					}
				}
			}

			dataToBeWritten.Clear();

			// Write primitives
			StringBuilder builderFieldKeys = new StringBuilder();
			StringBuilder builderFieldValues = new StringBuilder();
			StringBuilder builderFieldTypes = new StringBuilder();

			for (int i = 0; i < dataKeys.Count; i++) {
				//Debug.Log("Adding data key [" + i + "] for [" + dataKeys[i] + "]");
				if (i > 0) {
					builderFieldKeys.Append(SERIALIZATION_SEPARATOR);
					builderFieldValues.Append(SERIALIZATION_SEPARATOR);
					builderFieldTypes.Append(SERIALIZATION_SEPARATOR);
				}
				builderFieldKeys.Append(encodeString(dataKeys[i]));
				builderFieldValues.Append(encodeString(dataValues[i]));
				builderFieldTypes.Append(encodeString(dataTypes[i]));
			}

			setSavedString(FIELD_NAME_KEYS, builderFieldKeys.ToString());
			setSavedString(FIELD_NAME_VALUES, builderFieldValues.ToString());
			setSavedString(FIELD_NAME_TYPES, builderFieldTypes.ToString());

			// Write byte array keys
			StringBuilder builderByteArrayKeys = new StringBuilder();

			for (int i = 0; i < byteArrayKeys.Count; i++) {
				if (i > 0) {
					builderByteArrayKeys.Append(SERIALIZATION_SEPARATOR);
				}
				builderByteArrayKeys.Append(encodeString(byteArrayKeys[i]));
			}

			setSavedString(FIELD_NAME_BYTE_ARRAY_KEYS, builderByteArrayKeys.ToString());

			// Clears deleted byte arrays
			foreach (var key in byteArraysToBeDeleted) {
				clearSavedStringsOrBytes(key);
			}
			byteArraysToBeDeleted.Clear();
		}
	}


	// ================================================================================================================
	// ACCESSOR INTERFACE ---------------------------------------------------------------------------------------------

	public string name {
		get { return _name; }
	}

	public bool cacheData {
		get { return _cacheData; }
		set {
			if (_cacheData != value) {
				_cacheData = value;
				if (!_cacheData) {
					foreach (var key in dataKeys) cachedData.Remove(key);
				}
			}
		}
	}

	public bool cacheByteArrays {
		get { return _cacheByteArrays; }
		set {
			if (_cacheByteArrays != value) {
				_cacheByteArrays = value;
				if (!_cacheByteArrays) {
					foreach (var key in byteArrayKeys) cachedData.Remove(key);
				}
			}
		}
	}


	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

	private List<string> loadStringList(string fieldName) {
		// Loads a string list from a field
		var encodedList = getSavedString(fieldName).Split(SERIALIZATION_SEPARATOR.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
		var decodedList = new List<string>(encodedList.Length);
		foreach (string listItem in encodedList) {
			decodedList.Add(decodeString(listItem));
		}
		//Debug.Log("Reading field [" + fieldName + "]; value as encoded = [" + getSavedString(fieldName) + "] with [" + encodedList.Length + "] items");
		return decodedList;
	}

	/*
	private string loadStringListItem(string fieldName, int pos) {
		// Load just one position from the string list
		var encodedList = getSavedString(fieldName).Split(SERIALIZATION_SEPARATOR.ToCharArray(), pos + 1);
		Debug.Log("Reading field [" + fieldName + "] at position[" + pos + "]; value as encoded = [" + getSavedString(fieldName) + "] with [" + encodedList.Length + "] items");
		return decodeString(encodedList[pos]);
	}
	*/

	private byte[] serializeObject(object serializableObject) {
		// Returns a serializable object as a byte array
		using (var memoryStream = new MemoryStream()) {
			var formatter = new BinaryFormatter();
			formatter.Serialize(memoryStream, serializableObject);
			memoryStream.Flush();
			memoryStream.Position = 0;
			return memoryStream.ToArray();
		}
	}

	private object deserializeObject(byte[] source) {
		// Creates a serializable object from a byte array
		using (var memoryStream = new MemoryStream(source)) {
			var formatter = new BinaryFormatter();
			memoryStream.Seek(0, SeekOrigin.Begin);
			return formatter.Deserialize(memoryStream);
		}
	}
	
	private string getKeyForName(string name) {
		// Return a field name that is specific to this instance
		return namePrefix + name.Replace(".", "_").Replace("/", "_").Replace("\\", "_");
	}

	private T getValue<T>(string key) {
		// Returns the value of a given primitive field, cast to the required type

		// If waiting to be saved, return it
		if (dataToBeWritten.ContainsKey(key)) return (T)dataToBeWritten[key];

		// If already cached, return it
		if (_cacheData && cachedData.ContainsKey(key)) return (T)cachedData[key];

		// Read previously saved data
		var pos = dataKeys.IndexOf(key);
		var fieldType = loadStringList(FIELD_NAME_TYPES)[pos];
		T value = (T)getValueAsType(loadStringList(FIELD_NAME_VALUES)[pos], fieldType);

		// Save to cache
		if (_cacheData) cachedData[key] = value;

		return value;
	}

	private byte[] getValueBytes(string key) {
		// Returns the value of a given byte[] field

		// If waiting to be saved, return it
		if (dataToBeWritten.ContainsKey(key)) return (byte[])dataToBeWritten[key];

		// If already cached, return it
		if (_cacheData && cachedData.ContainsKey(key)) return (byte[])cachedData[key];

		// Read previously saved data
		byte[] value = getSavedBytes(key);

		// Save to cache
		if (_cacheByteArrays) cachedData[key] = value;

		return value;
	}

	private string getMD5(string src) {
		// Basic MD5 for short-ish name uniqueness
		// Source: http://wiki.unity3d.com/index.php?title=MD5
		
		System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
		byte[] bytes = ue.GetBytes(src);
	 
		// Encrypt
		System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
		byte[] hashBytes = md5.ComputeHash(bytes);
	 
		// Convert the encrypted bytes back to a string (base 16)
		string hashString = "";
	 
		for (int i = 0; i < hashBytes.Length; i++) {
			hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
		}
	 
		return hashString.PadLeft(32, '0');
	}
	
	private string encodeString(string src) {
		return src.Replace("\\", "\\\\").Replace(SERIALIZATION_SEPARATOR, "\\" + SERIALIZATION_SEPARATOR);
	}

	private string decodeString(string src) {
		return src.Replace("\\" + SERIALIZATION_SEPARATOR, SERIALIZATION_SEPARATOR).Replace("\\\\", "\\");
	}
	
	private object getValueAsType(string fieldValue, string fieldType) {
		if (fieldType == FIELD_TYPE_BOOLEAN)	return Convert.ToBoolean(fieldValue);
		if (fieldType == FIELD_TYPE_INT)		return Convert.ToInt32(fieldValue);
		if (fieldType == FIELD_TYPE_LONG)		return Convert.ToInt64(fieldValue);
		if (fieldType == FIELD_TYPE_FLOAT)		return Convert.ToSingle(fieldValue);
		if (fieldType == FIELD_TYPE_DOUBLE)		return Convert.ToDouble(fieldValue);
		if (fieldType == FIELD_TYPE_STRING)		return (object)fieldValue.ToString();
		//if (fieldType == FIELD_TYPE_BYTE_ARRAY)	return (object)fieldValue.ToString();
		//if (fieldType == FIELD_TYPE_OBJECT)		return (object)fieldValue.ToString();

		Debug.LogError("Unsupported type for conversion: [" + fieldType + "]");
		return null;
	}

	private string getFieldType(object value) {
		var realFieldType = value.GetType().ToString();
		// TODO: use "is" ? 
		if (realFieldType == "System.Boolean")	return FIELD_TYPE_BOOLEAN;
		if (realFieldType == "System.Int32")	return FIELD_TYPE_INT;
		if (realFieldType == "System.Int64")	return FIELD_TYPE_LONG;
		if (realFieldType == "System.Single")	return FIELD_TYPE_FLOAT;
		if (realFieldType == "System.Double")	return FIELD_TYPE_DOUBLE;
		if (realFieldType == "System.String")	return FIELD_TYPE_STRING;
		if (realFieldType == "System.Byte[]")	return FIELD_TYPE_BYTE_ARRAY;
		return null;
	}

	private void clearSavedStringsOrBytes(string name) {
		// Removes a byte array or string that has been saved previously
		// Save a string to some persistent data system
		#if USE_PLAYER_PREFS
			// Using PlayerPrefs
			PlayerPrefs.DeleteKey(getKeyForName(name));
		#else
			// Using a file
			deleteFile(getKeyForName(name));
		#endif
	}

	private byte[] getSavedBytes(string name) {
		// Reads a byte array that has been saved previously
		#if USE_PLAYER_PREFS
			// Using PlayerPrefs
			return Convert.FromBase64String(PlayerPrefs.GetString(getKeyForName(name)));
		#else
			// Using a file
			return loadFileAsBytes(getKeyForName(name));
		#endif
	}

	private void setSavedBytes(string name, byte[] value) {
		// Save a string to some persistent data system
		#if USE_PLAYER_PREFS
			// Using PlayerPrefs
			PlayerPrefs.SetString(getKeyForName(name), Convert.ToBase64String(value));
		#else
			// Using a file
			saveBytesToFile(getKeyForName(name), value);
		#endif
	}

	private string getSavedString(string name) {
		// Reads a string that has been saved previously
		#if USE_PLAYER_PREFS
			// Using PlayerPrefs
			return PlayerPrefs.GetString(getKeyForName(name));
		#else
			// Using a file
			return loadFileAsString(getKeyForName(name));
		#endif
	}

	private void setSavedString(string name, string value) {
		// Save a string to some persistent data system
		#if USE_PLAYER_PREFS
			// Using PlayerPrefs
			PlayerPrefs.SetString(getKeyForName(name), value);
		#else
			// Using a file
			saveStringToFile(getKeyForName(name), value);
		#endif
	}

	private void deleteFile(string filename) {
		File.Delete(Application.persistentDataPath + "/" + filename);
	}

	private void saveStringToFile(string filename, string content) {
		BinaryFormatter formatter = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/" + filename);
		formatter.Serialize(file, content);
		file.Close();
	}

	private void saveBytesToFile(string filename, byte[] content) {
		BinaryFormatter formatter = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/" + filename);
		formatter.Serialize(file, content);
		file.Close();
	}

	private string loadFileAsString(string filename) {
		object obj = loadFileAsObject(filename);
		return obj == null ? "" : (string) obj;
	}

	private byte[] loadFileAsBytes(string filename) {
		object obj = loadFileAsObject(filename);
		return obj == null ? null : (byte[]) obj;
	}

	private object loadFileAsObject(string filename) {
		string filePath = Application.persistentDataPath + "/" + filename;
		if (File.Exists(filePath)) {
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream file = File.Open(filePath, FileMode.Open);
			object content = formatter.Deserialize(file);
			file.Close();
			return content;
		}
		return null;
	}
}
