using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class placementManager : MonoBehaviour {

    gridLayout gridArray;
    buildingSelection buildingSelect;
    touchManager checkTouch;
    GameObject currentParent;

    GameObject squareTopRight;
    GameObject squareTop;
    GameObject squareRight;

    GameObject childRight;
    GameObject childTop;
    GameObject childTopRight;

    squareStatus mainStatus;
    squareStatus rightStatus;
    squareStatus topStatus;
    squareStatus topRightStatus;

    bool placing = false;

    public GameObject farm;
	private GameObject actManObj;

    void Start () {
        checkTouch = GameObject.FindGameObjectWithTag("gameController").GetComponent<touchManager>();
        buildingSelect = GameObject.FindGameObjectWithTag("gameController").GetComponent<buildingSelection>();
        gridArray = GetComponentInParent<gridLayout>();


		//Add action test
		if (actManObj == null) { actManObj = GameObject.Find("gameController"); };
		actionManager actMan = actManObj.GetComponent<actionManager>();
		Debug.Log(this.gameObject); Debug.Log(actionManager.inputType.tap);
		int listId = actMan.AddAction (this.gameObject, actionManager.inputType.tap, true, OnTapped);
	}

	void Update () {
        currentParent = transform.parent.gameObject;

    }

    void OnTapped() {
    	print("OnTapped: Hello");
    }

    void OnMouseOver()
    {
        getFourByFourSquares();
        highlightSquares();
        //Debug.Log(mainStatus.name);
        if (Input.GetMouseButtonDown(0)||checkTouch.touchDown)
        { 
            if (!mainStatus.isOccupied && !rightStatus.isOccupied && !topStatus.isOccupied && !topRightStatus.isOccupied&&buildingSelect.currentBuilding!=null)//places building and stores data in bottom left square
            {
                setHighlightedToOccupied(buildingSelect.currentBuilding);
                setChildren();
                squareRight.transform.parent = this.transform;
                squareTop.transform.parent = this.transform;
                squareTopRight.transform.parent = this.transform;
            }
            if (mainStatus.isOccupied && rightStatus.isOccupied && topStatus.isOccupied && topRightStatus.isOccupied && buildingSelect.currentBuilding == null)//removes building and resets all squares to default status
            {
                mainStatus.resetSquare();
                topStatus.resetSquare();
                rightStatus.resetSquare();
                topRightStatus.resetSquare();
                squareRight.transform.parent = this.transform.parent;
                squareTop.transform.parent = this.transform.parent;
                squareTopRight.transform.parent = this.transform.parent;
            }
            try
            {
                if (currentParent.GetComponent<squareStatus>().holdsBuilding && buildingSelect.currentBuilding == null) //this is to delete building and reset all squares, currently throwing NULL due to
                {                                                                                                       //overlap with previous button check, once buttons are added this will be nullified.
                    currentParent.GetComponent<placementManager>().resetChildren();
                }
            } catch (Exception e)
            {

            }
        }

    }
    bool checkCanPlace()
    {
        if (!mainStatus.isOccupied && !rightStatus.isOccupied && !topStatus.isOccupied && !topRightStatus.isOccupied && buildingSelect.currentBuilding != null)
        {
            return true;
        }
        else
            return false;
    }
    void OnMouseExit()
    {
        unHighlightSquares();
    }

    void confirmPlace()
    {

    }

    void setChildren()
    {
        childRight = squareRight;
        childTop = squareTop;
        childTopRight = squareTopRight;
    }

    public void resetChildren()
    {
        squareStatus mainStatus = GetComponentInParent<squareStatus>();
        squareStatus topStatus = childTop.GetComponent<squareStatus>();
        squareStatus rightStatus = childRight.GetComponent<squareStatus>();
        squareStatus topRightStatus = childTopRight.GetComponent<squareStatus>();
        mainStatus.resetSquare();
        topStatus.resetSquare();
        rightStatus.resetSquare();
        topRightStatus.resetSquare();
        squareRight.transform.parent = this.transform.parent;
        squareTop.transform.parent = this.transform.parent;
        squareTopRight.transform.parent = this.transform.parent;
    }

    void highlightSquares()
    {
        try
        {
            mainStatus.setToHighlighted();
            rightStatus.setToHighlighted();
            topStatus.setToHighlighted();
            topRightStatus.setToHighlighted();
        }
        catch(Exception e)
        {

        }
    }

    void unHighlightSquares()
    {
        try
        {
            mainStatus.setToNotHighlighted();
            rightStatus.setToNotHighlighted();
            topStatus.setToNotHighlighted();
            topRightStatus.setToNotHighlighted();
        }
        catch(Exception e)
        {

        }
    }

    void getFourByFourSquares()
    {
        try
        {
            for (int i = 0; i < gridArray.gridX; i++)//search through array to find current block then get right, top and top right block to fill 4x4 placement space.
            {
                for (int n = 0; n < gridArray.gridY; n++)
                {
                    if (gameObject.name == "grid:" + i + "," + n)
                    {
                        int posX = i + 1;
                        int posY = n + 1;

                        squareTop = GameObject.Find("grid:" + i + "," + posY);
                        squareRight = GameObject.Find("grid:" + posX + "," + n);
                        squareTopRight = GameObject.Find("grid:" + posX + "," + posY);

                        mainStatus = GetComponent<squareStatus>();
                        topStatus = squareTop.GetComponent<squareStatus>();
                        rightStatus = squareRight.GetComponent<squareStatus>();
                        topRightStatus = squareTopRight.GetComponent<squareStatus>();
                    }
                }
            }
        }
        catch(Exception e)
        {

        }
    }

    void setHighlightedToOccupied(GameObject selectedBuilding)//sets all highlighted squares to occupied and store building information in bottom left square
    {
        mainStatus.setToOccupied();
        mainStatus.setToHoldingBuilding(selectedBuilding);
        rightStatus.setToOccupied();
        topStatus.setToOccupied();
        topRightStatus.setToOccupied();
    }
}