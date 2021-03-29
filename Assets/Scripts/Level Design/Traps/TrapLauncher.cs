using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapLauncher : MonoBehaviour
{
    TrapBehavior _parentBehavior;
    private Animator Trigger_Anim;
    public bool DetectMonster = false;

    private void Awake() {
        _parentBehavior = this.transform.parent.GetComponent<TrapBehavior>();
        Trigger_Anim = this.transform.parent.GetChild(2).GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other) {
        if(DetectMonster == true){
            if(other.tag == "Player"){
                //do the shoot
                Trigger_Anim.SetTrigger("Triggered");
                _parentBehavior.StartCoroutine(_parentBehavior.TrapShooting_Procedure());
            }
        }
        else{
            //do the shoot
            _parentBehavior.StartCoroutine(_parentBehavior.TrapShooting_Procedure());
            Trigger_Anim.SetTrigger("Triggered");
        }
    }
}
