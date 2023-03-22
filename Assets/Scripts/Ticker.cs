using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ticker
{
    private float _timerStart = 0f;
    private float _timeThreshold = 1f;

    public Ticker(float timerStart, float timeThreshold)
    {
        _timerStart = timerStart;
        _timeThreshold = timeThreshold;
    }

    public Ticker()
    {
        _timerStart = 0f;
        _timeThreshold = 1f;
    }

    public bool TickPassed()
    {
        var currentTime = Time.time;
        if(currentTime - _timerStart >= _timeThreshold)
        {
            _timerStart = currentTime;
            return true;
        }
        return false;
    }
/*     void Start()
    {
        
    } */
/* 
    // Update is called once per frame
    void Update()
    {
        
    } */
}
