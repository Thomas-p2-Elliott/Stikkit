using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm01 : MonoBehaviour {
	//Private vars
	private actionManager 		actMan;
	private toolbarSlot 		slot;
	private buildingSelection 	buildingSelect;

	//Public vars for use in editor
	public  GameObject		selectedBuildingPrefab;
	public  Sprite			notSelectedSprite;
	public 	Sprite			selectedSprite;	

	// Use this for initialization
	void Start () {
		//Get game controller
		GameObject gameCtrl = GameObject.FindGameObjectWithTag("gameController");

		//Get Action Manager
		actMan = gameCtrl.GetComponent<actionManager>();

		//Get building selection
		buildingSelect = gameCtrl.GetComponent<buildingSelection>();

		//Get slot
		slot = this.gameObject.GetComponent<toolbarSlot>();

		//Add Action to execute select function on tap, use the slot image game object for this
		slot.actionID = actMan.AddAction (slot.gameObject, actionManager.inputType.tap, true, SelectFarm01);
	}

	//Keep Sprite Updated
	void FixedUpdate() {
		SetSprite ();
	}

	//Set sprite to use via selection status
	void SetSprite() {
		if (buildingSelect.currentBuilding == selectedBuildingPrefab) {
			slot.SetSprite (selectedSprite);
		} else {
			slot.SetSprite (notSelectedSprite);
		};
	}

	void SelectFarm01() {
		if (buildingSelect.currentBuilding == selectedBuildingPrefab) {
			//Building already selected, do nothing?
		} else {
			//Switching to Building from other selection
			buildingSelect.currentBuilding = selectedBuildingPrefab;
		};
	}
}
