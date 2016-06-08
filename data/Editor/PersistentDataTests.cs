using NUnit.Framework;

[TestFixture]
public class PersistentDataTests {

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
	public void Primitives() {
		// Saving
		var oldData = PersistentData.getInstance();
		oldData.Clear();

		var b0 = true;
		var b1 = false;
		var i0 = 2;
		var i1 = -1020;
		var i2 = 9081720;
		var l0 = 21983671920L;
		var l1 = -21983671222L;
		var f0 = 2.2f;
		var f1 = -2.3f;
		var d0 = -2.0928;
		var d1 = 655352.8762;
		var s0 = "aaaaaa";
		var s1 = @"a\naa""a\\aa";

		oldData.SetBool("b0", b0);
		oldData.SetBool("b1", b1);
		oldData.SetInt("i0", i0);
		oldData.SetInt("i1", i1);
		oldData.SetInt("i2", i2);
		oldData.SetLong("l0", l0);
		oldData.SetLong("l1", l1);
		oldData.SetFloat("f0", f0);
		oldData.SetFloat("f1", f1);
		oldData.SetDouble("d0", d0);
		oldData.SetDouble("d1", d1);
		oldData.SetString("s0", s0);
		oldData.SetString("s1", s1);
		oldData.Save();

		// Loading
		var newData = PersistentData.getInstance();

		// Test
		Assert.AreEqual(b0, newData.GetBool("b0"));
		Assert.AreEqual(b1, newData.GetBool("b1"));
		Assert.AreEqual(i0, newData.GetInt("i0"));
		Assert.AreEqual(i1, newData.GetInt("i1"));
		Assert.AreEqual(i2, newData.GetInt("i2"));
		Assert.AreEqual(l0, newData.GetLong("l0"));
		Assert.AreEqual(l1, newData.GetLong("l1"));
		Assert.AreEqual(f0, newData.GetFloat("f0"));
		Assert.AreEqual(f1, newData.GetFloat("f1"));
		Assert.AreEqual(d0, newData.GetDouble("d0"));
		Assert.AreEqual(d1, newData.GetDouble("d1"));
		Assert.AreEqual(s0, newData.GetString("s0"));
		Assert.AreEqual(s1, newData.GetString("s1"));
	}

	[Test]
	public void Bytes() {
		// Saving
		var oldData = PersistentData.getInstance();
		oldData.Clear();

		var oldBytes = new byte[] { 0x41, 0x20, 0x41 };
		oldData.SetBytes("bytes", oldBytes);
		oldData.Save();

		// Loading
		var newData = PersistentData.getInstance();

		// Test
		Assert.AreEqual(oldBytes, newData.GetBytes("bytes"), "Simple byte array");
	}

	[Test]
	public void Serializable() {
		// Saving
		var oldData = PersistentData.getInstance();
		oldData.Clear();

		var oldList = new int[] { 0, 1, 2 };
		oldData.SetObject("intList", oldList);
		oldData.Save();

		// Loading
		var newData = PersistentData.getInstance();

		// Test
		Assert.AreEqual(oldList, newData.GetObject<int[]>("intList"), "Integer list");
	}
}

