using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class materialController : MonoBehaviour {

    public Material defaultMat;
    public bool gridVisible = false;
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {
        if (gridVisible)
        {
            defaultMat.renderQueue = 3000;
        }
        else
        {
            defaultMat.renderQueue = 300;
        }
	}
}
