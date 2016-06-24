using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
[ExecuteInEditMode()]
#endif

public class MeshColliderCreator : MonoBehaviour {

	// Automatically add mesh colliders to all child meshes

	// TODO: actually remove this file

	// Properties
	public bool actOnChildren;

	// ================================================================================================================
	// MAIN EVENT INTERFACE -------------------------------------------------------------------------------------------

	#if UNITY_EDITOR
	void Start () {
		createColliders();
	}

	void Update() {
		if (!Application.isPlaying) createColliders();
	}
	#endif

	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

	private void createColliders() {
		GameObjectUtils.createMeshColliders(gameObject, actOnChildren);
	}
}
