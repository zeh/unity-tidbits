using System.Text;

/**
* A class to encode objects into JSON strings
* @author zeh
*/
public class JSONEncoder {

	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	/**
	 * Encodes an object into a string
	 */
	public static string encode(object value, bool prettyPrint = false) {
		if (prettyPrint) {
			return encode(value, 0, JSONStringifyOptions.getPretty());
		} else {
			return encode(value, 0, JSONStringifyOptions.getCompact());
		}
	}

	/**
	 * Encodes an object into a string, correctly identifying the type
	 */
	public static string encode(object value, int indentLevel, JSONStringifyOptions options) {
		// Decides what it is
		if (JSONArray.matchesValue(value)) {
			return new JSONArray(value).ToString(indentLevel, options);
		} else if (JSONString.matchesValue(value)) {
			return new JSONString(value).ToString(indentLevel, options);
		} else if (JSONNumber.matchesValue(value)) {
			return new JSONNumber(value).ToString(indentLevel, options);
		} else if (JSONLiteral.matchesValue(value)) {
			return new JSONLiteral(value).ToString(indentLevel, options);
		} else {
			return new JSONObject(value).ToString(indentLevel, options);
		}
	}

	/**
	 * Creates indents by repeating the same string
	 */
	public static string getIndents(string indent, int times) {
		var t = new StringBuilder();
		while (times > 0) {
			t.Append(indent);
			times--;
		}
		return t.ToString();
	}
}