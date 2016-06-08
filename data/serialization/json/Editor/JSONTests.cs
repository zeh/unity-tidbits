using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

/**
 * @author zeh
 */
[TestFixture]
public class JSONTests {

	/*
	 * A Assert.AreEqualfor the JSON class
	 *
	 * Uses NUnit: http://www.nunit.org/index.php?p=quickStart&r=2.6.4
	 *
	 * Assert.AreEqual( int expected, int actual, string message );
	 * Assert.AreEqual( int expected, int actual );
	 * Assert.AreSame( object expected, object actual );
	 * Assert.Contains( object anObject, IList collection );
	 * Assert.AreNotSame( object expected, object actual );
	 * Assert.Greater( int arg1, int arg2 );
	 * Assert.GreaterOrEqual( int arg1, int arg2 );
	 * Assert.Less( int arg1, int arg2 );
	 * Assert.IsInstanceOf<T>( object actual );
	 * Assert.IsNotInstanceOf<T>( object actual );
	 * Assert.IsAssignableFrom<T>( object actual );
	 * Assert.IsNotAssignableFrom<T>( object actual );
	 * Assert.IsTrue( bool condition );
	 * Assert.IsFalse( bool condition);
	 * Assert.IsNull( object anObject );
	 * Assert.IsNotNull( object anObject );
	 * Assert.IsEmpty( string aString ); // Or ICollection
	 * Assert.IsNotEmpty( string aString ); // Or ICollection
	 */

	/*
	 * TODO:
	 * * Lists into lists of unique types
	 * Test:
	 * * Use heavy JSON from JSON test suite: https://code.google.com/p/json-test-suite/
	 * * Test decoding of wrong types
	 */

	 /*
	 		//testStrings();
		//testNumbers();
		//testLiterals();
		//testArrays();
		//testJSONEncoding();
		//testJSONDecoding();
		testJSONDecodingObject();
*/

	// ================================================================================================================
	// SETUP ----------------------------------------------------------------------------------------------------------

	[TestFixtureSetUp]
	public void SetUp() {
	}

	[TestFixtureTearDown]
	public void TearDown() {
	}


	// ================================================================================================================
	// TEST INTERFACE -------------------------------------------------------------------------------------------------

