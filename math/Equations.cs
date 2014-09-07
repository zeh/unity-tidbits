public class Equations {

	/**
	 * Easing equation function for a quadratic (t^2) easing in: accelerating from zero velocity.
	 *
	 * @param	t			Current time/phase (0-1).
	 * @return				The new value/phase (0-1).
	 */
	public static float quadIn(float t) {
		return t*t;
	}

	/**
	 * Easing equation function for a quadratic (t^2) easing out: decelerating to zero velocity.
	 *
	 * @param	t			Current time/phase (0-1).
	 * @return				The new value/phase (0-1).
	 */
	public static float quadOut(float t) {
		return -t * (t-2);
	}

	/**
	 * Easing equation function for a quadratic (t^2) easing in and then out: accelerating from zero velocity, then decelerating.
	 *
	 * @param	t			Current time/phase (0-1).
	 * @return				The new value/phase (0-1).
	 */
	public static float quadInOut(float t) {
		//return t < 0.5 ? quadIn(t*2) : quadOut((t-0.5)*2);
		return ((t *= 2f) < 1f) ? t * t * 0.5f : -0.5f * (--t * (t-2f) - 1f);
	}

	/**
	 * Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in: accelerating from zero velocity.
	 *
	 * @param	t			Current time/phase (0-1).
	 * @param	s			Overshoot ammount: higher s means greater overshoot (0 produces cubic easing with no overshoot, and the default value of 1.70158 produces an overshoot of 10 percent).
	 * @param	p			Period.
	 */
	public static float backIn(float t, float s = 1.70158f) {
		return t*t*((s+1)*t - s);
	}

	/**
	 * Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out: decelerating from zero velocity.
	 *
	 * @param	t			Current time/phase (0-1).
	 * @param	s			Overshoot ammount: higher s means greater overshoot (0 produces cubic easing with no overshoot, and the default value of 1.70158 produces an overshoot of 10 percent).
	 * @param	p			Period.
	 */
	public static float backOut(float t, float s = 1.70158f) {
		t--;
		return t*t*((s+1)*t + s) + 1;
	}

	public static float backInOut(float t) {
		return (t *= 2f) < 1f ? backIn(t)/2f : backOut(t-1f)/2f+0.5f;
	}
}
