using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/**
 * A string is a sequence of zero or more Unicode characters, wrapped in double quotes, using backslash escapes.
 * A character is represented as a single character string. A string is very much like a C or Java string.
 * @author zeh
 */
public class JSONString:JSONValue {
	// Constants
	private const char CHAR_ESCAPE = '\\';
	private const char CHAR_START = '\"';
	private const char CHAR_END = '\"';
	private const string STRING_ESCAPE_UNICODE = @"\u";

	private static readonly List<char> CHARS_STRING_NEED_ESCAPE = new List<char>{
		'\"', // Quotation mark
		'\\', // Reverse solidus (backslash)
		'/',  // Solidus (forward slash)
		'\b', // Backspace
		'\f', // Form feed
		'\n', // Line feed
		'\r', // Carriage return
		'\t'  // Tab
	};

	private static readonly List<string> CHARS_STRING_ESCAPED = new List<string>{
		@"\""", // Quotation mark
		@"\\",  // Reverse solidus (backslash)
		@"\/",  // Solidus (forward slash)
		@"\b",  // Backspace
		@"\f",  // Form feed
		@"\n",  // Line feed
		@"\r",  // Carriage return
		@"\t"   // Tab
	};

	// Enums
	private enum ParsingState {
		Start,
		Middle,
		End,
	}

	// Properties
	private string value;


	// ================================================================================================================
	// CONSTRUCTOR ----------------------------------------------------------------------------------------------------

	public JSONString() : base() {
	}

	public JSONString(object value) : base(value) {
	}

	public JSONString(StringBuilder input, int position) : base(input, position) {
	}


	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	public static bool matchesInput(StringBuilder input, int position) {
		return JSONParser.compareStringValue(CHAR_START.ToString(), input, position + JSONParser.countWhitespaceCharacters(input, position));
	}

	public static bool matchesValue(object value) {
		return value is string || value is char;
	}

	public override object getValue() {
		return value;
	}

	public override void setValue(object newValue) {
		value = (string)newValue;
	}


	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

	protected override void parseValueFromInput() {
		ParsingState parsingState = ParsingState.Start;
		StringBuilder parsingString = new StringBuilder();
		char c;
		int i = 0;

		while (i < input.Length && parsingState != ParsingState.End) {
			c = input.ToString(inputStart + i, 1)[0]; // TODO: is this efficient?

			switch (parsingState) {
				case ParsingState.Start:
					if (c == CHAR_START) {
						// Starting string
						parsingState = ParsingState.Middle;
					} else if (!JSONParser.isWhitespace(c)) {
						Debug.LogError("Invalid string starting character \" + c + \"");
					}
					break;

				case ParsingState.Middle:
					if (c == CHAR_END) {
						// Ended string
						parsingState = ParsingState.End;
					} else if (JSONParser.compareStringValue(STRING_ESCAPE_UNICODE, input, inputStart + i)) {
						// Unicode encoded character
						i++;
						if (i < input.Length - 5) {
							parsingString.Append((char)(Convert.ToInt16(input.ToString(inputStart+i+1, 4), 16)));
							i += 4;
						} else {
							Debug.LogError("Unicode string started with not enough characters");
						}
					} else if (c == CHAR_ESCAPE) {
						// Some special character
						if (i < input.Length - 1) {
							string nc = c.ToString() + input[i+1];
							int charPos = CHARS_STRING_ESCAPED.IndexOf(nc);
							if (charPos >= 0) {
								parsingString.Append(CHARS_STRING_NEED_ESCAPE[charPos]);
								i++;
							} else {
								Debug.LogError("Escape string without equivalent char");
							}
						}
					} else {
						// Continued string
						parsingString.Append(c);
					}
					break;
			}

			i++;
		}

		inputLength = i;
		value = parsingString.ToString();
	}

	protected override string stringifyValue(int indentLevel, JSONStringifyOptions options) {
		// Encodes a string properly, escaping chars that need escaping
		var t = new StringBuilder();
		int i;
		char c;
		int charPos;
		string unicodeNumber;

		t.Append(CHAR_START);

		for (i = 0; i < value.Length; i++) {
			c = value[i];
			//Debug.Log("CHAR => " + (int)c + " = [" + c + "]");
			charPos = CHARS_STRING_NEED_ESCAPE.IndexOf(c);
			if (charPos >= 0) {
				// Special char: needs to be escaped
				t.Append(CHARS_STRING_ESCAPED[charPos]);
			} else if ((int)c < 0x20) {
				// Outside the range: needs to be escaped
				unicodeNumber = ((int)c).ToString("X");
				t.Append(STRING_ESCAPE_UNICODE + unicodeNumber.PadLeft(4, '0').ToLowerInvariant());
			} else if ((int)c >= 0xc200) {
				// Multi byte char
				// c2..fd means initial bytes of unicode, fe..ff means the char bytes themselves
				unicodeNumber = ((int)c).ToString("X");
				t.Append(STRING_ESCAPE_UNICODE + unicodeNumber.PadLeft(4, '0').ToLowerInvariant());
			} else {
				// Normal char
				t.Append(c);
			}
		}

		t.Append(CHAR_END);

		return t.ToString();
	}
}