	[Test]
	public void String_Encoding() {
		string q = "\"";

		Assert.AreEqual(JSON.stringify("aaaa"),							q+"aaaa"+q,									"Simple string");
		Assert.AreEqual(JSON.stringify(@"éúPŐ\u0123źŻ"),				q+@"éúPŐ\\u0123źŻ"+q,						"Simple string with unicode");
		Assert.AreEqual(JSON.stringify("\ud834\udd1e"),					q+@"\ud834\udd1e"+q,						"Simple string with unicode G clef");
		Assert.AreEqual(JSON.stringify("aa\"aa"),						q+"aa\\\"aa"+q,								"Simple string with quotes");
		Assert.AreEqual(JSON.stringify("aaa{a}a:a[a]a"), 				q+"aaa{a}a:a[a]a"+q,						"Simple string with special chars");
		Assert.AreEqual(JSON.stringify("a\"a\\a/a\ba\fa\na\ra\ta"),		q+"a\\\"a\\\\a\\/a\\ba\\fa\\na\\ra\\ta"+q,	"Simple string with all escaped chars");
		Assert.AreEqual(JSON.stringify(@"a\""a\a/a\ba\fa\na\ra\ta"),	q+@"a\\\""a\\a\/a\\ba\\fa\\na\\ra\\ta"+q,	"Simple string with unescaped chars");
	}

	[Test]
	public void String_Decoding() {
		string q = "\"";

		Assert.AreEqual((string)JSON.parse(q+"aaaa"+q),									"aaaa",							"Simple string");
		Assert.AreEqual((string)JSON.parse(q+@"éúPŐ\\u0123źŻ"+q),						@"éúPŐ\u0123źŻ",				"Simple string with unicode");
		Assert.AreEqual((string)JSON.parse(q+@"\ud834\udd1e"+q),						"\ud834\udd1e",					"Simple string with unicode G clef");
		Assert.AreEqual((string)JSON.parse(q+"aa\\\"aa"+q),								"aa\"aa",						"Simple string with quotes");
		Assert.AreEqual((string)JSON.parse(q+"aaa{a}a:a[a]a"+q),						"aaa{a}a:a[a]a",				"Simple string with special chars");
		Assert.AreEqual((string)JSON.parse(q+"a\\\"a\\\\a\\/a\\ba\\fa\\na\\ra\\ta"+q),	"a\"a\\a/a\ba\fa\na\ra\ta",		"Simple string with all escaped chars");
		Assert.AreEqual((string)JSON.parse(q+@"a\\\""a\\a\/a\\ba\\fa\\na\\ra\\ta"+q),	@"a\""a\a/a\ba\fa\na\ra\ta",	"Simple string with unescaped chars");
	}

	[Test]
	public void Number_Encoding() {
		Assert.AreEqual(JSON.stringify(1), "1");
		Assert.AreEqual(JSON.stringify(0), "0");
		Assert.AreEqual(JSON.stringify(-200), "-200");
		Assert.AreEqual(JSON.stringify(-200.0), "-200");
		Assert.AreEqual(JSON.stringify(120.1323), "120.1323");
		Assert.AreEqual(JSON.stringify(120.0f), "120");
		Assert.AreEqual(JSON.stringify(1201323L), "1201323");
	}

	[Test]
	public void Number_Decoding() {
		Assert.AreEqual(1, JSON.parse("1"));
		Assert.AreEqual(0, JSON.parse("0"));
		Assert.AreEqual(-200, JSON.parse("-200"));
		Assert.AreEqual(-200.0, JSON.parse("-200"));
		Assert.AreEqual(120.1323, JSON.parse("120.1323"));
		Assert.AreEqual(120.1323f, JSON.parse("120.1323"));
		Assert.AreEqual(1201323L, JSON.parse("1201323"));
	}

	[Test]
	public void Literal_Encoding() {
		Assert.AreEqual(JSON.stringify((string)null), "null");
		Assert.AreEqual(JSON.stringify(true), "true");
		Assert.AreEqual(JSON.stringify(false), "false");
	}

	[Test]
	public void Literal_Decoding() {
		Assert.IsNull(JSON.parse("null"));
		Assert.IsTrue((bool)JSON.parse("true"));
		Assert.IsFalse((bool)JSON.parse("false"));
	}

	[Test]
	public void Array_Encoding() {
		var intList = new List<int> { 1, 2 };
		var floatList = new List<float> { 1.0f, 2.2f };
		var stringList = new List<string> { "one", "two" };
		var mixedList = new List<object> { 1, "one", 4.5 };

		Assert.AreEqual(JSON.stringify(intList),	"[1,2]",				"int list");
		Assert.AreEqual(JSON.stringify(floatList),	"[1,2.2]",				"float list");
		Assert.AreEqual(JSON.stringify(stringList),	"[\"one\",\"two\"]",	"string list");
		Assert.AreEqual(JSON.stringify(mixedList),	"[1,\"one\",4.5]",		"mixed list");
	}

	[Test]
	public void Array_Decoding() {
		var intList = new List<int> { 1, 2 };
		var floatList = new List<float> { 1.0f, 2.2f };
		var stringList = new List<string> { "one", "two" };
		var mixedList = new List<object> { 1, "one", 4.5 };

		Assert.AreEqual((IList)JSON.parse("[1,2]"),				intList,		"int list");
		Assert.AreEqual((IList)JSON.parse("[1,2.2]"),			floatList,		"float list");
		Assert.AreEqual((IList)JSON.parse("[\"one\",\"two\"]"),	stringList,		"string list");
		Assert.AreEqual((IList)JSON.parse("[1,\"one\",4.5]"),	mixedList,		"mixel list");
	}

	[Test]
	public void Map_Encoding() {
		// Create object
		Dictionary<string, object> jsonObject = new Dictionary<string, object>();
		jsonObject["string"] = "astring";
		jsonObject["int"] = 10;
		jsonObject["float"] = 10.5f;
		jsonObject["double"] = 10.5;
		jsonObject["true"] = true;
		jsonObject["false"] = false;
		jsonObject["null"] = null;
		jsonObject["obj"] = new Dictionary<string, object>();
		((Dictionary<string, object>)jsonObject["obj"])["string"] = "astring";
		jsonObject["array"] = new List<object>();
		((List<object>)jsonObject["array"]).Add("astring");
		((List<object>)jsonObject["array"]).Add(10);
		((List<object>)jsonObject["array"]).Add(10.5f);
		((List<object>)jsonObject["array"]).Add(10.5);
		((List<object>)jsonObject["array"]).Add(true);
		((List<object>)jsonObject["array"]).Add(false);
		((List<object>)jsonObject["array"]).Add(null);

		// Array
		List<object> jsonArray = new List<object>();
		jsonArray.Add("astring");
		jsonArray.Add(new Dictionary<string, object>());
		((Dictionary<string, object>)jsonArray[1])["string"] = "bstring";

        string jsonObjectEncodedTarget = @"{""string"":""astring"",""int"":10,""float"":10.5,""double"":10.5,""true"":true,""false"":false,""null"":null,""obj"":{""string"":""astring""},""array"":[""astring"",10,10.5,10.5,true,false,null]}";

        string jsonObjectEncodedTargetPretty = @"{
	""string"" : ""astring"",
	""int"" : 10,
	""float"" : 10.5,
	""double"" : 10.5,
	""true"" : true,
	""false"" : false,
	""null"" : null,
	""obj"" : {
		""string"" : ""astring""
	},
	""array"" : [
		""astring"",
		10,
		10.5,
		10.5,
		true,
		false,
		null
	]
}";

        Assert.AreEqual(JSON.stringify(jsonObject, true), jsonObjectEncodedTargetPretty, "pretty encode");
		Assert.AreEqual(JSON.stringify(jsonObject, false), jsonObjectEncodedTarget, "inline encode");
	}

	[Test]
	public void Map_Decoding_Simple() {

		string encodedJSON = @"{
			""Image"": {
				""Width"": 800,
				""Height"": 600,
				""Title"": ""View from 15th Floor"",
				""Thumbnail"": {
					""Url"": ""http://www.example.com/image/481989943"",
					""Height"": 125,
					""Width"": ""100""
				},
				""IDs"": [116, 943, 234, 38793]
			}
		}";

        //Debug.Log("["+encodedJSON+"]");

		Dictionary<string, object> decodedJSON = JSON.parseAsDictionary(encodedJSON);
		Dictionary<string, object> imgObj = (Dictionary<string, object>)decodedJSON["Image"];

		Assert.AreEqual(imgObj["Width"], 800, "obj.int: image width");
		Assert.AreEqual(imgObj["Title"], "View from 15th Floor", "obj.string: image title");
		Assert.AreEqual((string)((Dictionary<string, object>)imgObj["Thumbnail"])["Url"], @"http://www.example.com/image/481989943", "obj.obj.string: image thumb url");
		Assert.AreEqual(((List<object>)imgObj["IDs"])[1], 943, "obj.array.index: image id");

		encodedJSON = @"[{
			""precision"": ""zip"",
			""Latitude"": 37.7668,
			""Longitude"": -122.3959,
			""Address"": """",
			""City"": ""SAN FRANCISCO"",
			""State"": ""CA"",
			""Zip"": ""94107"",
			""Country"": ""US""
		}, {
			""precision"": ""zip"",
			""Latitude"": 37.371991,
			""Longitude"": -122.026020,
			""Address"": """",
			""City"": ""SUNNYVALE"",
			""State"": ""CA"",
			""Zip"": ""94085"",
			""Country"": ""US""
		}]";

        //Debug.Log("["+encodedJSON+"]");

		List<object> decodedJSONArray = JSON.parseAsArray<object>(encodedJSON);
		Dictionary<string, object> obj0 = (Dictionary<string, object>)decodedJSONArray[0];
		Dictionary<string, object> obj1 = (Dictionary<string, object>)decodedJSONArray[1];

		Assert.AreEqual(obj0["precision"], "zip", "array.obj.string");
		Assert.AreEqual(""+obj0["Latitude"], ""+37.7668f, "array.obj.float");
		Assert.AreEqual(""+obj0["Longitude"], ""+(-122.3959f), "array.obj.float: negative float");
		Assert.AreEqual(obj0["Address"], "", "array.obj.string: empty string");
		Assert.AreEqual(obj1["City"], "SUNNYVALE", "array[1].obj.string: city");
	}

