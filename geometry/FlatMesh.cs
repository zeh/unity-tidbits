using UnityEngine;
using System;
using System.Collections.Generic;

public class FlatMesh {

	/*
	 * Given a mesh (vertices, normals, triangles, and colors), calculates a new mesh with:
	 * . Flat shaded triangles (splitting edges)
	 * . Fake SSAO/Ambient Occlusion (with vertex colors)
	 */

	// Properties
	private	Vector3[]	vertices;
	private	Vector3[]	normals;
	private	int[]		triangles;
	private	Color32[]	colors;

	private	Vector3[]	newVertices;
	private	int[]		newTriangles;
	private	Vector2[]	newUvs;
	private	Color32[]	newColors;

	private bool		isMeshDirty;			// If true, the mesh needs to be recalculated


	// ================================================================================================================
	// CONSTRUCTOR ----------------------------------------------------------------------------------------------------

	public FlatMesh(Vector3[] __vertices, Vector3[] __normals, int[] __triangles, Color32[] __colors) {
		D.Log("FlatMesh initialized; " + __vertices.Length + " vertices, " + (__triangles.Length/3) + " triangles, " + __colors.Length + " colors");

		vertices = __vertices;
		normals = __normals;
		triangles = __triangles;
		colors = __colors;

		isMeshDirty = true;
	}


	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	public Vector3[] flatVertices {
		get {
			if (isMeshDirty) recalculateFlatMesh();
			return newVertices;
		}
	}

	public int[] flatTriangles {
		get {
			if (isMeshDirty) recalculateFlatMesh();
			return newTriangles;
		}
	}

	public Vector2[] flatUvs {
		get {
			if (isMeshDirty) recalculateFlatMesh();
			return newUvs;
		}
	}

	public Color32[] flatColors {
		get {
			if (isMeshDirty) recalculateFlatMesh();
			return newColors;
		}
	}


	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

	private void recalculateFlatMesh() {
		// Creates the new mesh
		int numTriangles = triangles.Length / 3;
		int numVertices = vertices.Length;

		int i, j, t;
		Vector3 v;
		//Vector3 n;
		Color32 c;

		// Calculates ambient occlusion

		float ti = Time.realtimeSinceStartup;

		// Finds darkness of every vertice
		float[] darkness = new float[numVertices]; // 0 (normal color) to 1 (black)

		// Creates ray samples
		int numSamples = 20;
		Vector3[] rotations = new Vector3[numSamples];
		Vector3 o;
		float radius = 400f;

		for (i = 0; i < numSamples; i++) {
			rotations[i] = UnityEngine.Random.onUnitSphere * radius;
		}
		
		List<Vector3> myRots;
		
		Vector3 v1xv2, v2xv3, v3xv1;
		Vector3 v1, v2, v3;
		
		for (i = 0; i < numVertices; i++) {
			// For every vertice...
			o = vertices[i] + normals[i];
			
			myRots = new List<Vector3>(rotations);
			
			for (t = 0; t < numTriangles * 3; t += 3) {
				// ...check every triangle...
				
				v1 = vertices[triangles[t]] - o;
				v2 = vertices[triangles[t+1]] - o;
				v3 = vertices[triangles[t+2]] - o;
				
				v1xv2 = Vector3.Cross(v1, v2);
				v2xv3 = Vector3.Cross(v2, v3);
				v3xv1 = Vector3.Cross(v3, v1);
				
				for (j = 0; j < myRots.Count; j++) {
					// ...against every ray
					if (GeomUtils.rayIntersectsTriangleSF_precalculated(o, myRots[j], v1xv2, v2xv3, v3xv1)) {
						myRots.RemoveAt(j);
						j--;
					}
				}
			}

			darkness[i] = Math.Abs(((float)(rotations.Length - myRots.Count) / numSamples - 0.5f) * 2f);
			//if (i % 100 == 0 || hits >= numSamples) {
			//if (i % 100 == 0) D.Log ("@" + i + " : " + o + " -> " + Vector3.back + " >> hits = " + hits + ", " + darkness[i]);
		}

		// Applies darkness to the vertex colors
		for (i = 0; i < numVertices; i++) {
			colors[i].r = (byte)(colors[i].r * (1-darkness[i]));
			colors[i].g = (byte)(colors[i].g * (1-darkness[i]));
			colors[i].b = (byte)(colors[i].b * (1-darkness[i]));
			//colors[i].r = (byte)Math.Max(0, colors[i].r - Math.Floor(darkness[i] * 255f));
			//colors[i].g = (byte)Math.Max(0, colors[i].g - Math.Floor(darkness[i] * 255f));
			//colors[i].b = (byte)Math.Max(0, colors[i].b - Math.Floor(darkness[i] * 255f));
		}

		D.Log("Took " + (Time.realtimeSinceStartup-ti) + "s to generate vertices ambient occlusion.");

		// Creates new properties
		newVertices		= new Vector3[numTriangles * 3];
		newTriangles	= new int[numTriangles * 3];
		newUvs			= new Vector2[numTriangles * 3];
		newColors		= new Color32[numTriangles * 3];

		// Splits edges
		for (i = 0; i < triangles.Length; i++) {
			v = vertices[triangles[i]];
			c = colors[triangles[i]];
			newVertices[i] = new Vector3(v.x, v.y, v.z);
			newTriangles[i] = i;
			newColors[i] = new Color32(c.r, c.g, c.b, c.a);
		}

		// Creates individual UVs
		float uvNudge = 17f/256f; // Offset so the triangle fits
		for (i = 0; i < triangles.Length; i += 3) {
			newUvs[i+0] = new Vector2(0, 0 + uvNudge);
			newUvs[i+1] = new Vector2(0.5f, 1 - uvNudge);
			newUvs[i+2] = new Vector2(1, 0 + uvNudge);
		}

		isMeshDirty = false;
	}
}
