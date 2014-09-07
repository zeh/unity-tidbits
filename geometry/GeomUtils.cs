using System;
using UnityEngine;

public class GeomUtils {
	
	public static void test() {
		float ti;
		int times = 2;
		Boolean temp;
		int i;
		
		// Moller-Trumbore method
		ti = Time.realtimeSinceStartup;
		for (i = 0; i < times; i++) {
			temp = rayIntersectsTriangleMT(new Vector3(2, 2, 0), new Vector3(0, 0, 20), new Vector3(0, 0, 10), new Vector3(0, 10, 10), new Vector3(10, 0, 10), true);
			if (i == 0) D.Log ("==> [true, ff=true] = " + temp);
			temp = rayIntersectsTriangleMT(new Vector3(2, 2, 0), new Vector3(0, 0, 20), new Vector3(0, 0, 10), new Vector3(10, 0, 10), new Vector3(0, 10, 10), true);
			if (i == 0) D.Log ("==> [true, ff=false] = " + temp);
			temp = rayIntersectsTriangleMT(new Vector3(-2, 2, 0), new Vector3(0, 0, 20), new Vector3(0, 0, 10), new Vector3(10, 0, 10), new Vector3(0, 10, 10), true);
			if (i == 0) D.Log ("==> [false] = " + temp);
			temp = rayIntersectsTriangleMT(new Vector3(-2, 2, 10), new Vector3(10, 0, 10), new Vector3(0, 0, 10), new Vector3(10, 0, 10), new Vector3(0, 10, 10), true);
			if (i == 0) D.Log ("==> [false] = " + temp);
			temp = rayIntersectsTriangleMT(new Vector3(-492.3f, 375.0f, 1.0f), new Vector3(0.0f, 0.0f, -1.0f), new Vector3(-553.8f, 250.0f, 0.0f), new Vector3(-615.4f, 375.0f, 0.0f), new Vector3(-492.3f, 375.0f, 0.0f), true);
			if (i == 0) D.Log ("==> [true] = " + temp);
			temp = rayIntersectsTriangleMT(new Vector3(-492.3f, 375.0f, 1.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(-553.8f, 250.0f, 0.0f), new Vector3(-615.4f, 375.0f, 0.0f), new Vector3(-492.3f, 375.0f, 0.0f), true);
			if (i == 0) D.Log ("==> [false] = " + temp);
		}
		
		D.Log("[MT] Time for " + times + " calls: " + (Time.realtimeSinceStartup - ti) + "s");
		
		// Segura-Feito method method
		ti = Time.realtimeSinceStartup;
		for (i = 0; i < times; i++) {
			temp = rayIntersectsTriangleSF(new Vector3(2, 2, 0), new Vector3(0, 0, 20), new Vector3(0, 0, 10), new Vector3(0, 10, 10), new Vector3(10, 0, 10));
			if (i == 0) D.Log ("==> [true, hit ccw] = " + temp);
			temp = rayIntersectsTriangleSF(new Vector3(2, 2, 0), new Vector3(0, 0, 20), new Vector3(0, 0, 10), new Vector3(10, 0, 10), new Vector3(0, 10, 10));
			if (i == 0) D.Log ("==> [true, hit cw] = " + temp);
			temp = rayIntersectsTriangleSF(new Vector3(-2, 2, 0), new Vector3(0, 0, 20), new Vector3(0, 0, 10), new Vector3(10, 0, 10), new Vector3(0, 10, 10));
			if (i == 0) D.Log ("==> [false] = " + temp);
			temp = rayIntersectsTriangleSF(new Vector3(-2, 2, 10), new Vector3(10, 0, 10), new Vector3(0, 0, 10), new Vector3(10, 0, 10), new Vector3(0, 10, 10));
			if (i == 0) D.Log ("==> [false] = " + temp);
			temp = rayIntersectsTriangleSF(new Vector3(-492.3f, 375.0f, 1.0f), new Vector3(0.0f, 0.0f, -1.0f), new Vector3(-553.8f, 250.0f, 0.0f), new Vector3(-615.4f, 375.0f, 0.0f), new Vector3(-492.3f, 375.0f, 0.0f));
			if (i == 0) D.Log ("==> [true] = " + temp);
			temp = rayIntersectsTriangleSF(new Vector3(-492.3f, 375.0f, 1.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(-553.8f, 250.0f, 0.0f), new Vector3(-615.4f, 375.0f, 0.0f), new Vector3(-492.3f, 375.0f, 0.0f));
			if (i == 0) D.Log ("==> [false] = " + temp);
		}
		
		D.Log("[SF] Time for " + times + " calls: " + (Time.realtimeSinceStartup - ti) + "s");

		
		// Segura-Feito method method with precalculation
		ti = Time.realtimeSinceStartup;
		Vector3 v1xv2 = Vector3.Cross(new Vector3(0, 0, 10) - new Vector3(-2, -2, 0), new Vector3(10, 0, 10)- new Vector3(-2, -2, 0));
		Vector3 v2xv3 = Vector3.Cross(new Vector3(10, 0, 10)- new Vector3(-2, -2, 0), new Vector3(0, 10, 10)- new Vector3(-2, -2, 0));
		Vector3 v3xv1 = Vector3.Cross(new Vector3(0, 10, 10)- new Vector3(-2, -2, 0), new Vector3(0, 0, 10)- new Vector3(-2, -2, 0));
		for (i = 0; i < times; i++) {
			temp = rayIntersectsTriangleSF_precalculated(new Vector3(2, 2, 0), new Vector3(0, 0, 20), new Vector3(0, 0, 10), new Vector3(0, 10, 10), new Vector3(10, 0, 10));
			if (i == 0) D.Log ("==> [true, hit ccw] = " + temp);
			temp = rayIntersectsTriangleSF_precalculated(new Vector3(2, 2, 0), new Vector3(0, 0, 20), v1xv2, v2xv3, v3xv1);
			if (i == 0) D.Log ("==> [true, hit cw] = " + temp);
			temp = rayIntersectsTriangleSF_precalculated(new Vector3(-2, 2, 0), new Vector3(0, 0, 20), v1xv2, v2xv3, v3xv1);
			if (i == 0) D.Log ("==> [false] = " + temp);
			temp = rayIntersectsTriangleSF_precalculated(new Vector3(-2, 2, 10), new Vector3(10, 0, 10), v1xv2, v2xv3, v3xv1);
			if (i == 0) D.Log ("==> [false] = " + temp);
			temp = rayIntersectsTriangleSF(new Vector3(-492.3f, 375.0f, 1.0f), new Vector3(0.0f, 0.0f, -1.0f), new Vector3(-553.8f, 250.0f, 0.0f), new Vector3(-615.4f, 375.0f, 0.0f), new Vector3(-492.3f, 375.0f, 0.0f));
			if (i == 0) D.Log ("==> [true] = " + temp);
			temp = rayIntersectsTriangleSF(new Vector3(-492.3f, 375.0f, 1.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(-553.8f, 250.0f, 0.0f), new Vector3(-615.4f, 375.0f, 0.0f), new Vector3(-492.3f, 375.0f, 0.0f));
			if (i == 0) D.Log ("==> [false] = " + temp);
		}
		
		D.Log("[SF PRECALC] Time for " + times + " calls: " + (Time.realtimeSinceStartup - ti) + "s");

		//D.Log("Intersect 3 ==> " + rayIntersectsTriangle(new Vector3(2, 2, 100), new Vector3(0, 0, -200), new Vector3(-1000, -1000, 10), new Vector3(1000, -1000, 10), new Vector3(0, 500, 10)));
		//D.Log("Intersect 4 ==> " + rayIntersectsTriangle(new Vector3(2, 2, 100), new Vector3(0, 0, -200), new Vector3(-1000, -1000, 10), new Vector3(0, 500, 10), new Vector3(1000, -1000, 10)));
		//D.Log("Intersect 3 ==> " + segmentIntersectsTriangle(new Vector3(2, 2, 0), new Vector3(2, 2, 20), new Vector3(0, 0, 10), new Vector3(0, 10, 10), new Vector3(10, 0, 10)));
		//D.Log("Intersect 4 ==> " + segmentIntersectsTriangle(new Vector3(2, 2, 0), new Vector3(2, 2, 20), new Vector3(0, 0, 10), new Vector3(10, 0, 10), new Vector3(0, 10, 10)));
	}

	public static bool rayIntersectsTriangleMT(Vector3 __origin, Vector3 __direction, Vector3 __v1, Vector3 __v2, Vector3 __v3, bool __allowSegmentOnly) {
		// Moller-Trumbore method
		// http://www.hugi.scene.org/online/coding/hugi%2025%20-%20coding%20corner%20graphics,%20sound%20&%20synchronization%20ken%20ray-triangle%20intersection%20tests%20for%20dummies.htm
		// Returns: float u, float v, float t, bool intersect, bool frontfacing
		
		float eps = 0.000000001f;					// eps is the machine fpu epsilon (precision), or a very small number :)
		Vector3 e2 = __v3 - __v1;					// second edge
		Vector3 e1 = __v2 - __v1;					// first edge
		Vector3 r = Vector3.Cross(__direction, e2);	// (d X e2) is used two times in the formula so we store it in an appropriate vector
		Vector3 s = __origin - __v1;				// translated ray origin
		float a = Vector3.Dot(e1, r);				// a=(d X e2)*e1
		Vector3 q = Vector3.Cross(s, e1);
		float u = Vector3.Dot(s, r);
		//bool frontfacing = true;
		float v;
		if (a > eps) {
			// Front facing triangle...
			if ((u < 0) || (u > a)) {
				//D.Log ("u = " + 0 + ", v = " + 0 + ", t = " + 0 + ", intersect = false, frontfacing = " + frontfacing);
				return false;
			}
			v = Vector3.Dot(__direction, q);
			if ((v < 0) || (u+v > a)) {
				//D.Log ("u = " + 0 + ", v = " + 0 + ", t = " + 0 + ", intersect = false, frontfacing = " + frontfacing);
				return false;
			}
		} else if (a < -eps) {
			// Back facing triangle...
			//frontfacing = false;
			if ((u > 0) || (u < a)) {
				//D.Log ("u = " + 0 + ", v = " + 0 + ", t = " + 0 + ", intersect = false, frontfacing = " + frontfacing);
				return false;
			}
			v = Vector3.Dot(__direction, q);
			if ((v > 0) || (u+v < a)) {
				//D.Log ("u = " + 0 + ", v = " + 0 + ", t = " + 0 + ", intersect = false, frontfacing = " + frontfacing);
				return false;
			}
		} else {
			// Ray parallel to triangle plane
			//D.Log ("u = " + 0 + ", v = " + 0 + ", t = " + 0 + ", intersect = false, frontfacing = false [PARALLEL]");
			return false;
		}
		float f = 1f / a;							// slow division*
		float t = f * Vector3.Dot(e2, q);
		//u = u * f;
		//v = v * f;
		//D.Log ("u = " + u + ", v = " + v + ", t = " + t + ", intersect = true, frontfacing = " + frontfacing);
		return t > 0 && (!__allowSegmentOnly || t <= 1);
		//return true;
	}
	
	public static bool rayIntersectsTriangleSF_old(Vector3 __origin, Vector3 __direction, Vector3 __v1, Vector3 __v2, Vector3 __v3) {
		// Segura-Feito method

		/*
		__v1 -= __origin;
		__v2 -= __origin;
		__v3 -= __origin;
		
		Vector3 tmp = Vector3.Cross(__direction, __v1);

		float i = Mathf.Sign(Vector3.Dot(__v3, tmp));
		if (i<=0) return false;
		i = Mathf.Sign(Vector3.Dot(__v2, tmp));
		if (i<=0) return false;
		i = Mathf.Sign(Vector3.Dot(__v3, Vector3.Cross(__direction, __v2)));
		if (i<=0) return false;
		*/

		float i = Mathf.Sign(Vector3.Dot(__direction, Vector3.Cross(__v1 - __origin, __v2 - __origin)));
		if (i<=0) return false;
		i = Mathf.Sign(Vector3.Dot(__direction, Vector3.Cross(__v2 - __origin, __v3 - __origin)));
		if (i<=0) return false;
		i = Mathf.Sign(Vector3.Dot(__direction, Vector3.Cross(__v3 - __origin, __v1 - __origin)));
		if (i<=0) return false;
		
		return true;
	}
	
	public static bool rayIntersectsTriangleSF(Vector3 __origin, Vector3 __direction, Vector3 __v1, Vector3 __v2, Vector3 __v3) {
		return rayIntersectsTriangleSF_precalculated(__origin, __direction, Vector3.Cross(__v1 - __origin, __v2 - __origin), Vector3.Cross(__v2 - __origin, __v3 - __origin), Vector3.Cross(__v3 - __origin, __v1 - __origin));
	}

	public static bool rayIntersectsTriangleSF_precalculated(Vector3 __origin, Vector3 __direction, Vector3 __v1xv2, Vector3 __v2xv3, Vector3 __v3xv1) {
		if (Mathf.Sign(Vector3.Dot(__direction, __v1xv2)) <= 0) return false;
		if (Mathf.Sign(Vector3.Dot(__direction, __v2xv3)) <= 0) return false;
		if (Mathf.Sign(Vector3.Dot(__direction, __v3xv1)) <= 0) return false;
		return true;
	}
	
	public static bool rayIntersectsTriangleSF2(Vector3 __start, Vector3 __end, Vector3 __v1, Vector3 __v2, Vector3 __v3, bool __someBool) {
		// http://www.hugi.scene.org/online/coding/hugi%2025%20-%20coding%20corner%20graphics,%20sound%20&%20synchronization%20ken%20ray-triangle%20intersection%20tests%20for%20dummies.htm
		// http://iason.zcu.cz/wscg2001/Papers_2001/R75.pdf
		
		__end += __start;

		//D.Log ("   ==> " + areaSign(__start, __end, __v1, __v3));
		//D.Log ("   ==> " + areaSign(__start, __end, __v2, __v3));
		//D.Log ("   ==> " + areaSign(__start, __end, __v1, __v2));
		if (areaSign(__start, __end, __v1, __v3) <= 0) return false;
		if (areaSign(__start, __end, __v2, __v3) <= 0) return false;
		if (areaSign(__start, __end, __v1, __v2) <= 0) return false;
		return true;
	}
	
	public static float areaSign(Vector3 rs, Vector3 re, Vector3 v1, Vector3 v2) {
		return Mathf.Sign(v2.z * (re.x * v1.y - v1.x * re.y) + v2.y * (v1.z * re.x - re.z * v1.x) + v2.x * (v1.y * re.z - re.y * v1.z));
		//return Mathf.Sign(Vector3.Dot(re, Vector3.Cross(v1, v2)));
	}

	/*
	public bool rayIntersectsTriangle(Vector3 __origin, Vector3 __direction, Vector3 __v1, Vector3 __v2, Vector3 __v3) {
		__v1 -= __origin;
		__v2 -= __origin;
		__v3 -= __origin;
		
		Vector3 tmp = Vector3.Cross(__direction, __v1);

		D.Log ("   ==> " + Mathf.Sign(Vector3.Dot(__v3, tmp)));
		D.Log ("   ==> " + Mathf.Sign(Vector3.Dot(__v2, tmp)));
		D.Log ("   ==> " + Mathf.Sign(Vector3.Dot(__v3, Vector3.Cross(__direction, __v2))));

		float i = Mathf.Sign(Vector3.Dot(__v3, tmp));
		if (i<=0) return false;
		i = Mathf.Sign(Vector3.Dot(__v2, tmp));
		if (i<=0) return false;
		i = Mathf.Sign(Vector3.Dot(__v3, Vector3.Cross(__direction, __v2)));
		if (i<=0) return false;
		
		return true;
	}
	*/
}

