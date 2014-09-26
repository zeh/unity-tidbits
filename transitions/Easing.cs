using System;

public class Easing {

	/*
	Disclaimer for Robert Penner's Easing Equations license:

	TERMS OF USE - EASING EQUATIONS

	Open source under the BSD License.

	Copyright © 2001 Robert Penner
	All rights reserved.

	Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

	 * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
	 * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
	 * Neither the name of the author nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.

	THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
	 */

	// Constants
	private const float HALF_PI = (float)(Math.PI / 2.0);
	private const float TWO_PI = (float)(Math.PI * 2.0);

	/**
	 * Easing equation function for a simple linear tweening, with no easing.
	 *
	 * @param	t			Current time/phase (0-1).
	 * @return				The new value/phase (0-1).
	 */
	public static float none(float t) {
		return t;
	}

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
		return ((t *= 2f) < 1f) ? t * t * 0.5f : -0.5f * (--t * (t-2f) - 1f);
	}

	/**
	 * Easing equation function for a cubic (t^3) easing in: accelerating from zero velocity.
 	 *
	 * @param	t			Current time/phase (0-1).
	 * @return				The new value/phase (0-1).
	 */
	public static float cubicIn(float t) {
		return t*t*t;
	}

	/**
	 * Easing equation function for a cubic (t^3) easing out: decelerating from zero velocity.
 	 *
	 * @param	t			Current time/phase (0-1).
	 * @return				The new value/phase (0-1).
	 */
	public static float cubicOut(float t) {
		return (t = t-1.0f) * t * t + 1.0f;
	}

	public static float cubicInOut(float t) {
		return (t *= 2.0f) < 1.0f ? cubicIn(t)/2.0f : cubicOut(t-1.0f)/.0f+0.5f; // TODO: redo with in-line calculation
	}

	/**
	 * Easing equation function for a quartic (t^4) easing in: accelerating from zero velocity.
 	 *
	 * @param	t			Current time/phase (0-1).
	 * @return				The new value/phase (0-1).
	 */
	public static float quartIn(float t) {
		return t*t*t*t;
	}

	/**
	 * Easing equation function for a quartic (t^4) easing out: decelerating from zero velocity.
	 *
	 * @param	t			Current time/phase (0-1).
	 * @return				The new value/phase (0-1).
	 */
	public static float quartOut(float t) {
		t--;
		return -1.0f * (t * t * t * t - 1.0f);
	}

	public static float quartInOut(float t) {
		return (t *= 2.0f) < 1.0f ? quartIn(t)/2.0f : quartOut(t-1.0f)/2.0f+0.5f; // TODO: redo with in-line calculation
	}

	/**
	 * Easing equation function for a quintic (t^5) easing in: accelerating from zero velocity.
 	 *
	 * @param	t			Current time/phase (0-1).
	 * @return				The new value/phase (0-1).
	 */
	public static float quintIn(float t) {
		return t*t*t*t*t;
	}

	/**
	 * Easing equation function for a quintic (t^5) easing out: decelerating from zero velocity.
 	 *
	 * @param	t			Current time/phase (0-1).
	 * @return				The new value/phase (0-1).
	 */
	public static float quintOut(float t) {
		t--;
		return t*t*t*t*t + 1.0f;
	}

	public static float quintInOut(float t) {
		return (t *= 2.0f) < 1.0f ? quintIn(t)/2.0f : quintOut(t-1.0f)/2.0f+0.5f; // TODO: redo with in-line calculation
	}

	/**
	 * Easing equation function for a sinusoidal (sin(t)) easing in: accelerating from zero velocity.
 	 *
	 * @param	t			Current time/phase (0-1).
	 * @return				The new value/phase (0-1).
	 */
	public static float sineIn(float t) {
		return -1.0f * (float)Math.Cos(t * HALF_PI) + 1.0f;
	}

	/**
	 * Easing equation function for a sinusoidal (sin(t)) easing out: decelerating from zero velocity.
 	 *
	 * @param	t			Current time/phase (0-1).
	 * @return				The new value/phase (0-1).
	 */
	public static float sineOut(float t) {
		return (float)Math.Sin(t * HALF_PI);
	}

	public static float sineInOut(float t) {
		return (t *= 2.0f) < 1.0f ? sineIn(t)/2.0f : sineOut(t-1.0f)/2.0f+0.5f; // TODO: redo with in-line calculation
	}

	/**
	 * Easing equation function for an exponential (2^t) easing in: accelerating from zero velocity.
 	 *
	 * @param	t			Current time/phase (0-1).
	 * @return				The new value/phase (0-1).
	 */
	public static float expoIn(float t) {
		// return (t==0) ? b : c * Math.pow(2, 10 * (t/d - 1)) + b; // original
		// return (t==0) ? 0 : Math.pow(2, 10 * (t - 1)); // ztween
		// return (t == 0) ? b : c * Math.pow(2, 10 * (t / d - 1)) + b - c * 0.001; // tweener fixed
		return (t==0) ? 0 : (float)Math.Pow(2, 10.0f * (t - 1.0f)) - 0.001f; // ztween fixed
	}

	/**
	 * Easing equation function for an exponential (2^t) easing out: decelerating from zero velocity.
 	 *
	 * @param	t			Current time/phase (0-1).
	 * @return				The new value/phase (0-1).
	 */
	public static float expoOut(float t) {
		// return (t==d) ? b+c : c * (-Math.pow(2, -10 * t/d) + 1) + b; // original
		// return (t==1) ? 1 : (-Math.pow(2, -10 * t) + 1); // ztween
		// return (t == d) ? b + c : c * 1.001 * (-Math.pow(2, -10 * t / d) + 1) + b; // tweener fixed
		//log(">", t, (t==1) ? 1 : 1.001 * (-Math.pow(2, -10 * t) + 1))
		//return (t==1) ? 1 : 1.001 * (-Math.pow(2, -10 * t) + 1); // ztween fixed
		return (t>=0.999f) ? 1.0f : 1.001f * (-(float)Math.Pow(2, -10.0f * t) + 1.0f); // ztween fixed 2
	}

	public static float expoInOut(float t) {
		return (t *= 2.0f) < 1.0f ? expoIn(t)/2.0f : expoOut(t-1.0f)/2.0f+0.5f; // TODO: redo with in-line calculation
	}

	/**
	 * Easing equation function for a circular (sqrt(1-t^2)) easing in: accelerating from zero velocity.
 	 *
	 * @param	t			Current time/phase (0-1).
	 * @return				The new value/phase (0-1).
	 */
	public static float circIn(float t) {
		return -1.0f * ((float)Math.Sqrt(1.0f - t*t) - 1.0f);
	}

	/**
	 * Easing equation function for a circular (sqrt(1-t^2)) easing out: decelerating from zero velocity.
 	 *
	 * @param	t			Current time/phase (0-1).
	 * @return				The new value/phase (0-1).
	 */
	public static float circOut(float t) {
		t--;
		return (float)Math.Sqrt(1.0f - t*t);
	}

	public static float circInOut(float t) {
		return (t *= 2.0f) < 1.0f ? circIn(t)/2.0f : circOut(t-1.0f)/2.0f+0.5f; // TODO: redo with in-line calculation
	}

	/**
	 * Easing equation function for an elastic (exponentially decaying sine wave) easing in: accelerating from zero velocity.
	 *
	 * @param	t			Current time/phase (0-1).
	 * @param	a			Amplitude.
	 * @param	p			Period.
	 * @return				The new value/phase (0-1).
	 */
	public static float elasticIn(float t, float a, float p) {
		if (t==0) return 0;
		if (t==1) return 1;
		float s;
		if (a < 1.0f) {
			a = 1.0f;
			s = p/4.0f;
		} else {
			s = p/TWO_PI * (float)Math.Asin(1.0f/a);
		}
		return -(a*(float)Math.Pow(2,10.0f*(t-=1)) * (float)Math.Sin((t-s)*TWO_PI/p));
	}

	/**
	 * Overloads are used instead of optional arguments just so the function call works as a valid Func<float> type
	 */
	public static float elasticIn(float t) {
		return elasticIn(t, 0, 0.3f);
	}

	/**
	 * Easing equation function for an elastic (exponentially decaying sine wave) easing out: decelerating from zero velocity.
	 *
	 * @param	t			Current time/phase (0-1).
	 * @param	a			Amplitude.
	 * @param	p			Period.
	 */
	public static float elasticOut(float t, float a, float p) {
		if (t==0) return 0;
		if (t==1) return 1;
		float s;
		if (a < 1.0f) {
			a = 1.0f;
			s = p/4.0f;
		} else {
			s = p/TWO_PI * (float)Math.Asin(1.0f/a);
		}
		return (a*(float)Math.Pow(2,-10.0f*t) * (float)Math.Sin((t-s)*TWO_PI/p) + 1.0f);
	}

	/**
	 * Overloads are used instead of optional arguments just so the function call works as a valid Func<float> type
	 */
	public static float elasticOut(float t) {
		return elasticOut(t, 0, 0.3f);
	}


	/**
	 * Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing in: accelerating from zero velocity.
	 *
	 * @param	t			Current time/phase (0-1).
	 * @param	s			Overshoot ammount: higher s means greater overshoot (0 produces cubic easing with no overshoot, and the default value of 1.70158 produces an overshoot of 10 percent).
	 * @param	p			Period.
	 */
	public static float backIn(float t, float s) {
		return t*t*((s+1.0f)*t - s);
	}

	/**
	 * Overloads are used instead of optional arguments just so the function call works as a valid Func<float> type
	 */
	public static float backIn(float t) {
		return backIn(t, 1.70158f);
	}

	/**
	 * Easing equation function for a back (overshooting cubic easing: (s+1)*t^3 - s*t^2) easing out: decelerating from zero velocity.
	 *
	 * @param	t			Current time/phase (0-1).
	 * @param	s			Overshoot ammount: higher s means greater overshoot (0 produces cubic easing with no overshoot, and the default value of 1.70158 produces an overshoot of 10 percent).
	 * @param	p			Period.
	 */
	public static float backOut(float t, float s) {
		t--;
		return t*t*((s+1.0f)*t + s) + 1.0f;
	}

	/**
	 * Overloads are used instead of optional arguments just so the function call works as a valid Func<float> type
	 */
	public static float backOut(float t) {
		return backOut(t, 1.70158f);
	}

	public static float backInOut(float t) {
		return (t *= 2.0f) < 1.0f ? backIn(t)/2.0f : backOut(t-1.0f)/2.0f+0.5f;
	}

	/**
	 * Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in: accelerating from zero velocity.
	 *
	 * @param	t			Current time/phase (0-1).
	 * @param	p			Period.
	 */
	public static float bounceIn(float t) {
		return 1.0f - bounceOut(1.0f-t);
	}

	/**
	 * Easing equation function for a bounce (exponentially decaying parabolic bounce) easing out: decelerating from zero velocity.
	 *
	 * @param	t			Current time/phase (0-1).
	 * @param	p			Period.
	 */
	public static float bounceOut(float t) {
		if (t < (1.0f/2.75f)) {
			return 7.5625f*t*t;
		} else if (t < (2.0f/2.75f)) {
			return 7.5625f*(t-=(1.5f/2.75f))*t + 0.75f;
		} else if (t < (2.5f/2.75f)) {
			return 7.5625f*(t-=(2.25f/2.75f))*t + 0.9375f;
		} else {
			return 7.5625f*(t-=(2.625f/2.75f))*t + 0.984375f;
		}
	}
}
