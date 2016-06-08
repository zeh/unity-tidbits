using System.Collections.Generic;

/**
 * Utilitary functions for dictionary
 * @author zeh
 */
public class JSONDictUtils {
	public static Dictionary<string, object> getDict(Dictionary<string, object> input, string key, Dictionary<string, object> defaultValue = null) {
		if (input.ContainsKey(key)) return (Dictionary<string, object>)input[key];
		return defaultValue;
	}

	public static string getString(Dictionary<string, object> input, string key, string defaultValue = "") {
		if (input.ContainsKey(key)) return (string)input[key];
		return defaultValue;
	}

	public static bool getBoolean(Dictionary<string, object> input, string key, bool defaultValue = false) {
		if (input.ContainsKey(key)) return (bool)input[key];
		return defaultValue;
	}

	public static List<object> getList(Dictionary<string, object> input, string key, List<object> defaultValue = null) {
		if (input.ContainsKey(key)) return (List<object>)input[key];
		return defaultValue;
	}
}