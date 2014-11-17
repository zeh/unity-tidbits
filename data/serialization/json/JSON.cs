using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

/**
 * @author zeh
 */
public class JSON {

	/*
	TODO: 
	* Allow removal of comments
	* Allow max depth 
	* Proper number parse
	* Create a strong typed List<> when an array of a specific type is identified
	* Escape everything below 0x0020, or above 0x10ffffff
	* Decide on NaN, Infinity
	* Decide on key strings without quotes
	* Use stringify() and parse() instead?
	* Make encoding faster with string buffers instead
	* Auto encoder that understands vectors too?

	Test:
	* Encoding array
	* Unicode, proper high char
	* Empty strings, arrays, objects
	* Use heavy JSOn from JSON test suite: https://code.google.com/p/json-test-suite/
	*/
	
	// http://www.json.org/
	// http://www.ietf.org/rfc/rfc4627.txt
	
	/*
   A JSON parser transforms a JSON text into another representation.  A
   JSON parser MUST accept all texts that conform to the JSON grammar.
   A JSON parser MAY accept non-JSON forms or extensions.

   An implementation may set limits on the size of texts that it
   accepts.  An implementation may set limits on the maximum depth of
   nesting.  An implementation may set limits on the range of numbers.
   An implementation may set limits on the length and character contents
   of strings.
	*/

	// Configuration flags
	// Cannot be enum... makes the comparison too verbose later
	//private static readonly bool USE_SINGLE_TYPE_LIST = true;					// Attempt to create arrays of single types by checking all properties inside
	public static readonly int FLAG_REMOVE_COMMENTS = 1;

	// Constants for parsing
	private const char STRUCTURE_BEGIN_ARRAY		= '[';
	private const char STRUCTURE_END_ARRAY			= ']';
	private const char STRUCTURE_BEGIN_OBJECT		= '{';
	private const char STRUCTURE_END_OBJECT			= '}';
	private const char STRUCTURE_NAME_SEPARATOR		= ':';
	private const char STRUCTURE_VALUE_SEPARATOR	= ',';
	private const char STRUCTURE_STRING_DELIMITER	= '\"';

	private static readonly List<char> CHARS_WHITESPACE = new List<char>{
		'\u0020', // Space
		'\u0009', // Horizontal tab
		'\u000A', // Line feed or new line
		'\u000D'  // Carriage return
	};

	// Value literals
	private const string VALUE_LITERAL_TRUE			= "true";
	private const string VALUE_LITERAL_FALSE		= "false";
	private const string VALUE_LITERAL_NULL			= "null";

	// Valid chars
	private static readonly List<char> CHARS_NUMBER = new List<char>{
		'.', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'e', 'E', '+', '-'
	};
	// TODO: parse number properly

	/*
	private const char CHAR_NUMBER_MINUS			= '-';
	private const char CHAR_NUMBER_PLUS				= '+';
	private const char CHAR_NUMBER_DECIMAL			= '.';
	private const char CHAR_NUMBER_ZERO				= '0';
	private static readonly List<char> CHARS_NUMBER_EXPONENTIAL = new List<char>{
		'e', 'E'
	};
	private static readonly List<char> CHARS_NUMBER_DIGITS = new List<char>{
		'1', '2', '3', '4', '5', '6', '7', '8', '9'
	};
	*/

	// ValueNumberInt
	// ValueNumberFraction
	// ValueNumberExponential
	// ValueNumberExponentialDigits

	/*
	number = [ minus ] int [ frac ] [ exp ]

		 decimal-point = %x2E	   ; .

		 digit1-9 = %x31-39		 ; 1-9

		 e = %x65 / %x45			; e E

		 exp = e [ minus / plus ] 1*DIGIT

		 frac = decimal-point 1*DIGIT

		 int = zero / ( digit1-9 *DIGIT )

		 minus = %x2D			   ; -

		 plus = %x2B				; +

		 zero = %x30				; 0
	*/
	
