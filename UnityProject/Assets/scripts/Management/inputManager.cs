using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inputManager : MonoBehaviour {
	/* ChangeLog
	 * 
	 * 	      Date : StudentID : Notes
	 * ---------------------------------------------------------------------
	 *	05/03/2018 : 21614843  : Created to handle all inputs on all screens
	 *
	 * Notes
	 *  No multi-touch features are used, so the script is written for single-touch only
	 * 	'Touch' is synonymous with 'click' when running the PC build
	 *
	*/

	//Enable mouse for pc-build only, makes mouse click act as touch on screen, hold to hold finger down
	//Disable for IOS build!
	public bool enableMouse = true;

	//Action manager: We pass touch events to this, so that it can work out the action that should be performed
	private actionManager actionMan;

	//Main Run-Time Variables for Script
	// These are private as they are handed to the actionManager ONLY for processing to keep things neat.
	private enum inputType {tap, pressHold, swipeLeft, swipeRight, swipeUp, swipeDown};
	private bool touchStarted, touchEnded, mouseButtonDown, mouseButtonHeld, mouseButtonReleased;
	private Vector2 touchStartPos, touchEndPos;
	private float touchHoldTime;

	// Use this for initialization
	void Start () {
		//Get action manager
		actionMan = this.GetComponent<actionManager>();

		//Set up vars similar to TouchPhase for mouse use
		mouseButtonDown = mouseButtonHeld = false;
		mouseButtonReleased = true;

		//Set Values for Touch
		touchStartPos = touchEndPos = new Vector2(0,0);
		touchStarted = false; touchEnded = true;
		touchHoldTime = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {

		//Check for touch (if supported or mouse disabled)
		if (Input.touchSupported || !enableMouse) {
			CheckTouch();
		};

		//Check for clicks (if enabled or touch disabled)
		if (enableMouse || !Input.touchSupported) {
			CheckMouse();
		};
	}

	//Touch / Click Start
	private void TouchStart() {
		//print("InputManager: touchStart");

		//Set touch start & end values
		touchStarted = true;
		touchEnded = false;
		//Set both pos & hold time values
		touchHoldTime = 0.0f;
		if (enableMouse) { touchStartPos = touchEndPos = Input.mousePosition; } else {	touchStartPos = touchEndPos = Input.touches[0].position; };
	}

	//Touch / Click Hold
	private void TouchHeld() {
		//print("InputManager: touchHeld");

		//Add to hold time
		touchHoldTime += Time.deltaTime;
	}

	//Touch / Click End
	private void TouchEnd() {
		//print("InputManager: touchEnd");

		//Set touch start & end values
		touchStarted = false;
		touchEnded = true;
		//Add to hold time
		touchHoldTime += Time.deltaTime;
		//Update end pos only
		if (enableMouse) { touchEndPos = Input.mousePosition; } else {	touchEndPos = Input.touches[0].position; };
		//Get Touch Type (Tap, Swipe(Dir), HeldPress)
		//	0 = Tap, 1 = Held Press, 2 = LeftSwipe, 3 = Right Swipe, 4 = UpSwipe, 5 = DownSwipe
		int touchType = GetTouchType (touchStartPos, touchEndPos, touchHoldTime);
		//Pass event info to action manager
		actionMan.TouchEvent (touchStartPos, touchEndPos, touchType);
	}

	//		Gets Touch Type (Tap, Swipe(Dir), HeldPress)
	//	0 = Tap, 1 = Held Press, 2 = LeftSwipe, 3 = Right Swipe, 4 = UpSwipe, 5 = DownSwipe
	private int GetTouchType(Vector2 startPos, Vector2 endPos, float holdTime) {

		//Variables for function
		inputType resultingType = inputType.tap;	//Value to return to caller
		float minSwipeDist 		= 30f; 				//Minimum distance in pixelCoords for a swipe to be counted
		float minSwipeTime 		= 0.1f;  			//Minimum time a touch must be held for a swipe to count
		float minHoldTime 		= 0.75f;  			//Minimum time (s) a touch must be held for to be considered a held press

		//Check for tap
		if (holdTime < minSwipeTime) {
			resultingType = inputType.tap;
			return (int) resultingType;
		};

		//Check for held press
		if (holdTime >= minHoldTime) {
			resultingType = inputType.pressHold;
			return (int) resultingType;
		};

		//	Check for swipe
		if (holdTime >= minSwipeTime && holdTime < minHoldTime) {

			// Check for Horizontal Swipe Distance
			bool xDirRight = false; 	//Bool for L/R
			float xDiff = 0.0f; 	  	//Swipe Distance
			if (startPos.x >= endPos.x) {
				xDirRight = false;
				xDiff = startPos.x - endPos.x;
			} else {
				xDirRight = true;
				xDiff = endPos.x - startPos.x;
			};

			// Check for Vertical Swipe Distance
			bool yDirUp = false; 		//Bool for Up/Down
			float yDiff = 0.0f; 	  	//Swipe Distance
			if (startPos.y >= endPos.y) {
				yDirUp = false;
				yDiff = startPos.y - endPos.y;
			} else {
				yDirUp = true;
				yDiff = endPos.y - startPos.y;
			};

			//	Check if either were big enough to be considered a swipe
			// Check left/right first, then up/down, a swipe cannot be both, so if & else if
			if (xDiff >= minSwipeDist) {
				//Set swipe left/right values
				if (xDirRight) { resultingType = inputType.swipeRight; } else { resultingType = inputType.swipeLeft; };
			} else if (yDiff >= minSwipeDist) {
				//Set swipe uo/down values
				if (yDirUp) { resultingType = inputType.swipeUp; } else { resultingType = inputType.swipeDown; };
			} else {
				//Is not a swipe, or a touchHoldPress, just a tap, so set type
				resultingType = inputType.tap;
			};
		};

		//Return the type
		return (int) resultingType;
	}

	//Check for touch input
	private void CheckTouch() {
		//Get current touches
		Touch[] currentTouches = Input.touches;
		//If array has values (active touches)
		if (currentTouches.Length > 0) {
			//Get first active touch (multi-touch not supported in this game)
			Touch	currentTouch = currentTouches [0];

			//Check touch state (started / holding (moving || stationary) / finished)

			//Touch Start
			if (currentTouch.phase == TouchPhase.Began) {
				TouchStart ();
				return;
			};

			//Touch Held (Moving OR stationary hold)
			if (currentTouch.phase == TouchPhase.Moved || currentTouch.phase == TouchPhase.Stationary) {
				TouchHeld ();
				return;
			};

			//Touch Released
			if (currentTouch.phase == TouchPhase.Ended) {
				TouchEnd ();
				return;
			};

			//Reaching here is an error, code should of returned already.
			//Unknown Touch Phase: Reset values
			currentTouch.phase = TouchPhase.Canceled;
			touchStarted = false;
			touchEnded = true;
			touchStartPos = touchEndPos = new Vector2(0,0);
			touchHoldTime = 0.0f;
		};
	}

	//Check for mouse input
	private void CheckMouse() {
		//Get click status from mouse
		mouseButtonDown = Input.GetMouseButtonDown(0);
		mouseButtonHeld = Input.GetMouseButton(0);

		//Check click/touch state

		//MouseBtn/Finger pressed down (click/touch started)
		if (mouseButtonDown) {
			TouchStart();
			return;
		};

		//MouseBtn/Finger held down (click/touch held)
		if (touchStarted && mouseButtonHeld) {
			TouchHeld ();
			return;
		};

		//MouseBtn/Finger released (click/touch finished)
		if (touchStarted && !mouseButtonDown) {
			TouchEnd ();
			return;
		};
	}
}
