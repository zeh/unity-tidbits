using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[ExecuteInEditMode()]
#endif

public class Circle:CustomMeshObject {

	/*
	A simple circle.
	*/

	// Properties
	[SerializeField] private float _radius = 100;
	[SerializeField] private int _segments = 50;
	[SerializeField] private Color _color = new Color(0.5f, 0.5f, 0.5f);


	// ================================================================================================================
	// MAIN EVENT INTERFACE -------------------------------------------------------------------------------------------

	void Awake() {
	}


	// ================================================================================================================
	// EDITOR INTERFACE -----------------------------------------------------------------------------------------------

	#if UNITY_EDITOR

	[MenuItem("GameObject/Create Other/Circle")]
	public static void createNew() {
		var newObject = createNewGameObjectWithScript("Circle");
		Undo.RegisterCreatedObjectUndo(newObject, "Create Circle");
		Selection.objects = new GameObject[] { newObject };
	}

	#endif


	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	public static GameObject createNewGameObjectWithScript(string name) {
		var gameObject = createNewGameObject(name);
		gameObject.AddComponent<Circle>();
		return gameObject;
	}


	// ================================================================================================================
	// ACCESSOR INTERFACE ---------------------------------------------------------------------------------------------

	public float radius {
		get { return _radius; }
		set {
			_radius = value;
			requestMeshGeneration();
		}
	}

	public int segments {
		get { return _segments; }
		set {
			_segments = value;
			requestMeshGeneration();
		}
	}

	public Color color {
		get { return _color; }
		set {
			_color = value;
			requestMeshGeneration();
		}
	}


	// ================================================================================================================
	// EXTENDED INTERFACE ---------------------------------------------------------------------------------------------
	
	protected override void updateMesh(Mesh mesh) {
		generateMesh(mesh);
	}


	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

	private void generateMesh(Mesh mesh) {
		// Generate the entire mesh
		if (mesh != null) {
			int segs = Math.Max(4, _segments);
			float rad = Math.Max(0, _radius);
			if (rad == 0) segs = 0;
			if (segs == 0) rad = 0;
			int numVerticesTotal = segs + 1;
			int numTrianglesTotal = segs;

			Vector3[] vertices = new Vector3[numVerticesTotal];
			int[] triangles = new int[numTrianglesTotal * 3];
			Color[] colors = new Color[numVerticesTotal];

			// First vertex
			vertices[0].x = 0;
			vertices[0].y = 0;
			vertices[0].z = 0;

			// All other vertices
			for (int i = 0; i < segs; i++) {
				double f = (double)i / (double)segs;
				vertices[i+1].x = (float)Math.Sin(f * 2.0 * Math.PI) * _radius;
				vertices[i+1].y = (float)Math.Cos(f * 2.0 * Math.PI) * _radius;
				vertices[i+1].z = 0;
			}

			// Triangles
			int trianglesPosition = 0;
			for (int i = 0; i < numTrianglesTotal; i++) {
				triangles[trianglesPosition++] = 0;
				triangles[trianglesPosition++] = i + 1;
				if (i + 2 < numVerticesTotal) {
					triangles[trianglesPosition++] = i + 2;
				} else {
					triangles[trianglesPosition++] = (i + 2) % numTrianglesTotal;
				}
			}

			// Colors
			for (int i = 0; i < numVerticesTotal; i++) {
				colors[i] = _color;
			}

			// Create mesh
			mesh.Clear();
			mesh.vertices = vertices;
			mesh.colors = colors;
			mesh.SetTriangles(triangles, 0);
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			mesh.Optimize();

			updateUVs(mesh);
		}
	}

	private void updateUVs(Mesh mesh) {
		// Update the uvs (uses the same locations)
		Vector3[] vertices = mesh.vertices;
		Vector2[] uvs = new Vector2[vertices.Length];

		for (int i = 0; i < uvs.Length; i++) {
			uvs[i] = new Vector2(vertices[i].x, vertices[i].y);
		}

		mesh.uv = uvs;
	}
}