	// String constants
	private const char CHAR_ESCAPE = '\\';
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
		@"\\", // Reverse solidus (backslash)
		@"\/",  // Solidus (forward slash)
		@"\b", // Backspace
		@"\f", // Form feed
		@"\n", // Line feed
		@"\r", // Carriage return
		@"\t"  // Tab
	};

	// Encoding constants
	private static readonly string ENCODING_KEY_VALUE_SEPARATOR			= STRUCTURE_NAME_SEPARATOR.ToString();
	private static readonly string ENCODING_KEY_VALUE_SEPARATOR_PRETTY	= " " + STRUCTURE_NAME_SEPARATOR.ToString() + " ";
	private static readonly string ENCODING_LINE_SEPARATOR				= "";
	private static readonly string ENCODING_LINE_SEPARATOR_PRETTY		= "\r\n";
	private static readonly string ENCODING_INDENT						= "	";
	
	// Parsing states
	private enum ParsingType {
		Unknown,						// Starting parsing
		ValueObjectPreItem,				// An object started or an item delimiter was found, waiting for new item (beginning with key name)
		ValueObjectPostItem,			// An item inside an object ended, waiting for item delimiter (,) or end of object
		ValueArrayPreItem,				// An array started or an item delimiter was found, waiting for new item
		ValueArrayPostItem,				// An item inside an array ended, waiting for item delimiter (,) or end of array
		ValueString,					// In the middle of a string value, waiting for more chars or string delimiter
		ValueNumber,					// In the middle of a numeric value, waiting for more chars or end of number
		KeyName,						// In the middle of an item key (similar to string), waiting for more chars or string delimiter
		PostKeyName						// After a key name ended, waiting for delimiter (:)
	}


	// ================================================================================================================
	// PUBLIC STATIC INTERFACE ----------------------------------------------------------------------------------------

	public static object parse(string input, int flags = 0) {
		return decodeObject(input, flags).value;
	}

	public static T parseAs<T>(string input, int flags = 0) where T:new() {
		// Decodes an input as a target object
		return convertObject<T>(parse(input, flags));
	}

	public static Dictionary<string, object> parseAsDictionary(string input, int flags = 0) {
		return (Dictionary<string, object>)decodeObject(input, flags).value;
	}

	public static List<object> parseAsArray(string input, int flags = 0) {
		return (List<object>)decodeObject(input, flags).value;
	}

	public static string stringify(Dictionary<string, object> input, bool prettyPrint = false) {
		return encodeObject(input, prettyPrint);
	}


	// ================================================================================================================
	// INTERNAL STATIC INTERFACE --------------------------------------------------------------------------------------

	public static string encodeString(string input) {
		// Encodes a string properly, escaping chars that need escaping
		string t = "";
		int i;
		char c;
		int charPos;
		string unicodeNumber;

		for (i = 0; i < input.Length; i++) {
			c = input[i];
			//Debug.Log("CHAR => " + (int)c + " = [" + c + "]");
			charPos = CHARS_STRING_NEED_ESCAPE.IndexOf(c);
			if (charPos >= 0) {
				// Special char: needs to be escaped
				t += CHARS_STRING_ESCAPED[charPos];
			} else if ((int)c < 0x20) {
				// Outside the range: needs to be escaped
				unicodeNumber = ((int)c).ToString("X");
				t += STRING_ESCAPE_UNICODE + unicodeNumber.PadLeft(4, '0').ToLowerInvariant();
			} else if ((int)c >= 0xc200) {
				// Multi byte char
				// c2..fd means initial bytes of unicode, fe..ff means the char bytes themselves
				unicodeNumber = ((int)c).ToString("X");
				t += STRING_ESCAPE_UNICODE + unicodeNumber.PadLeft(4, '0').ToLowerInvariant();
			} else {
				// Normal char
				t += c;
			}
		}

		return t;
	}

	private static string encodeObject(object input, bool prettyPrint = true, int indentLevel = 0) {
		// Encodes a native object (boolean, string, number, List<>, Dictionary<>) as a string
		string txt = "";

		bool hasItemsInList = false;

		if (input is string || input is char) {
			// It's a String
			txt += STRUCTURE_STRING_DELIMITER + encodeString((string)input) + STRUCTURE_STRING_DELIMITER;
		} else if (input is sbyte || input is byte || input is short || input is ushort || input is int || input is uint || input is long || input is ulong || input is float || input is double || input is decimal) {
			// It's a Number
			txt += input.ToString();
		} else if (input is bool) {
			// It's a Boolean
			txt += (bool)input ? VALUE_LITERAL_TRUE : VALUE_LITERAL_FALSE;
		} else if (input == null) {
			// it's a null object
			txt += VALUE_LITERAL_NULL;
		} else if (input is List<object>) {
			// It's an array
			txt += STRUCTURE_BEGIN_ARRAY;
			foreach (object inputItem in (List<object>)input) {
				if (hasItemsInList) txt += STRUCTURE_VALUE_SEPARATOR;

				txt += prettyPrint ? ENCODING_LINE_SEPARATOR_PRETTY + getIndents(indentLevel + 1) : ENCODING_LINE_SEPARATOR;
				txt += encodeObject(inputItem, prettyPrint, indentLevel + 1);

				hasItemsInList = true;
			}

			txt += prettyPrint ? ENCODING_LINE_SEPARATOR_PRETTY + getIndents(indentLevel) : ENCODING_LINE_SEPARATOR;
			txt += STRUCTURE_END_ARRAY;
		} else {
			// It's an object
			txt += STRUCTURE_BEGIN_OBJECT;
			foreach(KeyValuePair<string, object> inputItem in (Dictionary<string, object>)input) {
				if (hasItemsInList) txt += STRUCTURE_VALUE_SEPARATOR;

				txt += prettyPrint ? ENCODING_LINE_SEPARATOR_PRETTY + getIndents(indentLevel + 1) : ENCODING_LINE_SEPARATOR;
				txt += STRUCTURE_STRING_DELIMITER + encodeString(inputItem.Key) + STRUCTURE_STRING_DELIMITER;
				txt += prettyPrint ? ENCODING_KEY_VALUE_SEPARATOR_PRETTY : ENCODING_KEY_VALUE_SEPARATOR;
				txt += encodeObject(inputItem.Value, prettyPrint, indentLevel + 1);

				hasItemsInList = true;
			}

			txt += prettyPrint ? ENCODING_LINE_SEPARATOR_PRETTY + getIndents(indentLevel) : ENCODING_LINE_SEPARATOR;
			txt += STRUCTURE_END_OBJECT;

			// TODO: make sure this works with any arbitrary object type!
		}

		return txt;
	}

	private static T convertObject<T>(object obj) where T:new() {
		return convertObject<T>(obj, new T());
	}

	private static T convertObject<T>(object obj, T item) {
		// Converts an object (dictionary or string) to a target type, recursively
		/*
		Debug.Log("===================> " + item + " has " + ((object)item).GetType().GetProperties().Length + " properties");
		for (int i = 0; i < item.GetType().GetProperties().Length; i++) {
			Debug.Log("===================> " + i + ": [" + item.GetType().GetProperties()[i].Name + "]");
		}
		*/

		if (obj is Dictionary<string, object>) {
			PropertyInfo propertyInfo;
			FieldInfo fieldInfo;
			foreach (KeyValuePair<string, object> entry in (Dictionary<string, object>)obj) {
				// Checks for properties (getter/setters pairs)
				propertyInfo = item.GetType().GetProperty(entry.Key);
				if (propertyInfo != null) {
					// This property exists in the destination type, set its values
					propertyInfo.SetValue(item, getValueForField(propertyInfo.GetValue(item, null), entry.Value, propertyInfo.PropertyType), null);
				} else {
					// Checks for fields (public vars)
					fieldInfo = item.GetType().GetField(entry.Key);
					if (fieldInfo != null) {
						// This field exists in the destination type, set its values
						fieldInfo.SetValue(item, getValueForField(fieldInfo.GetValue(item), entry.Value, fieldInfo.FieldType));
					}
				}
			}
			return item;
		} else if (obj is List<object>) {
			Debug.LogError("Lists are not parsed yet!");
			return default(T);
		} else {
			Debug.LogError("Type of object [" + obj + "] is unrecognized!");
			return default(T);
		}
	}

	private static object getValueForField(object targetField, object value, Type targetType) {
		// Convert an object into a value of a target type
		if (targetType.IsPrimitive || targetType == typeof(string) || targetType == typeof(decimal)) {
			// Is a primitive value type, so just set the new value without instantiating anything
			// Structs are value types, so we need to check for primitives and additional types
			return convertValueFieldToCorrectType(value, targetType);
		} else {
			// It's a reference type, so an instance need to be created or used
			if (targetField != null) {
				// An instance already exists, just fill it everything into it
				return convertObject(value, targetField);
			} else if (targetType.GetConstructor(Type.EmptyTypes) != null) {
				// The type has default constructor, just create and use it
				return convertObject(value, Activator.CreateInstance(targetType));
			} else {
				// No default constructor, nothing can be done
				Debug.LogError("Object needed to create a new field of type " + targetType + ", but it does not have a default constructor!");
			}
		}
		return null;
	}

	private static object convertValueFieldToCorrectType(object value, Type targetType) {
		// Convert a built-in type value (that can contain anything) to a given targetType
		if (targetType == typeof(bool)) {
			return Convert.ToBoolean(value);
		} else if (targetType == typeof(byte)) {
			return Convert.ToByte(value);
		} else if (targetType == typeof(sbyte)) {
			return Convert.ToSByte(value);
		} else if (targetType == typeof(char)) {
			return Convert.ToChar(value);
		} else if (targetType == typeof(string)) {
			return Convert.ToString(value);
		} else if (targetType == typeof(short)) {
			return Convert.ToInt16(value);
		} else if (targetType == typeof(int)) {
			return Convert.ToInt32(value);
		} else if (targetType == typeof(long)) {
			return Convert.ToInt64(value);
		} else if (targetType == typeof(ushort)) {
			return Convert.ToUInt16(value);
		} else if (targetType == typeof(uint)) {
			return Convert.ToUInt32(value);
		} else if (targetType == typeof(ulong)) {
			return Convert.ToUInt64(value);
		} else if (targetType == typeof(float)) {
			return Convert.ToSingle(value);
		} else if (targetType == typeof(double)) {
			return Convert.ToDouble(value);
		} else if (targetType == typeof(decimal)) {
			return Convert.ToDecimal(value);
		} else {
			Debug.LogWarning("Could not convert type: propertyType [" + targetType + "] unknown!");
			return null;
		}
		// ToDate ?
	}
	
	private static ParsedJSONValue decodeObject(string input, int flags = 0) {
		// Decodes a string object into a ParsedJSONValue containing an array, a string, a number, of a literal
		int i;
		char c;

		if ((flags & FLAG_REMOVE_COMMENTS) == FLAG_REMOVE_COMMENTS) {
			// Remove the comment and reset the flag
			flags -= FLAG_REMOVE_COMMENTS;
			input = Regex.Replace(input, "/\\*[\\w\\W]*?\\*/", "");
		}

		ParsedJSONValue returnObject = new ParsedJSONValue();

		ParsingType parsingType = ParsingType.Unknown;
		object parsingObject = null;
		string parsingObjectString = "";
		string parsingName = "";

		ParsedJSONValue parsedObject;

		i = 0;
		
		//Debug.Log("Decoding Object ------");

		bool mustEnd = false;

		while (i < input.Length && !mustEnd) {
			c = input[i];

			switch (parsingType) {
				case ParsingType.Unknown:
					//Debug.Log("STATE: UNKNOWN");

					parsingObject = null;
					parsingObjectString = "";

					if (c == STRUCTURE_BEGIN_OBJECT) {
						// Starting object
						//Debug.Log("-> starting object @ " + i);
						parsingType = ParsingType.ValueObjectPreItem;
						parsingObject = new Dictionary<string, object>();
					} else if (c == STRUCTURE_BEGIN_ARRAY) {
						// Starting array
						//Debug.Log("-> starting array @ " + i);
						parsingType = ParsingType.ValueArrayPreItem;
						parsingObject = new List<object>();
					} else if (c == STRUCTURE_STRING_DELIMITER) {
						// Starting string
						//Debug.Log("-> starting string @ " + i);
						parsingType = ParsingType.ValueString;
						parsingObjectString = "";
					} else if (CHARS_NUMBER.Contains(c)) {
						// Starting number
						//Debug.Log("-> starting number @ " + i);
						parsingType = ParsingType.ValueNumber;
						parsingObjectString = c.ToString();
					} else if (c == VALUE_LITERAL_NULL[0] && compareStringValue(VALUE_LITERAL_NULL, input, i)) {
						// Starting "null"
						//Debug.Log("-> starting null @ " + i);
						i += VALUE_LITERAL_NULL.Length - 1;
						mustEnd = true;
						returnObject.value = null;
						returnObject.length = i + VALUE_LITERAL_NULL.Length;
					} else if (c == VALUE_LITERAL_TRUE[0] && compareStringValue(VALUE_LITERAL_TRUE, input, i)) {
						// Starting "true"
						//Debug.Log("-> starting boolean true @ " + i);
						i += VALUE_LITERAL_TRUE.Length - 1;
						mustEnd = true;
						returnObject.value = true;
						returnObject.length = i + VALUE_LITERAL_TRUE.Length;
					} else if (c == VALUE_LITERAL_FALSE[0] && compareStringValue(VALUE_LITERAL_FALSE, input, i)) {
						// Starting "false"
						//Debug.Log("-> starting boolean false @ " + i);
						i += VALUE_LITERAL_FALSE.Length - 1;
						mustEnd = true;
						returnObject.value = false;
						returnObject.length = i + VALUE_LITERAL_FALSE.Length;
					}
					break;
				case ParsingType.ValueString:
					//Debug.Log("STATE: VALUE STRING");
					if (c == STRUCTURE_STRING_DELIMITER) {
						// Ended string
						//Debug.Log("-> ending string [" + parsingObjectString + "] @ " + i);
						mustEnd = true;
						returnObject.value = parsingObjectString;
						returnObject.length = i+1;
					} else if (compareStringValue(STRING_ESCAPE_UNICODE, input, i)) {
						//Debug.Log("-> continuing string with unicode char @ " + i);
						// Unicode encoded character
						i++;
						if (i < input.Length - 5) {
							parsingObjectString += Char.ConvertFromUtf32(Convert.ToInt32(input.Substring(i+1, 4), 16));
							i += 4;
						}
					} else if (c == CHAR_ESCAPE) {
						// Some special character
						//Debug.Log("-> continuing string @ " + i);
						if (i < input.Length - 1) {
							string nc = c.ToString() + input[i+1];
							int charPos = CHARS_STRING_ESCAPED.IndexOf(nc);
							if (charPos >= 0) {
								parsingObjectString += CHARS_STRING_NEED_ESCAPE[charPos];
								i++;
							} else {
								//Debug.LogError("Error! Escape string without equivalent char!");
							}
						}
					} else {
						// Continued string
						parsingObjectString += c;
					}
					break;
				case ParsingType.ValueNumber:
					//Debug.Log("STATE: VALUE NUMBER");
					if (CHARS_NUMBER.Contains(c)) {
						// Continued number
						//Debug.Log("-> continuing number @ " + i);
						parsingObjectString += c;
						break;
					} else {
						// Ended number
						//Debug.Log("-> ending number [" + parsingObjectString + "] @ " + i);
						mustEnd = true;
						returnObject.value = Convert.ToDouble(parsingObjectString);
						returnObject.length = i;
					}
					break;
				case ParsingType.ValueObjectPreItem:
					//Debug.Log("STATE: VALUE OBJECT PRE ITEM");
					if (c == STRUCTURE_END_OBJECT) {
						// Empty object?
						parsingType = ParsingType.ValueObjectPostItem;
						i--;
					} else if (c == STRUCTURE_STRING_DELIMITER) {
						// Starting a key name
//								trace ("  --> starting a key name @ ",__input.Length-i);
						parsingName = "";
						parsingType = ParsingType.KeyName;
					}
					break;
				case ParsingType.ValueObjectPostItem:
					//Debug.Log("STATE: VALUE OBJECT POST ITEM");
					if (c == STRUCTURE_VALUE_SEPARATOR) {
						// Starting a new object
						parsingType = ParsingType.ValueObjectPreItem;
					} else if (c == STRUCTURE_END_OBJECT) {
						// Ending object
						mustEnd = true;
						returnObject.value = parsingObject;
						returnObject.length = i+1;
					}
					break;
				case ParsingType.ValueArrayPreItem:
					//Debug.Log("STATE: VALUE ARRAY PRE ITEM");
					if (c == STRUCTURE_END_ARRAY) {
						// Empty array?
						//Debug.Log("-> ending empty array @ " + i);
						parsingType = ParsingType.ValueArrayPostItem;
						i--;
					} else if (CHARS_WHITESPACE.Contains(c)) {
						// Whitespace elements
					} else {
						//Debug.Log("-> getting array item @ " + i);

						// Value that must be added to this object
						// Everything that comes after is a new value that must be added to this object
						parsedObject = decodeObject(input.Substring(i), flags);

						i += parsedObject.length-1;
						(parsingObject as List<object>).Add(parsedObject.value);

						parsingType = ParsingType.ValueArrayPostItem;
					}
					break;
				case ParsingType.ValueArrayPostItem:
					//Debug.Log("STATE: VALUE ARRAY POST ITEM");
					if (c == STRUCTURE_VALUE_SEPARATOR) {
						// Starting a new object
						//Debug.Log("-> starting array @ " + i);
						parsingType = ParsingType.ValueArrayPreItem;
					} else if (c == STRUCTURE_END_ARRAY) {
						// Ending array
						//Debug.Log("-> ending array @ " + i);
						mustEnd = true;

						returnObject.value = parsingObject;
						returnObject.length = i+1;
					}
					break;
				case ParsingType.KeyName:
					//Debug.Log("STATE: KEY NAME");
					if (c == STRUCTURE_STRING_DELIMITER) {
						// Ending a key name
						//Debug.Log("-> ending key name [" + parsingName + "] @ " + i);
						parsingType = ParsingType.PostKeyName;
					} else {
						// Continuing the key name
						//Debug.Log("-> continuing key name @ " + i);
						parsingName += c;
					}
					break;
				case ParsingType.PostKeyName:
					//Debug.Log("STATE: POST KEY NAME");
					if (c == STRUCTURE_NAME_SEPARATOR) {
						//Debug.Log("-> found key name separator (:) @ " + i);

						// Find value that must be added to this object
						parsedObject = decodeObject(input.Substring(i + 1), flags);

						i += parsedObject.length-1;
						((Dictionary<string, object>)parsingObject)[parsingName] = parsedObject.value;

						parsingType = ParsingType.ValueObjectPostItem;
						parsingName = null;
					}
					break;
			}

			i++;
		}

		return returnObject;
	}

	protected static bool compareStringValue(string valueToSearch, string fullText, int position = 0) {
		return valueToSearch.Length <= fullText.Length - position && fullText.Substring(position, valueToSearch.Length) == valueToSearch;
	}

	protected static string getIndents(int indentLevel) {
		string txt = "";
		while (indentLevel-- > 0) txt += ENCODING_INDENT;
		return txt;
	}


	// ================================================================================================================
	// INTERNAL CLASSES -----------------------------------------------------------------------------------------------

	class ParsedJSONValue {
		public object value;
		public int length;

		public ParsedJSONValue() {
			value = null;
			length = 0;
		}
	}
}


