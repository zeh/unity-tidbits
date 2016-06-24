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

	public static float map(float value, float oldMin, float oldMax, float newMin = 0, float newMax = 1, bool clamp = false) {
		if (oldMin == oldMax) return newMin;
		float p = ((value - oldMin) / (oldMax - oldMin) * (newMax - newMin)) + newMin;
		if (clamp) p = newMin < newMax ? MathUtils.clamp(p, newMin, newMax) : MathUtils.clamp(p, newMax, newMin);
		return p;
	}

	/*
	public static double map(TimeSpan __value, TimeSpan __oldMin, TimeSpan __oldMax, double __newMin = 0, double __newMax = 0, bool __clamp = false) {
		return MathUtils.map(__value.TotalMilliseconds, __oldMin.TotalMilliseconds, __oldMax.TotalMilliseconds, __newMin, __newMax, __clamp);
	}
	*/

	public static float clamp(float value, float min = 0.0f, float max = 1.0f) {
		return value < min ? min : value > max ? max : value;
	}

	/*
	public static double clamp(double __value, double __min = 0.0, double __max = 1.0) {
		return __value < __min ? __min : __value > __max ? __max : __value;
	}
	*/

	/**
	 * Clamps a value to a range, by restricting it to a minimum and maximum values but folding the value to the range instead of simply resetting to the minimum and maximum. It works like a more powerful Modulo function.
	 * @param value	The value to be clamped.
	 * @param min		Minimum value allowed.
	 * @param max		Maximum value allowed.
	 * @return			The newly clamped value.
	 * @example Some examples:
	 *
	 * MathUtils.rangeMod(14, 0, 10); // 4
	 * MathUtils.rangeMod(360, 0, 360); // 0
	 * MathUtils.rangeMod(360, -180, 180); // 0
	 * MathUtils.rangeMod(21, 0, 10); // 1
	 * MathUtils.rangeMod(-98, 0, 100); // 2
	 */
	public static float rangeMod(float value, float min, float pseudoMax) {
		float range = pseudoMax - min;
		value = (value - min) % range;
		if (value < 0) value = range - (-value % range);
		value += min;
		return value;
	}

	/**
	 * Find the "distance" two item indexes are in a list. The list cycles through.
	 * If negative, the item index is behind the base index.
	 */
	// TODO: better name?
	public static float listCycleDistance(float newPosition, float basePosition, int baseLength) {
		float posBefore, posAfter;

		if (newPosition > basePosition) {
			posBefore = - baseLength + newPosition - basePosition;
			posAfter = newPosition - basePosition;
		} else {
			posBefore = newPosition - basePosition;
			posAfter = baseLength - basePosition + newPosition;
		}

		//posBefore = posBefore % baseLength;
		//posAfter = posAfter % baseLength;

		return Mathf.Abs(posBefore) < posAfter ? posBefore : posAfter;
	}
}

