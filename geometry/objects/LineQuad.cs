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

public class LineQuad:CustomMeshObject {

	/*
	A line with an origin point and correct tiled mapping
	*/

	// Constants
	public static Color COLOR_DEFAULT = new Color(1, 0.5f, 0.5f, 1);
	public static float THICKNESS_DEFAULT = 0.1f;

	// Properties
	private float _length;
	private float _thickness;
	private Vector2 _start;
	private Vector2 _end;
	private Color _color;


	// ================================================================================================================
	// MAIN EVENT INTERFACE -------------------------------------------------------------------------------------------

	void Awake() {
		_thickness = THICKNESS_DEFAULT;
		_color = COLOR_DEFAULT;
		_start = new Vector2(0, 0);
		_end = new Vector2(0, 0);
	}


	// ================================================================================================================
	// EDITOR INTERFACE -----------------------------------------------------------------------------------------------

	#if UNITY_EDITOR

	[MenuItem("GameObject/Create Other/Line Quad")]
	public static void createNew() {
		var newObject = createNewGameObjectWithScript("Line Quad");
		Undo.RegisterCreatedObjectUndo(newObject, "Create Line Quad");
		Selection.objects = new GameObject[] { newObject };
	}

	#endif



	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	public static GameObject createNewGameObjectWithScript(string name) {
		var gameObject = createNewGameObject(name);
		gameObject.AddComponent<LineQuad>();
		return gameObject;
	}


	// ================================================================================================================
	// ACCESSOR INTERFACE ---------------------------------------------------------------------------------------------

	public float length {
		get {
			return _length;
		}
	}
	
	// TODO: allow setting the length!

	public float thickness {
		get { return _thickness; }
		set {
			_thickness = value;
			requestMeshGeneration();
		}
	}

	public Vector2 start {
		get { return _start; }
		set {
			_start = value;
			updatePositions();
		}
	}

	public Vector2 end {
		get { return _end; }
		set {
			_end = value;
			updatePositions();
		}
	}

	public Color color {
		get { return _color; }
		set {
			_color = value;
			updateColor();
		}
	}


	// ================================================================================================================
	// EXTENDED INTERFACE ---------------------------------------------------------------------------------------------
	
	protected override void updateMesh(Mesh mesh) {
		generateMesh(mesh);
	}


	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

	private void updatePositions() {
		// Generates the whole line

		_length = Vector2.Distance(_start, _end);
		transform.localPosition = new Vector3(_start.x, _start.y, 0);
		transform.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(_end.y - _start.y, _end.x - _start.x) * Mathf.Rad2Deg);
		
		requestMeshGeneration();
	}

	private void generateMesh(Mesh mesh) {
		// Create vertices
		Vector3[] vertices = new Vector3[] {
			new Vector3(0,			-_thickness / 2,	0),
			new Vector3(_length,	-_thickness / 2,	0),
			new Vector3(0,			_thickness / 2,		0),
			new Vector3(_length,	_thickness / 2,		0)
		};

		// Create triangles
		int[] triangles = new int[] {0, 2, 1, 1, 2, 3};
	
		// Create mesh
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.SetTriangles(triangles, 0);
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();

		updateUVs(mesh);
		updateColor();
	}

	private void updateUVs(Mesh mesh) {
		// Update the uvs
		float uvLength = _length / _thickness / 2f;
		Vector2[] uvs = new Vector2[] {
			new Vector2(0, 0),
			new Vector2(uvLength, 0),
			new Vector2(0, 1),
			new Vector2(uvLength, 1)
		};
		mesh.uv = uvs;
	}

	private void updateColor() {
		renderer.materials[0].color = _color;
	}
}