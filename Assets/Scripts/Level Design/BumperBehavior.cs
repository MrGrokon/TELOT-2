using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumperBehavior : MonoBehaviour
{
    public float BumpForce = 100f;

    private void OnTriggerEnter(Collider _col) {
        if(_col.gameObject.tag == "Player"){
            _col.gameObject.GetComponentInChildren<Rigidbody>().AddForce(Vector3.up * BumpForce, ForceMode.Impulse);
            FMODUnity.RuntimeManager.PlayOneShot("event:/Envrionnement/BumperJump");
        }
    }
}
