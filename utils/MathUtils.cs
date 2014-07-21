using System;
using UnityEngine;

public class MathUtils {

	public static float map(float __value, float __oldMin, float __oldMax, float __newMin = 0, float __newMax = 0, bool __clamp = false) {
		if (__oldMin == __oldMax) return __newMin;
		float p = ((__value-__oldMin) / (__oldMax-__oldMin) * (__newMax-__newMin)) + __newMin;
		if (__clamp) p = __newMin < __newMax ? clamp(p, __newMin, __newMax) : clamp(p, __newMax, __newMin);
		return p;
	}

	public static float clamp(float __value, float __min = 0, float __max = 1) {
		return __value < __min ? __min : __value > __max ? __max : __value;
	}
}

