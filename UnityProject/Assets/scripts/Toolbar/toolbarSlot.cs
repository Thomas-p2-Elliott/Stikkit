using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NUnit.Framework.Constraints;
using NUnit.Framework.Interfaces;

public class toolbarSlot : MonoBehaviour {

	//Inspector/Pub vars
	private SpriteRenderer		slotRenderer;
	private Sprite 			slotSprite;
	public 	int 			actionID;

	// Use this for initialization
	void Start () {

		//Set Public-Vars to Existing Values
		slotRenderer 	= this.gameObject.GetComponentInChildren<SpriteRenderer>();
		slotSprite 	= slotRenderer.sprite;
	}

	// Overwrite Sprite with a new one
	public bool SetSprite (Sprite newSprite) {
		//Return fail if null
		if (newSprite == null) { return false; }; 

		//Catch errors while overwriting value & updating renderer with new sprite
		try { slotSprite = newSprite; slotRenderer.sprite = slotSprite; } catch { return false; };
		return true; //Return success
	}
}