using System;
using UnityEngine;

public class D {
	
	public static void Log(object __message) {
		if (!Debug.isDebugBuild) return;

		// Builds parameters
		System.Diagnostics.StackTrace t = new System.Diagnostics.StackTrace();
		string method = t.ToString().Split('\n')[1];
		method = method.Substring(method.IndexOf("at ") + 3);
		
		string currentFrame = Time.frameCount.ToString("0000000");
		string currentTime = (Time.realtimeSinceStartup * 1000).ToString("00000");
			
		// Finally, composes line
		string fullMessage = "[" + currentFrame + "@" + currentTime + "] " +  method + " :: " + __message;
		//string fullMessage = "[" + currentFrame + ":" + currentTime + " | " +  method + ": " + __message;
		
		// Writes to Unity console log and to normal log
		Debug.Log(fullMessage + "\n");
		System.Console.WriteLine(fullMessage);
		
		//Debug.Log("Flat Mesh Initialized (d)\n"); // UnityEngine.Debug.Log
		//System.Console.WriteLine("Flat Mesh Initialized");
		// Debug.isDebugBuild
		// http://docs.unity3d.com/Documentation/ScriptReference/Application.RegisterLogCallback.html

		
		// Port LoggingFramework:
		// http://forum.unity3d.com/threads/38720-Debug.Log-and-needless-spam

	}
	
}

