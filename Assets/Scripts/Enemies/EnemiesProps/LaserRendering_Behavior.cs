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
        int _layer =~ LayerMask.GetMask("EnemieProjectile");
        Physics.Raycast(this.transform.position, this.transform.forward, out _hit, Mathf.Infinity, _layer);

        /* Position in local space
        _LR.SetPosition(0, new Vector3());
        _LR.SetPosition(1, new Vector3(0f,0f, Vector3.Distance(this.transform.position, _hit.point)));
        */

        /* position in world space */
        _LR.SetPosition(0, this.transform.position);
        _LR.SetPosition(1, _hit.point);
    }
}
