using UnityEngine;
using System.Collections.Generic;

public class ServiceLocator:MonoBehaviour {

	// Properties
	public List<MonoBehaviour> _services;

	private static ServiceLocator instance;


	// ================================================================================================================
	// MAIN EVENT INTERFACE -------------------------------------------------------------------------------------------

	void Awake() {
		instance = this;
	}


	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	public static T get<T>() where T : MonoBehaviour {
		List<MonoBehaviour> services = instance._services;
		for (int i = 0; i < services.Count; i++) {
			if (services[i] is T) return (T)services[i];
		}
		Debug.LogError("Tried locating a service of type " + typeof(T) + " that could not be found!");
		return default(T);
	}
}
