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
	//'actionData' struct, contains all the info needed to execute an 'action' added by 'AddAction' function
	public struct actionData {
		public GameObject 		obj;
		public inputType 		inpType;
		public bool 			enabled;
		public System.Action 	func;
		public object[] 		funcArgs;
		//Default constructor, no data validation.
		public actionData(GameObject iObj, inputType iInpType, bool iEnabled, System.Action iFunc, object[] iFuncArgs = null) {
			//Set unvalidated values
			obj = iObj; inpType = iInpType; enabled = iEnabled; func = iFunc; funcArgs = iFuncArgs;
		}
		//Validated values constructor-ish method
		public bool addActionData(GameObject iObj, inputType iInpType, bool iEnabled, System.Action iFunc, object[] iFuncArgs = null) {
			//Validate data
			if (iObj == null) { return false; };
			if (((int) iInpType) > 5 || ((int) iInpType) < 0) { return false; };
			if (iFunc == null) { return false; };

			//Set validated values
			obj = iObj; inpType = iInpType; enabled = iEnabled; func = iFunc; funcArgs = iFuncArgs;
			return true;
		}
	};
	//Array of all registered actions via their actionData
	// public array so that functions can access its values and enable/disable their actions
	[System.NonSerialized] private static actionData[] registeredActions = new actionData[4095];
	[System.NonSerialized] private static int actionCount = 0;

	//	Internal script vars
	//Used to prevent multiple events running at once
	[System.NonSerialized] private bool actionInProgress = false; 
	//Mirror of the values used in inputManager
	//	0 = Tap, 1 = Held Press, 2 = LeftSwipe, 3 = Right Swipe, 4 = UpSwipe, 5 = DownSwipe
	public enum inputType {tap, pressHold, swipeLeft, swipeRight, swipeUp, swipeDown};



	// Use this for initialization
	void Start ()	{
		LogAction ("Started with debug logging enabled."); //LogAction only runs with debugging enabled
		//registeredActions = new actionData[4095];
		//actionCount = 0;
	}
	/*
		Func: AddAction
		Desc: Registers a function callback when an action (eg swipe) is performed on an object.
		Args:
			inpObj: 	GameObject:	The object that we are registering the action for
			inpType: 	inputType:	The action type we are watching for (tap/swipe/hold)
			enabled: 	bool:		Whether or not the action is currently enabled
			outFunc:	Function:	The function to run when the action is performed
			outArgs:	Arguments:  The arguments to run with the function when an action is performed
			... outArgs can continue indefinitely, as many args as you want can be given...
		Outs:
			Int: -1 on Error, >0 is list index in registeredActions, the public list registry
		Example Use(s):

			//Add action to grid for tapping to build a building, pass in touch pos to addBuilding func, it starts enabled
			AddAction(this.gameobject, inputType.tap, true, this.addBuilding, touchEndPos)

			//Add action to building for when user presses and holds on it, it starts disabled
			AddAction(this.GameObject, inputType.pressHold, false, this.showDetails)
	*/
	public int AddAction(GameObject inpObj, inputType inpType, bool enabled, System.Action outFunc, object[] outArgs = null) {
		LogAction(string.Format ("Adding '{0}' action to '{1}' with func '{3}' and args '{4}' in '{2}' state...", inpType, inpObj.name, enabled, outFunc.Method, outArgs));

		//Create action data for action
		actionData newActionData = new actionData();
		//Return -1 (fail) if addActionData fails, as this validates the input data
		if (!(newActionData.addActionData (inpObj, inpType, enabled, outFunc, outArgs))) {
			LogAction ("Error adding actionData to newActionData var.");
			return -1;
		};

		//Register the action in our action list
		if (actionCount >= 4094) { print("AddAction: Error: Action List Full!"); };
		registeredActions[actionCount] = newActionData;
		actionCount++;

		//Check it is there
		print(string.Format("action there and enabled?: {0}", registeredActions[actionCount-1].enabled));

		//Return index in list to caller func
		return (actionCount-1);
	}

	//Event called by inputManager, gives input with startPos and endPos in pixel coords, and the type of input given
	//	0 = Tap, 1 = Held Press, 2 = LeftSwipe, 3 = Right Swipe, 4 = UpSwipe, 5 = DownSwipe
	public void TouchEvent (Vector2 touchStartPos, Vector2 touchEndPos, int touchType) {

		//If no action is currently being performed, process this one
		if (!actionInProgress) {
			//Log touch event receipt
			//LogAction(string.Format ("actionManager: Touch Event received:(StartPos:{0},EndPos:{1},TypeVal:{2})). Checking for action to perform...",touchStartPos,touchEndPos,(inputType)touchType));

			//Set action in progress
			actionInProgress = true;

			print("registered actionzzz: " + actionCount);
			print("touchStartPos: " + touchStartPos.ToString ());

			//Check through registered actions to see if we have one to perform
			for (int i = 0; i < (actionCount); i++) {
				actionData actData = registeredActions[i];
				//Skip disabled actions
				if (!(actData.enabled)) { continue; };
				//Skip when action doesnt match registered one
				if ((int) actData.inpType != touchType) { continue; };
				//	Skip if touch is not within bounds of object
				//Convert touch pos to world space
				Vector3 tpWs = new Vector3();
				Camera cam = Camera.main;
				tpWs = cam.ScreenToWorldPoint (new Vector3(touchStartPos.x, touchStartPos.y, 0));
				tpWs.z = 0;
				print("tpWs: " + tpWs.ToString () + "\tbounds: " + actData.obj.GetComponent<Renderer>().bounds.ToString ());

				if (!(actData.obj.GetComponent<Renderer>().bounds.Contains(tpWs))) { continue; };
				//We got this far, invoke the function with the args!
				actData.func.Method.Invoke(actData.obj, actData.funcArgs);
			}
		
			//Reset action in progress so that next action can occur
			actionInProgress = false;

		} else {
			LogAction("Warning: Action ignored, action already running.");
		};
	}


	//Writes to log for action manager if debugging enabled
	private void LogAction(string logText = " ") {
		if (!enableDebugLogs) { return; };
		print("ActionManager: " + logText);
	}

}
