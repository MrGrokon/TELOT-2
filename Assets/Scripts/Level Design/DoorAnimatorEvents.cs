using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimatorEvents : MonoBehaviour
{
    ParticleSystem ToonSmoke;
    List<ParticleSystem> Sparks;

    void Start()
    {
        ToonSmoke = this.transform.GetChild(2).GetComponent<ParticleSystem>();
    }

    public void Call_Sparks(){
        foreach(var ps in Sparks){
            ps.Play();
        }
    }

    public void Call_Smoke(){
        ToonSmoke.Play();
    }


}
