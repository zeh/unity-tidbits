using System;
using UnityEngine;

public class MathUtils {

	/*
	public static double map(double __value, double __oldMin, double __oldMax, double __newMin = 0, double __newMax = 0, bool __clamp = false) {
		if (__oldMin == __oldMax) return __newMin;
		double p = ((__value - __oldMin) / (__oldMax - __oldMin) * (__newMax - __newMin)) + __newMin;
		if (__clamp) p = __newMin < __newMax ? clamp(p, __newMin, __newMax) : clamp(p, __newMax, __newMin);
		return p;
	}
	*/

	public static float map(float __value, float __oldMin, float __oldMax, float __newMin = 0, float __newMax = 0, bool __clamp = false) {
		if (__oldMin == __oldMax) return __newMin;
		float p = ((__value - __oldMin) / (__oldMax - __oldMin) * (__newMax - __newMin)) + __newMin;
		if (__clamp) p = __newMin < __newMax ? clamp(p, __newMin, __newMax) : clamp(p, __newMax, __newMin);
		return p;
	}

	/*
	public static double map(TimeSpan __value, TimeSpan __oldMin, TimeSpan __oldMax, double __newMin = 0, double __newMax = 0, bool __clamp = false) {
		return MathUtils.map(__value.TotalMilliseconds, __oldMin.TotalMilliseconds, __oldMax.TotalMilliseconds, __newMin, __newMax, __clamp);
	}
	*/

	public static float clamp(float __value, float __min = 0.0f, float __max = 1.0f) {
		return __value < __min ? __min : __value > __max ? __max : __value;
	}

	/*
	public static double clamp(double __value, double __min = 0.0, double __max = 1.0) {
		return __value < __min ? __min : __value > __max ? __max : __value;
	}
	*/
}

