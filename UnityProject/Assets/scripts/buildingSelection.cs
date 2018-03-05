using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buildingSelection : MonoBehaviour {

    public GameObject farm,barn;
    public GameObject currentBuilding;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown("1"))
        {
            currentBuilding = farm;
        }
        if(Input.GetKeyDown("2"))
        {
            currentBuilding = barn;
        }
        if (Input.GetKeyDown("3")) 
        {
            currentBuilding = null;
        }

	}
}
