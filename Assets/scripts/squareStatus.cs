using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class squareStatus : MonoBehaviour {

    public bool isOccupied;
    public bool isHighlighted;
    public bool holdsBuilding;
    bool placed;

    public Material defaultMat;
    public Material occupiedMat;
    public Material highlightMat;
    public Material occupiedHighlightMat;

    GameObject buildingController;
    GameObject currentBuilding;
    GameObject heldBuilding;
    Renderer rend;

	void Start () {
        isHighlighted = false;
        isOccupied = false;
        holdsBuilding = false;
        rend = GetComponent<Renderer>();
        buildingController = GameObject.FindGameObjectWithTag("gameController");
	}
	
	void Update () {

		if(isOccupied&&!isHighlighted)
        {
            rend.material = occupiedMat;
        }
        if(isHighlighted&&!isOccupied)
        {
            rend.material = highlightMat;
        }
        if(isOccupied&&isHighlighted)
        {
            rend.material = occupiedHighlightMat;
        }
        if(!isHighlighted&&!isOccupied)
        {
            rend.material = defaultMat;
        }
    }

    public void setToOccupied()
    {
        isOccupied = true;
    }

    public void setToNotOccupied()
    {
        isOccupied = false;
    }

    public void setToHighlighted()
    {
        isHighlighted = true;
    }

    public void setToNotHighlighted()
    {
        isHighlighted = false;
    }

    public void setToHoldingBuilding(GameObject building)
    {
        currentBuilding = building;
        holdsBuilding = true;
        currentBuilding = Instantiate(currentBuilding);
        currentBuilding.transform.parent = buildingController.transform;
        currentBuilding.name = currentBuilding.name + this.transform.name;
        currentBuilding.transform.position = this.transform.position+new Vector3(0,0,-1);
    }

    public void setToNotHoldingBuilding()
    {
        currentBuilding = null;
        holdsBuilding = false;
    }
    public void resetSquare()
    {
        holdsBuilding = false;
        isOccupied = false;
        if(currentBuilding!=null)
        {
            buildingStats stopGoldGen = currentBuilding.gameObject.GetComponent<buildingStats>();
            stopGoldGen.isActive = false;
        }
        currentBuilding = null;
    }
}