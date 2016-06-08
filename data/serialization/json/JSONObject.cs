using System.Collections.Generic;
using System.Text;
using UnityEngine;

/**
 * An object is an unordered set of name/value pairs. An object begins with { (left brace) and ends with } (right brace).
 * Each name is followed by : (colon) and the name/value pairs are separated by , (comma).
 * @author zeh
 */
public class JSONObject:JSONValue {

	// Constants
	private const char CHAR_START			= '{';
	private const char CHAR_END				= '}';
	private const char CHAR_KEY_SEPARATOR	= ':';
	private const char CHAR_ITEM_SEPARATOR	= ',';

	// Enums
	private enum ParsingState {
		Start,
		BeforeKey,
		AfterKey,
		BeforeValue,
		AfterValue,
		End,
	}

	// Properties
	private Dictionary<string, object> value;


	// ================================================================================================================
	// CONSTRUCTOR ----------------------------------------------------------------------------------------------------

	public JSONObject() : base() {
	}

	public JSONObject(object value) : base(value) {
	}

	public JSONObject(StringBuilder input, int position) : base(input, position) {
	}


	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	public static bool matchesInput(StringBuilder input, int position) {
		return JSONParser.compareStringValue(CHAR_START.ToString(), input, position + JSONParser.countWhitespaceCharacters(input, position));
	}

	public static bool matchesValue(object value) {
		// TODO: this will always return true?
		return !value.GetType().IsPrimitive;
	}

	public override object getValue() {
		return value;
	}

	public override void setValue(object newValue) {
		value = (Dictionary<string, object>)newValue;
	}


	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

	protected override void parseValueFromInput() {
		ParsingState parsingState = ParsingState.Start;
		char c;
		int i = 0;
		string key = "undefined";
		JSONValue valueObject;
		value = new Dictionary<string, object>();

		while (i < input.Length && parsingState != ParsingState.End) {
			c = input.ToString(inputStart + i, 1)[0]; // TODO: is this efficient?

			switch (parsingState) {
				case ParsingState.Start:
					if (c == CHAR_START) {
						// Starting object
						parsingState = ParsingState.BeforeKey;
					} else {
						Debug.LogError("Invalid character \"" + c + "\" when expecting object key start");
					}
					break;

				case ParsingState.BeforeKey:
					if (!JSONParser.isWhitespace(c)) {
						if (JSONString.matchesInput(input, inputStart + i)) {
							// Key starting
							JSONString keyObject = new JSONString(input, inputStart + i);
							key = (string) keyObject.getValue();
							i += keyObject.getInputLength() - 1;
							parsingState = ParsingState.AfterKey;
						} else if (c == CHAR_END) {
							// Premature end
							parsingState = ParsingState.End;
						} else {
							Debug.LogError("Invalid character \"" + c + "\" when expecting object key name");
						}
					}
					break;

				case ParsingState.AfterKey:
					if (!JSONParser.isWhitespace(c)) {
						if (c == CHAR_KEY_SEPARATOR) {
							parsingState = ParsingState.BeforeValue;
						} else {
							Debug.LogError("Invalid character \"" + c + "\" when expecting object key separator");
						}
					}
					break;

				case ParsingState.BeforeValue:
					if (!JSONParser.isWhitespace(c)) {
						valueObject = JSONParser.parse(input, inputStart + i);
						i += valueObject.getInputLength() - 1;
						value.Add(key, valueObject.getValue());
						parsingState = ParsingState.AfterValue;
					}
					break;

				case ParsingState.AfterValue:
					if (!JSONParser.isWhitespace(c)) {
						if (c == CHAR_END) {
							parsingState = ParsingState.End;
						} else if (c == CHAR_ITEM_SEPARATOR) {
							parsingState = ParsingState.BeforeKey;
						} else {
							Debug.LogError("Invalid character \"" + c + "\" when expecting object key end");
						}
					}
					break;
			}

			i++;
		}

		inputLength = i;
	}

	protected override string stringifyValue(int indentLevel, JSONStringifyOptions options) {
		var t = new StringBuilder();
		bool hasItemsInList = false;

		t.Append(CHAR_START);

		foreach(KeyValuePair<string, object> valueItem in value) {
			if (hasItemsInList) t.Append(CHAR_ITEM_SEPARATOR);

			t.Append(options.lineFeed + JSONEncoder.getIndents(options.individualIndent, indentLevel + 1));
			t.Append(JSONEncoder.encode(valueItem.Key, 0, options));
			t.Append(options.objectAfterKey + CHAR_KEY_SEPARATOR + options.objectBeforeValue);
			t.Append(JSONEncoder.encode(valueItem.Value, indentLevel + 1, options));

			hasItemsInList = true;
		}

		t.Append(options.lineFeed + JSONEncoder.getIndents(options.individualIndent, indentLevel));
		t.Append(CHAR_END);

		return t.ToString();
	}
}