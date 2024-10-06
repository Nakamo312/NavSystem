using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TimeEvent
{
    private float duration;
    private Action time;
    TimeEvent(float duration)
    {
        this.duration = duration;
    }

    public void Invoke()
    {
        this.time?.Invoke();
    }
}

public class CentralyzedTimer : MonoBehaviour
{
    public static CentralyzedTimer centralyzedTimer;
    private List<TimeEvent> timer = new List<TimeEvent>();

    public void AddWait(float delay)
    {
        //timer.Add(delay);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
