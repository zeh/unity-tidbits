using System.Text;

/**
 * A value can be a string in double quotes, or a number, or true or false or null, or an object or an array.
 * These structures can be nested.
 * @author zeh
 */
public abstract class JSONValue {

	// Properties
	protected int inputStart;
	protected int inputLength;
	protected StringBuilder input;


	// ================================================================================================================
	// CONSTRUCTOR ----------------------------------------------------------------------------------------------------

	public JSONValue() {
	}

	public JSONValue(object value) {
		setValue(value);
	}

	public JSONValue(StringBuilder input, int position) {
		parseInput(input, position);
	}


	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	public void parseInput(StringBuilder parseInput, int parseInputStart) {
		input = parseInput;
		inputStart = parseInputStart;
		parseValueFromInput();
	}

	public string ToString(int indentLevel, JSONStringifyOptions options) {
		return stringifyValue(indentLevel, options);
	}

	public abstract object getValue();

	public abstract void setValue(object newValue);

	public int getInputLength() {
		return inputLength;
	}


	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

	protected abstract void parseValueFromInput();

	protected abstract string stringifyValue(int indentLevel, JSONStringifyOptions options);
}