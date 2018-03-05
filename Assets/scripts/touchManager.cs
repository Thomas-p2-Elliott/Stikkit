using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class touchManager : MonoBehaviour {

    public bool touchDown,isTouching,touchReleased=false;
    Touch info;
    Action<Touch> touchUp;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    
        if(Input.touchCount>0) {
            checkTouchRelease();
        } else {
            touchDown = false;
        };
    }

    public static bool checkTouchRelease()
    {
        bool touchReleased = false;

        for (int i = 0; i < Input.touches.Length; i++) {
            touchReleased = Input.touches[i].phase == TouchPhase.Ended;
            print(touchReleased);
            if (touchReleased) {
                break;
            };
        };

        return touchReleased;
    }
}
