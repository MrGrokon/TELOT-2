using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRendering_Behavior : MonoBehaviour
{
    private LineRenderer _LR;

    void Start()
    {
        _LR = this.GetComponent<LineRenderer>();
    }

    private void Update(){
        RaycastHit _hit;
        Physics.Raycast(this.transform.position, this.transform.forward, out _hit, Mathf.Infinity);

        _LR.SetPosition(0, new Vector3());
        _LR.SetPosition(1, new Vector3(0f,0f, Vector3.Distance(this.transform.position, _hit.point)));
    }
}
