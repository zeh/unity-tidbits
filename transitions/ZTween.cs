using System;
using System.Collections.Generic;
using UnityEngine;

class ZTween {

	/**
	 * That works:
	 * 
	 * .use(gameObject)
	 * .scaleTo(Vector3, time = 0, transition = Equations.none)
	 * .scaleFrom(Vector3)
	 * .moveTo(Vector3, time = 0, transition = Equations.none)
	 * .moveFrom(Vector3)
	 * .call(Action)
	 * .wait(time)
	 * 
	 * use(ref float)
	 * use(getter, setter)
	 * .
	 * //.play()
	 * //.pause()
	 * 
	 */

	/**
	 * Examples:
	 * .call(() => logDone("over"))
	 * .call(someFunction)
	 * // Getter/setter, for function pairs
	 * ZTween.use(getValue, setValue).valueTo(1.0f, 1, Easing.quadOut).call(() => logDone("value ok"));
	 * // Lambdas, for getter-setter or member pairs
	 * ZTween.use(() => testNum, x => testNum = x).valueTo(1.0f, 1, Easing.quadOut).call(() => logDone("value ok"));
	 */

	/*
	//transform.localScale = new Vector3(2, 2, 2);

	// http://www.createjs.com/Docs/TweenJS/modules/TweenJS.html

	ZTween.use(gameObject).scaleTo(new Vector3(2, 2, 2), 1.0f, Easing.quadOut).wait(1).call(Func).set("visible", false).play();
	ZTween.use(gameObject).pause();
	ZTween.use(gameObject).resume();
	ZTween.use(gameObject).stop();
	ZTween.use(gameObject, loop, useTicks);
	*/

	#region ZTween static interface

	// Static properties
	private static ZTweenSurrogate tweenSurrogate;


	// ================================================================================================================
	// PUBLIC STATIC INTERFACE ----------------------------------------------------------------------------------------

	public static ZTweenGameObjectSequence use(GameObject gameObject) {
		return new ZTweenGameObjectSequence(gameObject);
	}

	public static ZTweenGetterSetterSequence use(Func<float> getValueFunction, Action<float> setValueFunction) {
		return new ZTweenGetterSetterSequence(getValueFunction, setValueFunction);
	}


	// ================================================================================================================
	// PRIVATE STATIC INTERFACE ---------------------------------------------------------------------------------------

	static ZTween() {
		GameObject surrogateObject = new GameObject("ZTween-controller");
		tweenSurrogate = surrogateObject.AddComponent<ZTweenSurrogate>();
	}

	#endregion


	// ================================================================================================================
	// INTERNAL CLASSES -----------------------------------------------------------------------------------------------

	// Aux classes

	class ZTweenStepMetadata {
		public bool hasStarted;
		public bool hasCompleted;
		public float timeStart;
		public float timeEnd;
		public float timeDuration {
			get {
				return timeEnd - timeStart;
			}
		}
	}

	internal interface IZTweenStep {
		void start();
		void update(float t);
		void end();
		float getDuration();
	}

	// Sequences

	internal class ZTweenSequence {

		// Instances
		private List<IZTweenStep> sequenceSteps;
		private List<ZTweenStepMetadata> sequenceStepsMetadatas;

		// Properties
		private bool isPlaying;
		private int currentStep;
		private float startTime;
		private float pauseTime;
		private float executedTime;
		private float duration;


		// ================================================================================================================
		// CONSTRUCTOR ----------------------------------------------------------------------------------------------------

		public ZTweenSequence() {
			// Create a new ZTween

			// Add to surrogate
			ZTween.tweenSurrogate.add(this);

			// Create lists
			sequenceSteps = new List<IZTweenStep>();
			sequenceStepsMetadatas = new List<ZTweenStepMetadata>();

			// Set properties
			isPlaying = true;
			currentStep = 0;
			startTime = Time.time;
			executedTime = 0;
			duration = 0;
		}


		// ================================================================================================================
		// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

		// Play control methods

		public ZTweenSequence play() {
			if (!isPlaying) {
				isPlaying = true;

				float timePaused = Time.time - pauseTime;
				startTime += timePaused;
			}
			return this;
		}

		public ZTweenSequence pause() {
			if (isPlaying) {
				isPlaying = false;

				pauseTime = Time.time;
			}
			return this;
		}


		// Utility methods

		public ZTweenSequence call(Action action) {
			addStep(new ZTweenStepCall(action));
			return this;
		}

		public ZTweenSequence wait(float duration) {
			duration += duration;
			return this;
		}


		// ================================================================================================================
		// PRIVATE INTERFACE ----------------------------------------------------------------------------------------------

