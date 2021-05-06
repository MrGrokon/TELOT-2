using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kill_VFX : MonoBehaviour
{
    public float TimeBeforeKill = 3f;
    private float _elapsedTime = 0f;

    private void Update() {
        _elapsedTime += Time.deltaTime;
        if(_elapsedTime >= TimeBeforeKill){
            Destroy(this.gameObject);
        }
    }
}
