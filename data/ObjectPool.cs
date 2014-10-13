using System;
using System.Collections.Generic;

class ObjectPool<T> {

	// Instances
	private List<T> objects;
	private List<bool> objectsUsed;

	private int numObjectsFree;
	
	private Func<T> create;

	// Delegate types
	public event Action<T> OnGet;
	public event Action<T> OnPut;
	public event Action<T> OnClear;


	// ================================================================================================================
	// CONSTRUCTOR ----------------------------------------------------------------------------------------------------

	public ObjectPool(Func<T> createFunction = null) {
		objects = new List<T>();
		objectsUsed = new List<bool>();
		numObjectsFree = 0;
		create = createFunction;
	}


	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	public T Get() {
		// Returns an unused object from the existing pool, or creating a new if none is available
		if (numObjectsFree == 0) {
			// No objects free, create a new one
			if (create != null) {
				int pos = addObject(create());
				return getObjectAt(pos);
			} else {
				return default(T);
			}
		} else {
			// Find first unused object
			for (int i = 0; i < objectsUsed.Count; i++) {
				if (!objectsUsed[i]) return getObjectAt(i);
			}
		}
		return default(T);
	}

	public void Put(T obj) {
		// Put an object back in the pool
		int index = objects.IndexOf(obj);
		if (index > -1) {
			// Object is in the pool, just put it back
			putObject(index, obj);
		} else {
			// Object is not in the pool yet, add it
			addObject(obj);
		}
	}

	public void Clear() {
		while (objects.Count > 0) clearObjectAt(0);
		objects.Clear();
		objectsUsed.Clear();
		numObjectsFree = 0;
	}

	public int GetNumObjectsFree() {
		return numObjectsFree;
	}

	public int GetNumObjects() {
		return objects.Count;
	}


	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

	private int addObject(T obj) {
		objects.Add(obj);
		objectsUsed.Add(false);
		numObjectsFree++;
		return objects.Count - 1;
	}

	private T getObjectAt(int index) {
		if (!objectsUsed[index]) {
			objectsUsed[index] = true;
			numObjectsFree--;
		}
		if (OnGet != null) OnGet(objects[index]);
		return objects[index];
	}

	private void putObject(int pos, T obj) {
		if (objectsUsed[pos]) {
			objectsUsed[pos] = false;
			numObjectsFree++;
		}
		if (OnPut != null) OnPut(obj);
	}

	private void clearObjectAt(int index) {
		if (OnClear != null) OnClear(objects[index]);
		objects.RemoveAt(index);
		objectsUsed.RemoveAt(index);
	}

}
