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

public class RoundedCube:CustomMeshObject {

	/*
	A cube with rounded corners
	*/

	// Properties
	[SerializeField] private float _width = 100;
	[SerializeField] private float _height = 100;
	[SerializeField] private float _depth = 50;
	[SerializeField] private float _offsetTopX = 0;
	[SerializeField] private float _offsetTopY = 0;
	[SerializeField] private float _cornerRadius = 10;
	[SerializeField] private int _cornerSegments = 5;
	[SerializeField] private Color _color = new Color(0.5f, 0.5f, 0.5f);


	// ================================================================================================================
	// MAIN EVENT INTERFACE -------------------------------------------------------------------------------------------

	void Awake() {
	}


	// ================================================================================================================
	// EDITOR INTERFACE -----------------------------------------------------------------------------------------------

	#if UNITY_EDITOR

	[MenuItem("GameObject/Create Other/Rounded Cube")]
	public static void createNew() {
		var newObject = createNewGameObjectWithScript("Line Rounded Cube");
		Undo.RegisterCreatedObjectUndo(newObject, "Create Rounded Cube");
		Selection.objects = new GameObject[] { newObject };
	}

	#endif


	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	public static GameObject createNewGameObjectWithScript(string name) {
		var gameObject = createNewGameObject(name);
		gameObject.AddComponent<RoundedCube>();
		return gameObject;
	}


	// ================================================================================================================
	// ACCESSOR INTERFACE ---------------------------------------------------------------------------------------------

	public float width {
		get { return _width; }
		set {
			_width = value;
			requestMeshGeneration();
		}
	}

	public float height {
		get { return _height; }
		set {
			_height = value;
			requestMeshGeneration();
		}
	}

	public float depth {
		get { return _depth; }
		set {
			_depth = value;
			requestMeshGeneration();
		}
	}

	public float offsetTopX {
		get { return _offsetTopX; }
		set {
			_offsetTopX = value;
			requestMeshGeneration();
		}
	}

	public float offsetTopY {
		get { return _offsetTopY; }
		set {
			_offsetTopY = value;
			requestMeshGeneration();
		}
	}

	public float cornerRadius {
		get { return _cornerRadius; }
		set {
			_cornerRadius = value;
			requestMeshGeneration();
		}
	}

