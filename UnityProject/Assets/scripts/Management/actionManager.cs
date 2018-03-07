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
	//Mirror of the values used in inputManager
	//	0 = Tap, 1 = Held Press, 2 = LeftSwipe, 3 = Right Swipe, 4 = UpSwipe, 5 = DownSwipe
	private enum inputType {tap, pressHold, swipeLeft, swipeRight, swipeUp, swipeDown};

	// Use this for initialization
	void Start () {
		LogAction("Started with debug logging enabled."); //LogAction only runs with debugging enabled
	}

	// Update is called once per frame
	void Update () {

	}

	//Event called by inputManager, gives input with startPos and endPos in pixel coords, and the type of input given
	//	0 = Tap, 1 = Held Press, 2 = LeftSwipe, 3 = Right Swipe, 4 = UpSwipe, 5 = DownSwipe
	public void TouchEvent (Vector2 touchStartPos, Vector2 touchEndPos, int touchType) {

		//If no action is currently being performed, process this one
		if (!actionInProgress) {
			StartCoroutine (TouchEvent_c(touchStartPos, touchEndPos, touchType));
		} else {
			LogAction("Warning: Action ignored, action already running.");
		};
	}

	IEnumerator TouchEvent_c(Vector2 touchStartPos, Vector2 touchEndPos, int touchType) {
		//Log touch event receipt
		LogAction(string.Format ("actionManager: Touch Event received:(StartPos:{0},EndPos:{1},TypeVal:{2})). Checking for action to perform...",touchStartPos,touchEndPos,(inputType)touchType));

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
