using System;
using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(RoundedCube))]
public class RoundedCubeEditor:Editor {

	// ================================================================================================================
	// MAIN EVENT INTERFACE -------------------------------------------------------------------------------------------

	public override void OnInspectorGUI() {
		RoundedCube roundedCube = (RoundedCube) target;

		roundedCube.width			= EditorGUILayout.FloatField("Width", roundedCube.width);
		roundedCube.height			= EditorGUILayout.FloatField("Height", roundedCube.height);
		roundedCube.depth			= EditorGUILayout.FloatField("Depth", roundedCube.depth);
		roundedCube.cornerRadius	= EditorGUILayout.FloatField("Corner radius", roundedCube.cornerRadius);
		roundedCube.offsetTopX		= EditorGUILayout.FloatField("Top Offset (X)", roundedCube.offsetTopX);
		roundedCube.offsetTopY		= EditorGUILayout.FloatField("Top Offset (Y)", roundedCube.offsetTopY);
		roundedCube.cornerSegments	= EditorGUILayout.IntSlider("Corner segments", roundedCube.cornerSegments, 0, 32);
		roundedCube.color			= EditorGUILayout.ColorField("Color", roundedCube.color);

		if (GUI.changed) {
			EditorUtility.SetDirty(roundedCube);
		}

		/*
		serializedObject.Update();

		// Totals
		//GUILayout.LabelField("Triangles", 10.ToString());

		//GUILayout.BeginVertical("Terrain", "box");
		//GUILayout.Space(20);
		//GUILayout.EndVertical();

		// Terrain info
		//terrainTarget.title = EditorGUILayout.TextField("Terrain title", terrainTarget.title);

		// Other
		terrainTarget.setTileTypeList((TiledTerrainTileTypes)EditorGUILayout.ObjectField("List of tile types", terrainTarget.getTileTypeList(), typeof(TiledTerrainTileTypes), true));

		//((TiledTerrain)target).ping();//lookAtPoint = EditorGUILayout.Vector3Field ("Look At Point", target.lookAtPoint);
		serializedObject.ApplyModifiedProperties();
		*/

	}
}
