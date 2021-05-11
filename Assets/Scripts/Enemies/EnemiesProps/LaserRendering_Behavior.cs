using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRendering_Behavior : MonoBehaviour
{   
    [HideInInspector]
    public LineRenderer _LR;
    private Material _mat;

    void Start()
    {
        _LR = this.GetComponent<LineRenderer>();
        _mat = _LR.material;
    }

    virtual public void Update(){
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

    public IEnumerator StartShootChrono(float _time){
        float _elapsedTime = 0f;
        float LerpedValue;

        while(_elapsedTime <= _time){
            _elapsedTime += Time.deltaTime;
            LerpedValue = _elapsedTime / _time;
            _mat.SetFloat("_Activness", LerpedValue);

            yield return null;
        }

        yield return null;
    }

    public void StopShootingLaserProcedure(){
        StopCoroutine("StartShootChrono");
    }
}
