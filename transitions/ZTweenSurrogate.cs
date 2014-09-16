using System.Collections.Generic;
using UnityEngine;

public class ZTweenSurrogate:MonoBehaviour {

	// Tween surrogate, to control its updating

	private List<ZTween.ZTweenSequence> tweenSequences = new List<ZTween.ZTweenSequence>();

	void Update() {
		for (int i = 0; i < tweenSequences.Count; i++) {
			if (tweenSequences[i] != null) {
				tweenSequences[i].update();
			} else {
				tweenSequences.RemoveAt(i);
				i--;
			}
		}
	}

	internal void add(ZTween.ZTweenSequence tweenSequence) {
		tweenSequences.Add(tweenSequence);
	}

	internal void remove(ZTween.ZTweenSequence tweenSequence) {
		// Nullify first, remove later - otherwise it gets remove while doing Update(), which can cause the list to trip on itself
		tweenSequences[tweenSequences.IndexOf(tweenSequence)] = null;
	}
}