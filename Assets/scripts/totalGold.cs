using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class totalGold : MonoBehaviour {

    public float currentTotalGold;
    public float goldPerHour=0;
    Text goldText;
    bool isActive=false;
   

	void Start () {
        isActive = true;
        goldText = GameObject.FindGameObjectWithTag("goldDisplay").GetComponent<Text>();
        StartCoroutine(waitUpdateDisplay());
	}
	
	void Update () {
        
	}

    public void addGold(float gold)//called by buildings to add their gold generation to the main wallet
    {
        currentTotalGold += gold;
    }
    public void calculateGoldPerHour(float GPH)//called by buildings to calculate total gold generation. each building has a calculation to check for updates to GPH to recall this with any amendments
    {
        goldPerHour += GPH;
    }
    
    IEnumerator waitUpdateDisplay()// updates the gold display every second on the second to avoid updating at random times dependant on buildings
    {
        while (isActive)
        {
            yield return new WaitForSeconds(1);
            goldText.text ="Gold: " + currentTotalGold.ToString("F0")+" gph:" + goldPerHour;
        }
    }


}
