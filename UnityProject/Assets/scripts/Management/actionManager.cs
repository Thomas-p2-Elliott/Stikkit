using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal.Filters;
using System;

public class actionManager : MonoBehaviour {

	//	Public vars
	//Enable logs to console for actionManager
	public bool enableDebugLogs = true;


	//	Internal script vars
	//Used to prevent multiple events running at once
	private bool actionInProgress = false; 

	// Use this for initialization
	void Start () {
		LogAction("Started with debug logging enabled."); //LogAction only runs with debugging enabled
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void TouchEvent(Vector2 touchPos, float touchHoldTime) {
		//If no action is currently being performed, process this one
		if (!actionInProgress) {
			StartCoroutine (TouchEvent_c(touchPos, touchHoldTime));
		} else {
			LogAction("Warning: Action ignored, action already running.");
		};
	}

	IEnumerator TouchEvent_c(Vector2 touchPos, float touchHoldTime) {
		//Log touch event receipt
		LogAction(string.Format ("actionManager: Touch Event received:(Pos:{0},HoldTime:{1})). Checking for action to perform...",touchPos,touchHoldTime));

		//Set action in progress
		actionInProgress = true;

		//Check for proper action to perform
		// TODO
		bool isBuildingPlacement = false;
		bool isBuildingDemolition = false;
		bool isBuildingSelection = false;
		bool isUISelection = false;

		//Reset action status
		actionInProgress = false;

		//Return to TouchEvent
		yield return new WaitForEndOfFrame ();
	}

	//Writes to log for action manager if debugging enabled
	private void LogAction(string logText = " ") {
		if (!enableDebugLogs) { return; };
		print("ActionManager: " + logText);
	}

}