	public int cornerSegments {
		get { return _cornerSegments; }
		set {
			_cornerSegments = value;
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
		bool hasTop = true;
		bool hasSides = true;
		bool hasBottom = false;

		Color colorSide = new Color(_color.r * 0.75f, _color.g * 0.75f, _color.b * 0.75f, _color.a);

		if (mesh != null) {
			int segs = Math.Max(0, _cornerSegments);
			float radius = Math.Max(0, _cornerRadius);
			if (radius == 0) segs = 0;
			if (segs == 0) radius = 0;
			if (radius >= _width * 0.5f) radius = _width * 0.4999f;
			if (radius >= _height * 0.5f) radius = _height * 0.4999f;
			int verticesPerPlane = 4 + (segs * 4);
			int numVerticesTotal = (hasBottom ? verticesPerPlane : 0) + (hasTop ? verticesPerPlane : 0) + (hasSides ? verticesPerPlane * 2: 0); // For proper smoothing groups
			int numTrianglesTotal = (hasBottom ? verticesPerPlane-2 : 0) + (hasTop ? verticesPerPlane-2 : 0) + (hasSides ? verticesPerPlane * 2: 0);

			Vector3[] vertices = new Vector3[numVerticesTotal];
			int[] triangles = new int[numTrianglesTotal * 3];
			Color[] colors = new Color[numVerticesTotal];

			// Create vertice positions
			int verticesPosition = 0;
			int trianglesPosition = 0;
			int tempPos;

			// Create top
			if (hasTop) {
				tempPos = verticesPosition;
				addCapVerticesToArray(ref vertices, ref verticesPosition, segs, radius, _width, _height, _depth, _offsetTopX, _offsetTopY);
				addCapTrianglesToArray(ref triangles, ref trianglesPosition, tempPos, verticesPerPlane);
				addColorsToArray(ref colors, tempPos, verticesPerPlane, _color);
			}

			// Create sides
			if (hasSides && depth > 0) {
				tempPos = verticesPosition;
				addSideVerticesToArray(ref vertices, ref verticesPosition, segs, radius, _width, _height, _depth, _offsetTopX, _offsetTopY);
				addSideTrianglesToArray(ref triangles, ref trianglesPosition, tempPos, verticesPerPlane);
				addColorsToArray(ref colors, tempPos, verticesPerPlane * 2, colorSide);
			}

			// Create bottom
			if (hasBottom) {
				tempPos = verticesPosition;
				addCapVerticesToArray(ref vertices, ref verticesPosition, segs, radius, _width, _height, 0);
				addCapTrianglesToArray(ref triangles, ref trianglesPosition, tempPos, verticesPerPlane, true);
				addColorsToArray(ref colors, tempPos, verticesPerPlane, _color);
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

//			Debug.Log("Generated mesh with " + vertices.Length + " vertices.");
		}
	}

	private void addCapVerticesToArray(ref Vector3[] vertices, ref int position, int segments, float radius, float w, float h, float z, float offsetX = 0, float offsetY = 0) {
		// Create the top or bottom planes

		// TR
		addCornerVerticesToArray(ref vertices, ref position, segments, radius, w * 0.5f - radius + offsetX, -h * 0.5f + radius + offsetY, z);
		// BR
		addCornerVerticesToArray(ref vertices, ref position, segments, radius, w * 0.5f - radius + offsetX, h * 0.5f - radius + offsetY, z);
		// BL
		addCornerVerticesToArray(ref vertices, ref position, segments, radius, -w * 0.5f + radius + offsetX, h * 0.5f - radius + offsetY, z);
		// TL
		addCornerVerticesToArray(ref vertices, ref position, segments, radius, -w * 0.5f + radius + offsetX, -h * 0.5f + radius + offsetY, z);
	}

	private void addCapTrianglesToArray(ref int[] triangles, ref int position, int verticesPosition, int verticesPerPlane, bool flip = false) {
		int vp = 0;

		while (vp < verticesPerPlane - 2) {
			// Create zigzagging triangles
			int flipVal = flip ? 1 : 0; // Reverses triangle

			for (int i = 0; i < verticesPerPlane - 1; i++) {
				// Skip middle connection
				if (i == (verticesPerPlane / 2)-1) i++;

				triangles[position + 0] = vp + verticesPosition + i;
				triangles[position + 1 + flipVal] = vp + verticesPosition + i + 1;
				triangles[position + 2 - flipVal] = vp + verticesPosition + verticesPerPlane - i - 1;

				position += 3;
			}
			vp += verticesPerPlane;
		}
	}

	private void addSideVerticesToArray(ref Vector3[] vertices, ref int position, int segments, float radius, float w, float h, float d, float offsetTopX, float offsetTopY) {
		// Create the side
		addCapVerticesToArray(ref vertices, ref position, segments, radius, w, h, 0);
		addCapVerticesToArray(ref vertices, ref position, segments, radius, w, h, d, offsetTopX, offsetTopY);
	}

	private void addSideTrianglesToArray(ref int[] triangles, ref int position, int verticesPosition, int verticesPerPlane, bool flip = false) {
		// Create the triangles for the side

		int flipVal = flip ? 1 : 0; // Reverses triangle
		int r1, r2;
		for (int i = 0; i < verticesPerPlane; i++) {
			r1 = verticesPosition + i;
			r2 = verticesPosition + ((i + 1) % verticesPerPlane);

			triangles[position + 0] = r1;
			triangles[position + 1 + flipVal] = r2;
			triangles[position + 2 - flipVal] = r1 + verticesPerPlane;

			position += 3;

			triangles[position + 0] = r2;
			triangles[position + 1 + flipVal] = r2 + verticesPerPlane;
			triangles[position + 2 - flipVal] = r1 + verticesPerPlane;

			position += 3;
		}
	}

	private void addCornerVerticesToArray(ref Vector3[] vertices, ref int position, int segments, float radius, float x, float y, float z) {
		// Create the corner vertices
		if (segments == 0) {
			// No rounded corner at all
			vertices[position].x = x + (x > 0 ? radius : -radius);
			vertices[position].y = y + (y > 0 ? radius : -radius);
			vertices[position].z = z;
			position++;
		} else {
			const float HALF_PI = (float)(Math.PI * 0.5);

			float startingAngle;
			if (x > 0 && y > 0) {
				startingAngle = 0;
			} else if (x < 0 && y > 0) {
				startingAngle = HALF_PI;
			} else if (x < 0 && y < 0) {
				startingAngle = HALF_PI * 2f;
			} else {
				startingAngle = HALF_PI * 3f;
			}

			float angle;
			for (int i = 0; i <= segments; i++) {
				angle = startingAngle + HALF_PI * ((float)i / (float)segments);
				vertices[position].x = x + Mathf.Cos(angle) * radius;
				vertices[position].y = y + Mathf.Sin(angle) * radius;
				vertices[position].z = z;
				position++;
			}
		}
	}

	private void updateUVs(Mesh mesh) {
		// Update the uvs (uses the same locations)
		Vector3[] vertices = mesh.vertices;
		Vector2[] uvs = new Vector2[vertices.Length];

		for (int i = 0; i < uvs.Length; i++) {
			uvs[i] = new Vector2(vertices[i].x / _width, vertices[i].y / _height);
		}

		mesh.uv = uvs;
	}

	private void addColorsToArray(ref Color[] colors, int position, int numVertices, Color color) {
		//renderer.materials[0].color = _color;

		for (int i = 0; i < numVertices; i++) {
			colors[position + i] = color;
		}
	}
}