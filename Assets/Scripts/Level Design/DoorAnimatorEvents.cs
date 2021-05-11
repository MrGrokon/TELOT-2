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
        /*Sparks.Add(this.transform.GetChild(3).GetComponent<ParticleSystem>());
        Sparks.Add(this.transform.GetChild(4).GetComponent<ParticleSystem>());*/
    }

    public void Call_Sparks(){
        /*foreach(var ps in Sparks){
            ps.Play();
        }*/
    }

    public void Stop_Sparks(){
        /*foreach(var ps in Sparks){
            ps.Stop();
        }*/
    }

    public void Call_Smoke(){
        ToonSmoke.Play();
    }


}
