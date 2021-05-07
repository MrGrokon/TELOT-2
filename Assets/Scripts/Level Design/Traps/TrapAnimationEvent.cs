using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapAnimationEvent : MonoBehaviour
{
    TrapBehavior_v2 _trapBehavior;

    void Start()
    {
        _trapBehavior = this.GetComponentInChildren<TrapBehavior_v2>();
    }

    public void StartFiring(){
        _trapBehavior.StartShooting();
    }

    public void start_PS(){
        _trapBehavior.Start_PS();
    }

    public void stop_PS(){
        _trapBehavior.Stop_PS();

    }
}
