using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectEnergie : MonoBehaviour
{
    public GameObject PlayerProjectile;

    [Range(0.1f, 1f)]
    public float TimeBetweenShots = 0.3f;

    private EnergieStored _Energie;

    private void Awake() {
        _Energie = this.GetComponent<EnergieStored>();
        if(_Energie == null){
            Debug.Log("EnergieStored not defined");
        }
    }

    private void Update() {
        if(Input.GetButtonDown("Fire1")){

        }
    }
}
