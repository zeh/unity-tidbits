using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[ExecuteInEditMode()]
#endif

public class CustomMeshObject:MonoBehaviour {

	/*
	A basic class for custom meshes (avoids most issues with editor errors). Meant to be extended.
	*/

	// Properties
	private MeshFilter meshFilter;
	private Mesh mesh;

	private bool isDirty = true;


	// ================================================================================================================
	// MAIN EVENT INTERFACE -------------------------------------------------------------------------------------------

	void Update() {
		if (Application.isEditor && !Application.isPlaying) {
			// Always update while editing
			generateMesh();
		}
	}

	void LateUpdate() {
		if (isDirty) {
			generateMesh();
		}
	}


	// ================================================================================================================
	// EDITOR INTERFACE -----------------------------------------------------------------------------------------------

	#if UNITY_EDITOR

	/*
	// Example:
	[MenuItem("GameObject/Create Other/Rounded Cube")]
	public static void createNew() {
		newObject = createNewGameObjectFromEditor("Rounded Cube");
		newObject.AddComponent<RoundedCube>();
	 
		// Make this action undoable
		Undo.RegisterCreatedObjectUndo(newObject, "Create " + name);

		// Select the new object
		Selection.objects = new GameObject[] { newObject };

		return newObject;
	}
	*/

	#endif



	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	public static GameObject createNewGameObject(string name) {
		// Create the actual object
		GameObject newObject = new GameObject(name);

		// Create mesh filter (with no mesh yet)
		newObject.AddComponent<MeshFilter>();

		// Create mesh renderer
		newObject.AddComponent<MeshRenderer>();

//		Debug.Log("Creating, class =======> " + MethodBase.GetCurrentMethod().DeclaringType);

		/*
		// Create mesh collider
		newObject.AddComponent<MeshCollider>();
		*/

		return newObject;
	}
	

	// ================================================================================================================
	// EXTENDABLE INTERFACE -------------------------------------------------------------------------------------------
	
	// EXTEND with override
	protected virtual void updateMesh(Mesh mesh) {
		mesh.Clear();
		/*
		mesh.vertices = vertices;
		mesh.colors = colors;
		mesh.SetTriangles(triangles, 0);
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.Optimize();
		mesh.uv = uvs;
		*/
	}


	// ================================================================================================================
	// INTERNAL INTERFACE FOR EXTENDED --------------------------------------------------------------------------------
	
	protected void requestMeshGeneration() {
		isDirty = true;
	}


	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

	private void generateMesh() {
		// Generate the entire mesh
		if (meshFilter == null) meshFilter = gameObject.GetComponent<MeshFilter>();
		if ((mesh == null && meshFilter != null) || (meshFilter != null && meshFilter.sharedMesh == null)) {
			if (meshFilter.sharedMesh == null) {
				// Need to create a mesh first
				Mesh newMesh = new Mesh();
				newMesh.name = name + " mesh";
				meshFilter.mesh = newMesh;
			}

			// Create own mesh reference
			if (Application.isPlaying) {
				mesh = meshFilter.mesh;
			} else {
				// Avoid stupid warnings about leaks by making a deep copy
				// The old one will still be garbage collected later when the scene is saved
				Mesh meshCopy = (Mesh) Mesh.Instantiate(meshFilter.sharedMesh);
				meshCopy.name = meshFilter.sharedMesh.name;
				mesh = meshFilter.mesh = meshCopy;
			}
		}

		isDirty = false;
		updateMesh(mesh);
	}
}