﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timeControl : MonoBehaviour

{
    [System.Serializable]
    public class Clock
    {
        private System.DateTime now;
        private System.TimeSpan timeNow;
        private System.TimeSpan gameTime;
        private int minutesPerDay; //Realtime minutes per game-day (1440 would be realtime)

        public Clock(int minPerDay)
        {
            minutesPerDay = minPerDay;
        }

        public System.TimeSpan GetTime()
        {
            now = System.DateTime.Now;
            timeNow = now.TimeOfDay;
            double hours = timeNow.TotalMinutes % minutesPerDay;
            double minutes = (hours % 1) * 60;
            double seconds = (minutes % 1) * 60;
            gameTime = new System.TimeSpan((int)hours, (int)minutes, (int)seconds);

            return gameTime;
        }
    }

    Clock clock;

    void Start()
    {
        clock = new Clock(3);
        Debug.Log(System.DateTime.Now);
    }

    void FixedUpdate()
    {
        //Debug.Log(clock.GetTime().ToString());
    }
}
