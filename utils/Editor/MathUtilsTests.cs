using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

/**
 * @author zeh
 */
[TestFixture]
public class MathUtilsTests {

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
	public void listCycleDistance() {
		Assert.AreEqual(0, 		MathUtils.listCycleDistance(0,		0, 10));
		Assert.AreEqual(0.5f, 	MathUtils.listCycleDistance(0.5f,	0, 10));
		Assert.AreEqual(1, 		MathUtils.listCycleDistance(1,		0, 10));
		Assert.AreEqual(2, 		MathUtils.listCycleDistance(2,		0, 10));
		Assert.AreEqual(3, 		MathUtils.listCycleDistance(3,		0, 10));
		Assert.AreEqual(4, 		MathUtils.listCycleDistance(4,		0, 10));
		Assert.AreEqual(5,	 	MathUtils.listCycleDistance(5,		0, 10));
		Assert.AreEqual(-4, 	MathUtils.listCycleDistance(6,		0, 10));
		Assert.AreEqual(-3, 	MathUtils.listCycleDistance(7,		0, 10));
		Assert.AreEqual(-2, 	MathUtils.listCycleDistance(8,		0, 10));
		Assert.AreEqual(-1, 	MathUtils.listCycleDistance(9,		0, 10));
		Assert.AreEqual(-0.5f, 	MathUtils.listCycleDistance(9.5f,	0, 10));

		Assert.AreEqual(-1, 	MathUtils.listCycleDistance(0,		1, 10));
		Assert.AreEqual(-0.5f, 	MathUtils.listCycleDistance(0.5f,	1, 10));
		Assert.AreEqual(0, 		MathUtils.listCycleDistance(1,		1, 10));
		Assert.AreEqual(1, 		MathUtils.listCycleDistance(2,		1, 10));
		Assert.AreEqual(2, 		MathUtils.listCycleDistance(3,		1, 10));
		Assert.AreEqual(3, 		MathUtils.listCycleDistance(4,		1, 10));
		Assert.AreEqual(4,	 	MathUtils.listCycleDistance(5,		1, 10));
		Assert.AreEqual(5, 		MathUtils.listCycleDistance(6,		1, 10));
		Assert.AreEqual(-4, 	MathUtils.listCycleDistance(7,		1, 10));
		Assert.AreEqual(-3, 	MathUtils.listCycleDistance(8,		1, 10));
		Assert.AreEqual(-2, 	MathUtils.listCycleDistance(9,		1, 10));
		Assert.AreEqual(-1.5f, 	MathUtils.listCycleDistance(9.5f,	1, 10));

		Assert.AreEqual(1, 		MathUtils.listCycleDistance(0,		9, 10));
		Assert.AreEqual(1.5f, 	MathUtils.listCycleDistance(0.5f,	9, 10));
		Assert.AreEqual(2, 		MathUtils.listCycleDistance(1,		9, 10));
		Assert.AreEqual(3, 		MathUtils.listCycleDistance(2,		9, 10));
		Assert.AreEqual(4, 		MathUtils.listCycleDistance(3,		9, 10));
		Assert.AreEqual(5, 		MathUtils.listCycleDistance(4,		9, 10));
		Assert.AreEqual(-4,	 	MathUtils.listCycleDistance(5,		9, 10));
		Assert.AreEqual(-3, 	MathUtils.listCycleDistance(6,		9, 10));
		Assert.AreEqual(-2, 	MathUtils.listCycleDistance(7,		9, 10));
		Assert.AreEqual(-1, 	MathUtils.listCycleDistance(8,		9, 10));
		Assert.AreEqual(0,	 	MathUtils.listCycleDistance(9,		9, 10));
		Assert.AreEqual(0.5f, 	MathUtils.listCycleDistance(9.5f,	9, 10));

		Assert.AreEqual(5,		MathUtils.listCycleDistance(0,		5, 10));
		Assert.AreEqual(-4.5f, 	MathUtils.listCycleDistance(0.5f,	5, 10));
		Assert.AreEqual(-4,		MathUtils.listCycleDistance(1,		5, 10));
		Assert.AreEqual(-3,		MathUtils.listCycleDistance(2,		5, 10));
		Assert.AreEqual(-2,		MathUtils.listCycleDistance(3,		5, 10));
		Assert.AreEqual(-1,		MathUtils.listCycleDistance(4,		5, 10));
		Assert.AreEqual(0,	 	MathUtils.listCycleDistance(5,		5, 10));
		Assert.AreEqual(1, 		MathUtils.listCycleDistance(6,		5, 10));
		Assert.AreEqual(2, 		MathUtils.listCycleDistance(7,		5, 10));
		Assert.AreEqual(3,	 	MathUtils.listCycleDistance(8,		5, 10));
		Assert.AreEqual(4,	 	MathUtils.listCycleDistance(9,		5, 10));
		Assert.AreEqual(4.5f, 	MathUtils.listCycleDistance(9.5f,	5, 10));
	}
}