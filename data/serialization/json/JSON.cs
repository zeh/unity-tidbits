using System;
using System.Collections;
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
	* Allow max depth
	* Proper number parse?
	* Create a strong typed List<> when an array of a specific type is identified
	* Escape everything below 0x0020, or above 0x10ffffff
	* Decide on NaN, Infinity
	* Decide on key strings without quotes
	* Do stringify() on any object
	* Make encoding faster with string buffers instead
	* Auto encoder that understands vectors too?
	* Numbers are always decoded (from string) to double.. automatically detect format?
	*/

	/*
	// GSON examples:
	// Serialization
	Gson gson = new Gson();
	gson.toJson(1);            ==> prints 1
	gson.toJson("abcd");       ==> prints "abcd"
	gson.toJson(new Long(10)); ==> prints 10
	int[] values = { 1 };
	gson.toJson(values);       ==> prints [1]
	gson.toJson(new BagOfPrimitives(););  // {"value1":1,"value2":"abc"}

	int[] ints = {1, 2, 3, 4, 5};
	gson.toJson(ints);     ==> prints [1,2,3,4,5]

	String[] strings = {"abc", "def", "ghi"};
	gson.toJson(strings);  ==> prints ["abc", "def", "ghi"]

	// Deserialization
	int one = gson.fromJson("1", int.class);
	Integer one = gson.fromJson("1", Integer.class);
	Long one = gson.fromJson("1", Long.class);
	Boolean false = gson.fromJson("false", Boolean.class);
	String str = gson.fromJson("\"abc\"", String.class);
	String anotherStr = gson.fromJson("[\"abc\"]", String.class);
	BagOfPrimitives obj2 = gson.fromJson(json, BagOfPrimitives.class);    ==> obj2 is just like obj
	int[] ints2 = gson.fromJson("[1,2,3,4,5]", int[].class)
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

	/*
	// Configuration flags
	// Cannot be enum... makes the comparison too verbose later
	private static readonly bool USE_SINGLE_TYPE_LIST = false;					// Attempt to create arrays of single types by checking all properties inside
	*/
	public static readonly int FLAG_REMOVE_COMMENTS = 1;
	/*

	// ================================================================================================================
	// PUBLIC STATIC INTERFACE ----------------------------------------------------------------------------------------
	*/

	public static IList parseAsArray(string input, int flags = 0) {
		return (IList)parse(input, flags);
	}

	public static List<T> parseAsArray<T>(string input, int flags = 0) {
		return (List<T>)parse(input, flags);
	}

	public static Dictionary<string, object> parseAsDictionary(string input, int flags = 0) {
		return (Dictionary<string, object>)parse(input, flags);
	}

	public static T parseAs<T>(string input, int flags = 0) where T:new() {
		return convertObject<T>(parse(input, flags));
	}

	public static object parse(string input, int flags = 0) {
		if ((flags & FLAG_REMOVE_COMMENTS) == FLAG_REMOVE_COMMENTS) {
			// Remove comments and reset the flag
			flags -= FLAG_REMOVE_COMMENTS;
			input = Regex.Replace(input, "/\\*[\\w\\W]*?\\*/", "");
		}

		JSONValue jsonValue = JSONParser.parse(input);
		if (jsonValue != null) return jsonValue.getValue();
		return null;
	}

	public static string stringify(string input) {
		return JSONEncoder.encode(input, false);
	}

	public static string stringify(float input) {
		return JSONEncoder.encode(input, false);
	}

	public static string stringify(int input) {
		return JSONEncoder.encode(input, false);
	}

	public static string stringify(double input) {
		return JSONEncoder.encode(input, false);
	}

	public static string stringify(long input) {
		return JSONEncoder.encode(input, false);
	}

	public static string stringify(bool input) {
		return JSONEncoder.encode(input, false);
	}

	public static string stringify(IList input, bool prettyPrint = false) {
		return JSONEncoder.encode(input, prettyPrint);
	}

	public static string stringify(IDictionary input, bool prettyPrint = false) { // TODO: prettyPrint should be false by default... but it conflicts with the string version
		return JSONEncoder.encode(input, prettyPrint);
	}


	// ================================================================================================================
	// INTERNAL STATIC INTERFACE --------------------------------------------------------------------------------------

	private static T convertObject<T>(object obj) where T:new() {
		return convertObject<T>(obj, new T());
	}

	private static T convertObject<T>(object obj, T item) {
		// Converts an object (dictionary, string or list) to a target type, recursively

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
			if (item is IList) {
				// Destination is list, can set
				PropertyInfo propertyInfo = item.GetType().GetProperty("Item");

				IList itemAsList = item as IList;
				itemAsList.Clear();
				foreach (object entryValue in obj as List<object>) {
					itemAsList.Add(getValueForField(null, entryValue, propertyInfo.PropertyType));
				}
			}
			return item;
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
				// An instance already exists, just fill everything into it
				return convertObject(value, targetField);
			} else if (targetType.GetConstructor(Type.EmptyTypes) != null) {
				// The type has default constructor, just create and use it
				// TODO: special case for Vector2
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

	/*
	
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

						// Create a list of a single type, if possible
						if (USE_SINGLE_TYPE_LIST) {
							Type singleType = getSingleListType(parsingObject as IList);
							if (singleType != null) {
								// List is of a single type, create a list
								parsingObject = convertListType(parsingObject as IList, singleType);
							}
							Debug.LogWarning("Created list of single type " + singleType);
						}

						returnObject.value = parsingObject;
						returnObject.length = i+1;
					}
					break;
				case ParsingType.KeyName:
					--
					break;
				case ParsingType.PostKeyName:
					--
					break;
			}

			i++;
		}

		//Debug.Log("Returning object ------ [" + returnObject.value + "] with length [" + returnObject.length + "]");

		return returnObject;
	}

	protected static string getIndents(int indentLevel) {
		StringBuilder builder = new StringBuilder(ENCODING_INDENT.Length * indentLevel);
		for (int i = 0; i < indentLevel; i++) builder.Append(ENCODING_INDENT);
		return builder.ToString();
	}

	private static Type getSingleListType(IList list) {
		// Try to find the only type in a list
		Type lastType = null;
		for (int i = 0; i < list.Count; i++) {
			if (lastType == null) {
				// First type
				lastType = list[i].GetType();
			} else {
				if (list[i].GetType() != lastType) {
					// Different type
					return null;
				}
			}
		}
		Debug.Log("SINGLE TYPE IS: " + lastType);
		return lastType;
	}

	private static IList convertListType(IList list, Type newType) {
		Debug.Log("casting list from [object] to [" + newType + "]");
		var newList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(newType));
		Debug.Log("new list is " + newList);
		for (int i = 0; i < list.Count; i++) {
			newList.Add(list[i]);
		}
		return newList;
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
	*/
}