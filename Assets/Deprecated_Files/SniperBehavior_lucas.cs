using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperBehavior_lucas : MonsterBehavior
{
    [Header("Général Parameters")]
    public EnemieProjectileBehavior EnemieProjectile;
    private LineRenderer _Line;

    [Header("Shoot Parameters")]
    public float KillZoneDuration = 2f;
    public float TimeToAim = 3f;
    private float _elapsedShootTime = 0f;
    

    #region Overided Functios
    
    public override void Awake()
    {
        base.Awake();
        _Line = this.GetComponent<LineRenderer>();
        _Line.startColor = Color.red;
        _Line.endColor = Color.red;
    }

    public override void Update()
    {
        base.Update();
        Vector3 _dirToPlayer = ObjectReferencer.Instance.Avatar_Object.transform.position - this.transform.position;
        if(Physics.Raycast(this.transform.position, _dirToPlayer, out RaycastHit _hit, Mathf.Infinity) && _hit.collider.tag == "Player"){
            //Debug.Log("Clear line of sight of Player");
            Vector3[] posArray = new Vector3[2];
            posArray[0] = this.transform.position;
            posArray[1] = _hit.point;

            _Line.SetPositions( posArray);
        }
        else{
            ResetShootTimer();
        }
    }

    #endregion

    private void Shoot(){
        ResetShootTimer();
        Instantiate(EnemieProjectile, this.transform.position, this.transform.rotation).SetSpeed(50f);
    }

    private void ResetShootTimer(){
        _elapsedShootTime = 0f;
    }
}
