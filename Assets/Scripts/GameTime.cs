using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameTime : MonoBehaviour
{
    private bool chronoStarted = false;

    private float seconds;

    private int minutes;

    public TMPro.TextMeshProUGUI chronoText;

    private void Awake()
    {
        chronoText = GameObject.Find("ChronoText").GetComponent<TextMeshProUGUI>();
        chronoStarted = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (chronoStarted)
        {
            if(seconds < 60)
                seconds += 1 * Time.deltaTime;
            else
            {
                seconds = 0;
                minutes++;
            }

            float rounded = Mathf.Round(seconds);
            chronoText.text = minutes + " : " + rounded;
        }
    }

    public void StartChrono()
    {
        seconds = 0;
        minutes = 0;
        chronoStarted = true;
    }

    public void StopChrono()
    {
        
    }
}
