using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class buildingStats : MonoBehaviour {

    public float goldPerHour=50;
    public float totalGoldEarned=0;
    public int timeAlive;
    public bool isActive = false;
    public bool passedCorrectGoldGeneration;
    public float lastGoldCheck;
    bool sendingGold;
    totalGold totalGold;
        
    //EACH IN GAME DAY(24 HOURS) IS REPRESENTED BY 30 MINUTES/1800 seconds(REAL TIME)
    //75 SECONDS IS 1 HOUR IN GAME. DEVIDE GOLD PER HOUR ON GOLD GENERATION BY THIS TO PASS HOURLY EARNINGS IN 1 SECOND INTERVALS.
	void Start () {

        totalGold = GameObject.FindGameObjectWithTag("gameController").GetComponent<totalGold>();
        lastGoldCheck = goldPerHour;
        totalGold.calculateGoldPerHour(goldPerHour);
        isActive = true;
        StartCoroutine(goldGeneration());
        StartCoroutine(checkTimeAlive());

    }
    private void Awake()
    {

    }

    void Update () {
        if(lastGoldCheck!=goldPerHour)//if any changes happen to GPH from last frame, use function to repass correct
        {
            passUpdateGPH();
        }
        if(!isActive)
        {
            destroyBuilding();
        }
	}

    void OnAwake()
    {

    }

    void passUpdateGPH()//if any changes happen pass gold - the last known gold before the change then update to curent gph
    {
        totalGold.calculateGoldPerHour(goldPerHour - lastGoldCheck);
        lastGoldCheck = goldPerHour;
    }

    IEnumerator goldGeneration()
    {
        while(isActive)
        {
            totalGoldEarned += goldPerHour/75;
            totalGold.addGold(goldPerHour/75);
            yield return new WaitForSeconds(1);
        }
    }
    IEnumerator checkTimeAlive()
    {
        while (isActive)
        {
            timeAlive += 1;
            yield return new WaitForSeconds(1);
        }
    }

    public void destroyBuilding()
    {
        goldPerHour = 0;
        Destroy(this.gameObject,.1f);
    }
    

}
