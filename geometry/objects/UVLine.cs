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

public class UVLine:MonoBehaviour {

	/*
	A line with an origin point: set start and end vectors. Uv mapping is adjusted
	*/

	// Constants
	public static Color COLOR_DRAGGING = new Color(0, 1, 1, 0.4f);
	public static Color COLOR_NORMAL = new Color(0, 1, 1, 1);
	public static Color COLOR_INVALID = new Color(1, 0.5f, 0.5f, 0.5f);
	public static float THICKNESS_DEFAULT = 16;

	// Properties
	private float _length;
	private float _thickness;
	private MeshFilter meshFilter;
	private Mesh mesh;
	private Vector2 _start;
	private Vector2 _end;
	private Color _color;


	// ================================================================================================================
	// MAIN EVENT INTERFACE -------------------------------------------------------------------------------------------

	void Awake() {
		_thickness = THICKNESS_DEFAULT;
		_color = COLOR_NORMAL;
		_start = new Vector2(0, 0);
		_end = new Vector2(0, 0);
	}

	void Start() {
		generate();
	}

	void Update() {
		if (Application.isEditor && !Application.isPlaying) {
			// Always update while editing
			generate();
		}
	}


	// ================================================================================================================
	// EDITOR INTERFACE -----------------------------------------------------------------------------------------------

	#if UNITY_EDITOR

	[MenuItem("GameObject/Create Other/Line")]
	static void CreateNew() {
		// Create the actual object
		GameObject newObject = new GameObject("Line");
		
		Mesh newMesh = new Mesh();
		newMesh.name = "Line mesh";

		MeshFilter newMeshFilter = newObject.AddComponent<MeshFilter>();
		newMeshFilter.mesh = newMesh;

		// Create mesh collider
		newObject.AddComponent<MeshCollider>();

		// Create mesh renderer
		newObject.AddComponent<MeshRenderer>();

		// Attach script
		UVLine newLineScript = newObject.AddComponent<UVLine>();
		newLineScript.start = new Vector2(0, 0);
		newLineScript.end = new Vector2(10, 0);

		// Make this action undoable
		Undo.RegisterCreatedObjectUndo(newObject, "Create Line");

		// Select the new object
		Selection.objects = new GameObject[] { newObject };
		//Selection.activeTransform = gameObject.transform;
	}

	#endif



	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	// ================================================================================================================
	// ACCESSOR INTERFACE ---------------------------------------------------------------------------------------------

	public float length {
		get {
			return _length;
		}
	}

	public float thickness {
		get {
			return _thickness;
		}
		set {
			_thickness = value;
			generateMesh();
		}
	}

	public Vector2 start {
		get {
			return _start;
		}
		set {
			_start = value;
			generate();
		}
	}

	public Vector2 end {
		get {
			return _end;
		}
		set {
			_end = value;
			generate();
		}
	}

	public Color color {
		get {
			return _color;
		}
		set {
			_color = value;
			updateColor();
		}
	}


	// ================================================================================================================
	// EXTENDABLE INTERFACE -------------------------------------------------------------------------------------------



	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

	private void generate() {
		// Generates the whole line

		_length = Vector2.Distance(_start, _end);
		transform.position = new Vector3(_start.x, 0, _start.y);
		transform.rotation = Quaternion.Euler(270, -Mathf.Atan2(_end.y - _start.y, _end.x - _start.x) * Mathf.Rad2Deg, 0);
		
		generateMesh();
	}

	private void generateMesh() {
		if (meshFilter == null) meshFilter = gameObject.GetComponent<MeshFilter>();
		if (mesh == null && meshFilter != null) mesh = Application.isPlaying ? meshFilter.mesh : meshFilter.sharedMesh;

		//if (mesh == null) mesh = (Application.isEditor && !Application.isPlaying) ? meshFilter.sharedMesh : meshFilter.mesh;

		if (mesh != null) {
			// Create vertices
			Vector3[] vertices = new Vector3[] {
				new Vector3(0,			-_thickness / 2,	0),
				new Vector3(_length,	-_thickness / 2,	0),
				new Vector3(0,			_thickness / 2,		0),
				new Vector3(_length,	_thickness / 2,		0)
			};

			// Create triangles
			int[] triangles = new int[] {0, 1, 2, 2, 1, 3};
		
			// Create mesh
			mesh.Clear();
			mesh.vertices = vertices;
			mesh.SetTriangles(triangles, 0);
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			mesh.Optimize();

			updateUVs();
			updateColor();

			//Debug.Log("Generated mesh, length = " + _length);
		}
	}

	private void updateUVs(float __uvOffsetX = 0) {
		// Update the uvs according to size and time
		float uvLength = _length / _thickness / 2f;
		Vector2[] uvs = new Vector2[] {
			new Vector2(__uvOffsetX, 0),
			new Vector2(__uvOffsetX + uvLength, 0),
			new Vector2(__uvOffsetX, 1),
			new Vector2(__uvOffsetX + uvLength, 1)
		};
		mesh.uv = uvs;
	}

	private void updateColor() {
		GetComponent<Renderer>().materials[0].color = _color;
	}
}