	/*
	private static void testJSONDecodingObject() {
		Assert.AreEqualstartGroup("Object decode");

		string encodedObject = @"{""Width"":800,""Title"":""The title""}";

		object obj = JSON.parse(encodedObject);
		Dictionary<string, object> dict = (Dictionary<string, System.Object>) obj;

		Assert.AreEqual("obj.int", dict["Width"], 800);
		Assert.AreEqual("obj.string", dict["Title"], "The title");

		encodedObject = @"{
			""Width"" : 800,
			""Title"" : ""The title""
		}";

		obj = JSON.parse(encodedObject);
		dict = (Dictionary<string, object>) obj;

		Assert.AreEqual("obj.int", dict["Width"], 800);
		Assert.AreEqual("obj.string", dict["Title"], "The title");

		Assert.AreEqualendGroup();
	}
	*/

	[Test]
	public void Array_Decoding_Complex() {
		string encodedObject = @"{
			""Width"": 800,
			""Height"": 600,
			""Title"": ""The title"",
			""Thumbnail"": {
				""Url"": ""http://www.example.com/image/123"",
				""Height"": 125,
				""Width"": ""100""
			},
			""Thumbnail2"": {
				""Url"": ""http://www.example.com/image/124"",
				""Height"": 225,
				""Width"": ""200""
			},
			""IDs"": [116, 943, 234, 38793],
			""IDsExisting"": [116, 943, 234, 38793],
			""IDsGS"": [116, 943, 234, 38793],
			""vec"": {""x"": 10, ""y"": 20},
			""vecs"": [{""x"": 10, ""y"": 20}, {""x"": 30, ""y"": 40}],
			""thumbs"": [{""Url"": ""http://www.example.com/image/124"", ""Height"": 225, ""Width"": ""200""},{""Url"": ""http://www.example.com/image/222"", ""Height"": 1, ""Width"": ""2""}]
		}";

		TestObject obj = JSON.parseAs<TestObject>(encodedObject);

		Assert.AreEqual(obj.Width, 800, "obj.int");
		Assert.AreEqual(obj.Height, 600, "obj.int(G/S)");
		Assert.AreEqual(obj.Title, "The title", "obj.string");
		Assert.IsTrue(obj.Thumbnail != null && obj.Thumbnail.Url == "http://www.example.com/image/123", "obj.obj.string");
		Assert.IsTrue(obj.Thumbnail2 != null && obj.Thumbnail2.Height == 225, "obj.obj(G/S).int");
		Assert.AreEqual(obj.vec.x, 10, "obj.vector.x");
		//Assert.IsTrue(obj.vecs != null && obj.vecs.Count == 2 && obj.vecs[1].y == 40, "obj.List<vector>.y");
		Assert.IsTrue(obj.IDs != null && obj.IDs.Count > 1 && obj.IDs[1] == 943, "obj.List<int>.[index] (new)");
		Assert.IsTrue(obj.IDs != null && obj.IDs.Count == 4, "obj.List<int>.count");
		Assert.IsTrue(obj.IDsExisting != null && obj.IDsExisting.Count > 1 && obj.IDsExisting[1] == 943, "obj.List<int>.[index] (existing)");
		Assert.AreEqual(obj.IDsGS[1], 943, "obj.List<int>(G/S).[index]");
		Assert.IsTrue(obj.thumbs != null && obj.thumbs.Count == 2 && obj.thumbs[1].Width == "2", "obj.List<obj>.[index]");
	}

	// ================================================================================================================
	// TEST CLASSES ---------------------------------------------------------------------------------------------------

	public class TestObject {
		public int Width;
		private int _Height; // Property equivalent
		public string Title;
		public TestThumbnailObject Thumbnail;
		private TestThumbnailObject _Thumbnail2;
		public List<int> IDs;
		public List<int> IDsExisting = new List<int>();
		private List<int> _IDsGS; // Property equivalent
		public Vector2 vec;
		//public List<Vector2> vecs;
		public List<TestThumbnailObject> thumbs;

		public int Height {
			get { return _Height; }
			set { _Height = value; }
		}
		public TestThumbnailObject Thumbnail2 {
			get { return _Thumbnail2; }
			set { _Thumbnail2 = value; }
		}
		public List<int> IDsGS {
			get { return _IDsGS; }
			set { _IDsGS = value; }
		}
	}

	public class TestThumbnailObject {
		public string Url;
		public int Height;
		public String Width;
	}
}