		// Core tween step control methods

		internal void addStep(IZTweenStep step) {
			sequenceSteps.Add(step);

			ZTweenStepMetadata tweenMetadata = new ZTweenStepMetadata();
			tweenMetadata.timeStart = startTime + duration;
			duration += step.getDuration();
			tweenMetadata.timeEnd = startTime + duration;

			sequenceStepsMetadatas.Add(tweenMetadata);
		}

		internal void update() {
			// Update current step(s) based on the time

			// Check if finished
			if (currentStep >= sequenceSteps.Count) {
				// Finished all, remove itself
				destroy();
			} else {

				bool shouldUpdateOnce = isPlaying;

				while (shouldUpdateOnce && currentStep < sequenceSteps.Count) {
					shouldUpdateOnce = false;

					if (Time.time >= sequenceStepsMetadatas[currentStep].timeStart) {
						// Start the current tween step if necessary
						if (!sequenceStepsMetadatas[currentStep].hasStarted) {
							sequenceSteps[currentStep].start();
							sequenceStepsMetadatas[currentStep].hasStarted = true;
						}

						// Update the current tween step
						sequenceSteps[currentStep].update(Mathf.Clamp01(Mathf.InverseLerp(sequenceStepsMetadatas[currentStep].timeStart, sequenceStepsMetadatas[currentStep].timeEnd, Time.time)));

						// Check if it's finished
						if (Time.time >= sequenceStepsMetadatas[currentStep].timeEnd) {
							if (!sequenceStepsMetadatas[currentStep].hasCompleted) {
								sequenceSteps[currentStep].end();
								sequenceStepsMetadatas[currentStep].hasCompleted = true;
								executedTime += sequenceStepsMetadatas[currentStep].timeDuration;
								shouldUpdateOnce = true;
								currentStep++;
							}
						}
					}
				}
			}
		}

		internal void destroy() {
			tweenSurrogate.remove(this);
		}

		internal Func<float, float> getTransition(Func<float, float> transition) {
			return transition == null ? Easing.none : transition;
		}

	}

	internal class ZTweenGetterSetterSequence:ZTweenSequence {

		// Instances
		private Func<float> targetGet;
		private Action<float> targetSet;


		// ================================================================================================================
		// CONSTRUCTOR ----------------------------------------------------------------------------------------------------

		public ZTweenGetterSetterSequence(Func<float> targetGet, Action<float> targetSet):base() {
			this.targetGet = targetGet;
			this.targetSet = targetSet;
		}


		// ================================================================================================================
		// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

		public ZTweenGetterSetterSequence valueFrom(float value) {
			addStep(new ZTweenStepValueFrom(targetSet, value));
			return this;
		}

		public ZTweenGetterSetterSequence valueTo(float value, float duration = 0, Func<float, float> transition = null) {
			addStep(new ZTweenStepValueTo(targetGet, targetSet, value, duration, getTransition(transition)));
			return this;
		}
	}

	internal class ZTweenGameObjectSequence:ZTweenSequence {

		// Instances
		private GameObject targetGameObject;


		// ================================================================================================================
		// CONSTRUCTOR ----------------------------------------------------------------------------------------------------

		public ZTweenGameObjectSequence(GameObject gameObject):base() {
			this.targetGameObject = gameObject;
		}

		// ================================================================================================================
		// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

		public ZTweenGameObjectSequence scaleFrom(Vector3 scale) {
			addStep(new ZTweenStepScaleFrom(targetGameObject, scale));
			return this;
		}

		public ZTweenGameObjectSequence scaleTo(Vector3 scale, float duration = 0, Func<float, float> transition = null) {
			addStep(new ZTweenStepScaleTo(targetGameObject, scale, duration, getTransition(transition)));
			return this;
		}

		public ZTweenGameObjectSequence moveFrom(Vector3 scale) {
			addStep(new ZTweenStepPositionFrom(targetGameObject, scale));
			return this;
		}

		public ZTweenGameObjectSequence moveTo(Vector3 scale, float duration = 0, Func<float, float> transition = null) {
			addStep(new ZTweenStepPositionTo(targetGameObject, scale, duration, getTransition(transition)));
			return this;
		}

	}

	// Steps for generic sequences

	class ZTweenStepCall:IZTweenStep {

		// Properties
		private Action action;

		// Extension functions
		public ZTweenStepCall(Action action) {
			this.action = action;
		}

		public void start() { }

		public void update(float t) { }

		public void end() {
			action();
		}

		public float getDuration() {
			return 0;
		}
	}

	// Steps for GetterSetter sequences

	class ZTweenStepValueFrom:IZTweenStep {

