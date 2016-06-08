using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/**
 * An array is an ordered collection of values.
 * An array begins with [ (left bracket) and ends with ] (right bracket).
 * Values are separated by , (comma).
 * @author zeh
 */
public class JSONArray:JSONValue {

	// Constants
    private const char CHAR_START				= '[';
    private const char CHAR_END					= ']';
    private const char CHAR_ITEM_SEPARATOR		= ',';

	// Enums
    private enum ParsingState {
        Start,
        BeforeValue,
        AfterValue,
        End,
    }

	// Properties
    private IList value;


	// ================================================================================================================
	// CONSTRUCTOR ----------------------------------------------------------------------------------------------------

	public JSONArray() : base() {
	}

	public JSONArray(object value) : base(value) {
	}

	public JSONArray(StringBuilder input, int position) : base(input, position) {
	}


	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	public static bool matchesInput(StringBuilder input, int position) {
        return JSONParser.compareStringValue(CHAR_START.ToString(), input, position + JSONParser.countWhitespaceCharacters(input, position));
    }

	public static bool matchesValue(object value) {
		return value is IList;
	}

    public override object getValue() {
        return value;
    }

	public override void setValue(object newValue) {
		value = (IList)newValue;
	}


	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

	protected override void parseValueFromInput() {
        ParsingState parsingState = ParsingState.Start;
        char c;
        int i = 0;
        JSONValue valueObject;
        value = new List<object>();

        while (i < input.Length && parsingState != ParsingState.End) {
            c = input.ToString(inputStart + i, 1)[0]; // TODO: is this efficient?

            switch (parsingState) {
                case ParsingState.Start:
                    if (c == CHAR_START) {
                        // Starting array
                        parsingState = ParsingState.BeforeValue;
                    } else {
                        Debug.LogError("Invalid character \"" + c + "\" when expecting array start");
                    }
                    break;

                case ParsingState.BeforeValue:
                    if (!JSONParser.isWhitespace(c)) {
                        valueObject = JSONParser.parse(input, inputStart + i);
                        i += valueObject.getInputLength() - 1;
                        value.Add(valueObject.getValue());
                        parsingState = ParsingState.AfterValue;
                    }
                    break;

                case ParsingState.AfterValue:
                    if (!JSONParser.isWhitespace(c)) {
                        if (c == CHAR_END) {
                            parsingState = ParsingState.End;
                        } else if (c == CHAR_ITEM_SEPARATOR) {
                            parsingState = ParsingState.BeforeValue;
                        } else {
                            Debug.LogError("Invalid character \"" + c + "\" when expecting array end or item separator");
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

		foreach (object valueItem in value) {
			if (hasItemsInList) t.Append(CHAR_ITEM_SEPARATOR);

			if (options.lineFeedOnArrays) {
				t.Append(options.lineFeed + JSONEncoder.getIndents(options.individualIndent, indentLevel + 1));
			} else {
				t.Append(options.arrayAfterSeparator);
			}
			t.Append(JSONEncoder.encode(valueItem, indentLevel + 1, options));

			hasItemsInList = true;
		}

		t.Append(options.lineFeed + JSONEncoder.getIndents(options.individualIndent, indentLevel));
		t.Append(CHAR_END);

		return t.ToString();
	}
}