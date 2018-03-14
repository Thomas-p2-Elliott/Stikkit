using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal.Filters;
using System;
using NUnit.Framework;

public class actionManager : MonoBehaviour {

	//	Public vars
	//Enable logs to console for actionManager
	public bool enableDebugLogs = true;
	//'actionData' struct, contains all the info needed to execute an 'action' added by 'AddAction' function
	public struct actionData {
		public GameObject 		obj;
		public inputType 		inpType;
		public bool 			enabled;
		public System.Action 		func;
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

			//Check through registered actions to see if we have one to perform
			for (int i = 0; i < (actionCount); i++) {

				//Get current action
				actionData actData = registeredActions [i];

				//Skip disabled actions
				if (!(actData.enabled)) {
					continue;
				}

				//Skip when action doesnt match registered one
				if ((int)actData.inpType != touchType) {
					continue;
				}

				//Convert touch pos to world space
				Vector3 tpWs = new Vector3 ();
				Camera cam = Camera.main;
				tpWs = cam.ScreenToWorldPoint (new Vector3 (touchStartPos.x, touchStartPos.y, 0));
				tpWs.z = 0;

				//Log position of touch
				//print ("tpWs: " + tpWs.ToString ());
				//Log position of actionable obj
				//print ("objPos: " + actData.obj.transform.position);

				bool hasRenderer = (actData.obj.GetComponent<Renderer> () == null) ? false : true;
				bool hasSpriteRenderer = (actData.obj.GetComponent<SpriteRenderer> () == null) ? false : true;
				//bool hasRect = (actData.obj.GetComponent<Rect> () == null) ? false : true;   //Result of this is always false, need new method?
				bool hasRectTrans = (actData.obj.GetComponent<RectTransform> () == null) ? false : true;

				//Log touched obj properties
				//print (string.Format ("hasRenderer: {0}\thasSpriteRenderer:{1}\thasRect:{2}\thasRectTrans:{3}", hasRenderer, hasSpriteRenderer, hasRect, hasRectTrans));

				//If none of the above options then continue to check next action
				if (!hasRenderer && !hasSpriteRenderer && !hasRectTrans) {
					continue;
				}

				//If it has a regular renderer and touch is within its bounds
				if (hasRenderer) {
					if (actData.obj.GetComponent<Renderer> ().bounds.Contains (tpWs)) {
						//Log the action
						LogAction ("Renderer: " + actData.obj.name + " " + (inputType)touchType + " action started.");
						//We got this far, invoke the function with the args!
						actData.func.Method.Invoke (actData.func.Target, actData.funcArgs);
						//Continue to next action to check
						continue;
					} else {
						continue;
					}
				}

				//If it has a sprite renderer and touch is within its bounds
				if (hasSpriteRenderer) {
					if (actData.obj.GetComponent<SpriteRenderer> ().bounds.Contains (tpWs)) {
						//Log the action
						LogAction ("SpriteRenderer: " + actData.obj.name + " " + (inputType)touchType + " action started.");
						//We got this far, invoke the function with the args!
						actData.func.Method.Invoke (actData.func.Target, actData.funcArgs);
						//Continue to next action to check
						continue;
					} else {
						continue;
					}
				}

				//If it has a RectTrans and a Rect and touch is within its bounds
				if (hasRectTrans) {
			
					//Get world space position values for corners
					Vector3[] fourCornersArr = new Vector3[4];
					actData.obj.GetComponent<RectTransform> ().GetWorldCorners(fourCornersArr);
					float xMin = fourCornersArr[0].x;
					float xMax = fourCornersArr[3].x;
					float yMin = fourCornersArr[0].y;
					float yMax = fourCornersArr[1].y;
					//print ("xMin: " + xMin + "  " +  "xMax: " + xMax + "    " + "yMin: " + yMin + "  " +  "yMax: " + yMax); 
			
					//Check if within X bounds
					if ((tpWs.x >= xMin) && (tpWs.x <= xMax)) {
						//Check if within y bounds
						if ((tpWs.y >= yMin) && (tpWs.y <= yMax)) {

							//Within bounds, log action & run the function associated with it!
							LogAction("Obj: " + actData.obj.name + " " + (inputType) touchType + " action started.");
				                	actData.func.Method.Invoke(actData.func.Target, actData.funcArgs);
						}
					}

					//Continue to check next action in list
					continue;
				}
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
