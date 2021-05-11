using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperLaserRendering_Behavior : LaserRendering_Behavior
{

    Transform Target;
    bool isAiming;

    public override void Update()
    {
        //base.Update();
        if(isAiming){
            RaycastHit _hit;
            int _layer =~ LayerMask.GetMask("EnemieProjectile");
            Physics.Raycast(this.transform.position, Target.position - this.transform.position, out _hit, Mathf.Infinity, _layer);

            _LR.SetPosition(0, this.transform.position);
            _LR.SetPosition(1, _hit.point);
        }
        else{
            _LR.SetPosition(0, this.transform.position);
            _LR.SetPosition(1, this.transform.position);
        }
    }

    public void AimAt(Transform _t){
        isAiming = true;
        Target = _t;
    }

    public void StopAiming(){
        isAiming = false;
        StopShootingLaserProcedure();
    }
}
