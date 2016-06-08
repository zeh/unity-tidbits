using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/**
 * A number is very much like a C or Java number, except that the octal and hexadecimal formats are not used.
 * @author zeh
 */
public class JSONNumber:JSONValue {

	// Constants
	private static readonly char CHAR_MINUS = '-';
	private static readonly char CHAR_PLUS = '+';
	private static readonly char CHAR_PERIOD = '.';
	private static readonly char CHAR_ZERO = '0';
	private static readonly List<char> CHARS_SIGNALS = new List<char>{ CHAR_MINUS, CHAR_PLUS };
	private static readonly List<char> CHARS_DIGITS1_9 = new List<char>{ '1', '2', '3', '4', '5', '6', '7', '8', '9' };
	private static readonly List<char> CHARS_DIGITS = new List<char>{ '1', '2', '3', '4', '5', '6', '7', '8', '9', CHAR_ZERO };
	private static readonly List<char> CHARS_E = new List<char>{ 'e', 'E' };
	private static readonly List<char> CHARS_START = new List<char>{ '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-' }; //

	// Enums
	private enum ParsingState {
		Start,
		NumberAfterSignal,
		NumberDuringInteger,
		NumberAfterInteger,
		NumberDuringDecimals,
		NumberAfterDecimals,
		NumberAfterExponent,
		NumberDuringExponentInteger,
		End
	}

	// Properties
	private Decimal value;


	// ================================================================================================================
	// CONSTRUCTOR ----------------------------------------------------------------------------------------------------

	public JSONNumber() : base() {
	}

	public JSONNumber(object value) : base(value) {
	}

	public JSONNumber(StringBuilder input, int position) : base(input, position) {
	}


	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	public static bool matchesInput(StringBuilder input, int position) {
		int whitespaceChars = JSONParser.countWhitespaceCharacters(input, position);
		foreach (var startChar in CHARS_START) {
			if (JSONParser.compareStringValue(startChar.ToString(), input, position + whitespaceChars)) return true;
		}
		return false;
	}

	public static bool matchesValue(object value) {
		return value is sbyte || value is byte || value is short || value is ushort || value is int || value is uint || value is long || value is ulong || value is float || value is double || value is decimal;
	}

	public override object getValue() {
		return value;
	}

	public override void setValue(object newValue) {
		value = Convert.ToDecimal(newValue);
	}


	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

	protected override void parseValueFromInput() {
		ParsingState parsingState = ParsingState.Start;
		StringBuilder parsingString = new StringBuilder();
		char c;
		int i = 0;

		// TODO: maybe calculate each component separately?

		while (i < input.Length && parsingState != ParsingState.End) {
			c = input.ToString(inputStart + i, 1)[0]; // TODO: is this efficient?

			switch (parsingState) {
				case ParsingState.Start:
					if (!JSONParser.isWhitespace(c)) {
						if (c == CHAR_MINUS) {
							parsingString.Append(CHAR_MINUS);
						} else {
							i--;
						}
						parsingState = ParsingState.NumberAfterSignal;
					}
					break;
				case ParsingState.NumberAfterSignal:
					if (c == CHAR_ZERO) {
						parsingString.Append(c);
						parsingState = ParsingState.NumberAfterInteger;
					} else if (CHARS_DIGITS1_9.Contains(c)) {
						i--;
						parsingState = ParsingState.NumberDuringInteger;
					} else {
						Debug.LogError("Invalid number character");
					}
					break;
				case ParsingState.NumberDuringInteger:
					if (CHARS_DIGITS.Contains(c)) {
						parsingString.Append(c);
					} else {
						i--;
						parsingState = ParsingState.NumberAfterInteger;
					}
					break;
				case ParsingState.NumberAfterInteger:
					if (c == CHAR_PERIOD) {
						parsingString.Append(c);
						parsingState = ParsingState.NumberDuringDecimals;
					} else {
						i--;
						parsingState = ParsingState.NumberAfterDecimals;
					}
					break;
				case ParsingState.NumberDuringDecimals:
					if (CHARS_DIGITS.Contains(c)) {
						parsingString.Append(c);
					} else {
						i--;
						parsingState = ParsingState.NumberAfterDecimals;
					}
					break;
				case ParsingState.NumberAfterDecimals:
					if (CHARS_E.Contains(c)) {
						parsingString.Append(c);
						parsingState = ParsingState.NumberAfterExponent;
					} else {
						i--;
						parsingState = ParsingState.End;
					}
					break;
				case ParsingState.NumberAfterExponent:
					if (c == CHAR_MINUS) {
						parsingString.Append(CHAR_MINUS);
					} else if (c == CHAR_PLUS) {
						parsingString.Append(CHAR_MINUS);
					} else {
						i--;
					}
					parsingState = ParsingState.NumberDuringExponentInteger;
					break;
				case ParsingState.NumberDuringExponentInteger:
					if (CHARS_DIGITS.Contains(c)) {
						parsingString.Append(c);
					} else {
						i--;
						parsingState = ParsingState.End;
					}
					break;
			}

			i++;
		}

		inputLength = i;
		value = Convert.ToDecimal(parsingString.ToString());
	}

	protected override string stringifyValue(int indentLevel, JSONStringifyOptions options) {
		return value.ToString();
	}
}