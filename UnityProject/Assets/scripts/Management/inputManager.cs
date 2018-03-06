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
	private bool touchStarted, touchEnded, mouseButtonDown, mouseButtonHeld, mouseButtonReleased;
	private Vector2 touchPos;
	private float touchHoldTime;

	// Use this for initialization
	void Start () {
		//Get action manager
		actionMan = this.GetComponent<actionManager>();

		//Set up vars similar to TouchPhase for mouse use
		mouseButtonDown = mouseButtonHeld = false;
		mouseButtonReleased = true;

		//Set Values for Touch
		touchPos.x = touchPos.y = 0;
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
		//Set pos & hold time values
		touchHoldTime = 0.0f;
		if (enableMouse) { touchPos = Input.mousePosition; } else {	touchPos = Input.touches[0].position; };
	}

	//Touch / Click Hold
	private void TouchHeld() {
		//print("InputManager: touchHeld");

		//Add to hold time & update pos
		touchHoldTime += Time.deltaTime;
		if (enableMouse) { touchPos = Input.mousePosition; } else {	touchPos = Input.touches[0].position; };
	}

	//Touch / Click End
	private void TouchEnd() {
		//print("InputManager: touchEnd");

		//Set touch start & end values
		touchStarted = false;
		touchEnded = true;
		//Add to hold time
		touchHoldTime += Time.deltaTime;
		//Update pos
		if (enableMouse) { touchPos = Input.mousePosition; } else {	touchPos = Input.touches[0].position; };
		//Pass event info to action manager
		actionMan.TouchEvent (touchPos, touchHoldTime);
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
			touchPos = currentTouch.position;
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
