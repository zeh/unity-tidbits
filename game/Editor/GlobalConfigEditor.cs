using System;
using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(GlobalConfigGO))]
public class GlobalConfigEditor:Editor {

	// ================================================================================================================
	// MAIN EVENT INTERFACE -------------------------------------------------------------------------------------------

	public override void OnInspectorGUI() {
		GlobalConfigGO config = (GlobalConfigGO) target;

		var lw = GUILayout.Width(Screen.width * 0.3f);
		var cw = GUILayout.Width(Screen.width * 0.21f);
		var rw = GUILayout.Width(Screen.width * 0.3f);
		var b1 = GUILayout.Width(Screen.width * 0.05f);
		var b2 = GUILayout.Width(Screen.width * 0.076f);
		var b3 = GUILayout.Width(Screen.width * 0.075f);

		// Header
		GUILayout.BeginHorizontal();
		GUILayout.Label("ID", EditorStyles.label, lw);
		GUILayout.Label("Type", EditorStyles.label, cw);
		GUILayout.Label("Value", EditorStyles.label, rw);
		GUILayout.EndHorizontal();

		// Lists entries
		for (var i = 0; i < config.getNumEntries(); i++) {
			//config.getEntryAt(i).name = GUILayout.TextField(config.getEntryAt(i).name);
			GUILayout.BeginHorizontal();

			// ID
			config.getEntryAt(i).name = GUILayout.TextField(config.getEntryAt(i).name, lw);
	
			// Type
			config.getEntryAt(i).type = (GlobalConfigGO.ConfigEntry.Types)EditorGUILayout.EnumPopup(config.getEntryAt(i).type, EditorStyles.popup, cw);

			// Value
			switch (config.getEntryAt(i).type) {
				case GlobalConfigGO.ConfigEntry.Types.String:
				case GlobalConfigGO.ConfigEntry.Types.Int:
				case GlobalConfigGO.ConfigEntry.Types.Float:
					config.getEntryAt(i).valueString = GUILayout.TextField(config.getEntryAt(i).valueString, rw);
					break;
				case GlobalConfigGO.ConfigEntry.Types.Boolean:
					config.getEntryAt(i).valueBool = GUILayout.Toggle(config.getEntryAt(i).valueBool, "True", rw);
					break;
			}

			// Buttons
			if (GUILayout.Button("-", b1)) {
			}
			if (GUILayout.Button("UP", b2)) {
			}
			if (GUILayout.Button("DOWN", b3)) {
			}

			// End
			GUILayout.EndHorizontal();
		}

		// Allow adding entries
		if (GUILayout.Button("Add entry")) {
			config.addEntry(new GlobalConfigGO.ConfigEntry());
		}

		if (GUI.changed) config.save();

	}
}
