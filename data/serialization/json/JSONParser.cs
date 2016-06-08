using System.Collections.Generic;
using System.Text;
using UnityEngine;

/**
 * A class to parse Strings and turn them into specific JSON types
 * @author zeh
 */
public class JSONParser {

	// Constants
	private static readonly List<char> CHARS_WHITESPACE = new List<char>{
		'\u0020', // Space
		'\u0009', // Horizontal tab
		'\u000A', // Line feed or new line
		'\u000D'  // Carriage return
	};


	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	/**
	 * Parses a string into its correct equivalent JSON value
	 */
	public static JSONValue parse(string input) {
		JSONValue jsonValue = parse(new StringBuilder(input), 0);
		return jsonValue;
	}

	/**
	 * Parses a StringBuilder into its correct equivalent JSON value, starting at a specific position
	 */
	public static JSONValue parse(StringBuilder input, int position) {
		// Decides what it is
		if (JSONObject.matchesInput(input, position)) {
			return new JSONObject(input, position);
		} else if (JSONArray.matchesInput(input, position)) {
			return new JSONArray(input, position);
		} else if (JSONString.matchesInput(input, position)) {
			return new JSONString(input, position);
		} else if (JSONNumber.matchesInput(input, position)) {
			return new JSONNumber(input, position);
		} else if (JSONLiteral.matchesInput(input, position)) {
			return new JSONLiteral(input, position);
		} else {
			// Invalid
			Debug.LogError("No valid type found when parsing " + input + " @ " + position + " (" + input.ToString(position, input.Length - position) + ")");
			return null;
		}
	}

	/**
	 * Tests whether a StringBuilder has a specific string at a specific position
	 */
	public static bool compareStringValue(string valueToSearch, StringBuilder fullText, int position) {
		return valueToSearch.Length <= fullText.Length - position && fullText.ToString(position, valueToSearch.Length) == valueToSearch;
	}

	/**
	 * Counts the number of uninterrupted whitespace characters in a StringBuilder instance, starting at a specific position
	 */
	public static int countWhitespaceCharacters(StringBuilder input, int position) {
		int chars = 0;
		while (position + chars < input.Length && isWhitespace(input.ToString(position + chars, 1)[0])) {
			chars++;
		}
		return chars;
	}

	/**
	 * Tests whether a specific character is a whitespace character or not
	 */
	public static bool isWhitespace(char c) {
		return CHARS_WHITESPACE.Contains(c);
	}
}