		// Properties
		private Action<float> targetSet;
		private float targetValue;

		// Extension functions
		public ZTweenStepValueFrom(Action<float> targetSet, float targetValue) {
			this.targetSet = targetSet;
			this.targetValue = targetValue;
		}

		public void start() { }

		public void update(float t) { }

		public void end() {
			targetSet(targetValue);
		}

		public float getDuration() {
			return 0;
		}
	}

	class ZTweenStepValueTo:IZTweenStep {

		// Properties
		private Func<float> targetGet;
		private Action<float> targetSet;
		private float duration;
		private float startValue;
		private float targetValue;
		private Func<float, float> transition;

		// Extension functions
		public ZTweenStepValueTo(Func<float> targetGet, Action<float> targetSet, float targetValue, float duration, Func<float, float> transition) {
			this.targetGet = targetGet;
			this.targetSet = targetSet;
			this.duration = duration;
			this.targetValue = targetValue;
			this.transition = transition;
		}

		public void start() {
			this.startValue = targetGet();
		}

		public void update(float t) {
			targetSet(MathUtils.lerp(startValue, targetValue, transition(t)));
		}

		public void end() {
			targetSet(targetValue);
		}

		public float getDuration() {
			return duration;
		}
	}

	// Steps for GameObject sequences

	class ZTweenStepScaleFrom:IZTweenStep {

		// Properties
		private GameObject target;
		private Vector3 targetValue;

		// Extension functions
		public ZTweenStepScaleFrom(GameObject target, Vector3 targetValue) {
			this.target = target;
			this.targetValue = targetValue;
		}

		public void start() { }

		public void update(float t) { }

		public void end() {
			target.transform.localScale = targetValue;
		}

		public float getDuration() {
			return 0;
		}
	}

	class ZTweenStepScaleTo:IZTweenStep {

		// Properties
		private GameObject target;
		private float duration;
		private Vector3 startValue;
		private Vector3 targetValue;
		private Vector3 tempValue;
		private Func<float, float> transition;

		// Extension functions
		public ZTweenStepScaleTo(GameObject target, Vector3 targetValue, float duration, Func<float, float> transition) {
			this.target = target;
			this.duration = duration;
			this.targetValue = targetValue;
			this.transition = transition;
		}

		public void start() {
			this.startValue = target.transform.localScale;
			this.tempValue = new Vector3();
		}

		public void update(float t) {
			MathUtils.applyLerp(startValue, targetValue, transition(t), ref tempValue);
			target.transform.localScale = tempValue;
		}

		public void end() {
			target.transform.localScale = targetValue;
		}

		public float getDuration() {
			return duration;
		}
	}

	class ZTweenStepPositionFrom:IZTweenStep {

		// Properties
		private GameObject target;
		private Vector3 targetValue;

		// Extension functions
		public ZTweenStepPositionFrom(GameObject target, Vector3 targetValue) {
			this.target = target;
			this.targetValue = targetValue;
		}

		public void start() { }

		public void update(float t) { }

		public void end() {
			target.transform.localPosition = targetValue;
		}

		public float getDuration() {
			return 0;
		}
	}

	class ZTweenStepPositionTo:IZTweenStep {

		// Properties
		private GameObject target;
		private float duration;
		private Vector3 startValue;
		private Vector3 targetValue;
		private Vector3 tempValue;
		private Func<float, float> transition;

		// Extension functions
		public ZTweenStepPositionTo(GameObject target, Vector3 targetValue, float duration, Func<float, float> transition) {
			this.target = target;
			this.duration = duration;
			this.targetValue = targetValue;
			this.transition = transition;
		}

		public void start() {
			this.startValue = target.transform.localPosition;
			this.tempValue = new Vector3();
		}

		public void update(float t) {
			MathUtils.applyLerp(startValue, targetValue, transition(t), ref tempValue);
			target.transform.localPosition = tempValue;
		}

		public void end() {
			target.transform.localPosition = targetValue;
		}

		public float getDuration() {
			return duration;
		}
	}

	// Auxiliary functions

	class MathUtils {
		public static float lerp(float start, float end, float t) {
			// Lerp: needed because Mathf.lerp clamps between 0 and 1
			return start + (end - start) * t;
		}

		public static void applyLerp(Vector3 start, Vector3 end, float t, ref Vector3 receiver) {
			// Lerp: needed because Mathf.lerp clamps between 0 and 1
			// Dumps into a target to avoid GC
			receiver.x = start.x + (end.x - start.x) * t;
			receiver.y = start.y + (end.y - start.y) * t;
			receiver.z = start.z + (end.z - start.z) * t;
		}
	}
}
