using System.Collections.Generic;
using System.Text;
using UnityEngine;

/**
 * Literals: true, false, or null
 * @author zeh
 */
public class JSONLiteral:JSONValue {

	// Constants
	private static readonly string LITERAL_TRUE = "true";
	private static readonly string LITERAL_FALSE = "false";
	private static readonly string LITERAL_NULL = "null";
	private static readonly List<string> LITERALS_VALID = new List<string>{ LITERAL_TRUE, LITERAL_FALSE, LITERAL_NULL};

	// Properties
	private object value;


	// ================================================================================================================
	// CONSTRUCTOR ----------------------------------------------------------------------------------------------------

	public JSONLiteral() : base() {
	}

	public JSONLiteral(object value) : base(value) {
	}

	public JSONLiteral(StringBuilder input, int position) : base(input, position) {
	}


	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	public static bool matchesInput(StringBuilder input, int position) {
		int whitespaceChars = JSONParser.countWhitespaceCharacters(input, position);
		foreach (var literal in LITERALS_VALID) {
			if (JSONParser.compareStringValue(literal, input, position + whitespaceChars)) return true;
		}
		return false;
	}

	public static bool matchesValue(object value) {
		return value == null || value is bool;
	}

	public override object getValue() {
		return value;
	}

	public override void setValue(object newValue) {
		value = newValue;
	}


	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

	protected override void parseValueFromInput() {
		if (JSONParser.compareStringValue(LITERAL_TRUE, input, inputStart)) {
			inputLength = LITERAL_TRUE.Length;
			value = true;
		} else if (JSONParser.compareStringValue(LITERAL_FALSE, input, inputStart)) {
			inputLength = LITERAL_FALSE.Length;
			value = false;
		} else if (JSONParser.compareStringValue(LITERAL_NULL, input, inputStart)) {
			inputLength = LITERAL_NULL.Length;
			value = null;
		} else {
			Debug.LogError("Invalid literal constant");
		}
	}

	protected override string stringifyValue(int indentLevel, JSONStringifyOptions options) {
		if (value == null) {
			// It's a null
			return LITERAL_NULL;
		} else if (value is bool) {
			// It's a Boolean
			return (bool)value ? LITERAL_TRUE : LITERAL_FALSE;
		} else {
			Debug.LogError("Invalid literal value");
			return null;
		}
	}
}