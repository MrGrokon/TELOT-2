using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Call_Coroutine : MonoBehaviour
{
    public GameObject _target;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            LaserRendering_Behavior _laser = _target.GetComponentInChildren<LaserRendering_Behavior>();
            if(_laser != null){
                StartCoroutine(_laser.StartShootChrono(5f));
            }
        }
    }
}
