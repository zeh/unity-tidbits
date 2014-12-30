using UnityEngine;
using System.Collections;
using System.IO;

public class ValueListGO:MonoBehaviour {

	// Properties
	public string filename;
	public string name;


	// ================================================================================================================
	// MAIN EVENT INTERFACE -------------------------------------------------------------------------------------------

	void Start() {
		var valueList = new ValueList(name);
		valueList.SetFromJSON(File.ReadAllText(filename));

		//Debug.Log("====> " + ValueList.getInstance().GetString("language.systems.hepburn"));
	}
}
