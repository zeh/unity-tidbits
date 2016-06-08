public class JSONStringifyOptions {

	public string individualIndent;
	public string lineFeed;
	public string arrayAfterSeparator;
	public string objectAfterKey;
	public string objectBeforeValue;
	public bool lineFeedOnArrays;


	// ================================================================================================================
	// CONSTRUCTOR ----------------------------------------------------------------------------------------------------

	public JSONStringifyOptions() {
	}

	// ================================================================================================================
	// STATIC INTERFACE -----------------------------------------------------------------------------------------------

	public static JSONStringifyOptions getCompact() {
		var o = new JSONStringifyOptions();
		return o;
	}

	public static JSONStringifyOptions getPretty() {
		var o = new JSONStringifyOptions();
		o.individualIndent = "\t";
		o.lineFeed = "\n";
		o.arrayAfterSeparator = " ";
		o.lineFeedOnArrays = true;
		o.objectAfterKey = " ";
		o.objectBeforeValue = " ";
		return o;
	}
}
