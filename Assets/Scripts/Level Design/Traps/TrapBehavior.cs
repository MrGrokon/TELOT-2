using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBehavior : MonoBehaviour
{
    public enum TrapMode
    {
        Burst, Continuous
    }

    [Header("Init By Hand")]
    public EnemieProjectileBehavior Projectile;
    public Transform ShootPoint;

    [Header("General Parameters")]
    public TrapMode MyMode = TrapMode.Burst;
    public float LoadingTime = 1.5f;
    public float ProjectileSpeed = 50f;
    private Animator Trap_Anim;

    [Header("Continuous Parameters")]
    public float TimeBetweenShoots = 1f;
    private bool ShootContinuously = false;
    private float _elapsedCountinuousTime = 0f;
    
    [Header("Burst Parameters")]
    public int NumberOfRoundShout = 5;
    public float BurstTime = 1f;

    private void Awake() {
        Trap_Anim = this.transform.GetChild(1).GetComponent<Animator>();
    }

    private void Update() {
        if(MyMode == TrapMode.Continuous && ShootContinuously){
            _elapsedCountinuousTime += Time.deltaTime;
            if(_elapsedCountinuousTime >= TimeBetweenShoots){
                _elapsedCountinuousTime =0f;
                Shoot();
            }
        }
    }

    public IEnumerator TrapShooting_Procedure(){
        #region Loading time
            float _elapsedTime = 0f;
            Trap_Anim.Play("TrapLoading", 0, LoadingTime);
            while(_elapsedTime < LoadingTime){
                _elapsedTime += Time.deltaTime;
                yield return null;
            }
        _elapsedTime = 0f;
        #endregion

        #region Shooting Phase
            switch (MyMode){
            case TrapMode.Continuous:
                ShootContinuously = true;
            break;

            case TrapMode.Burst:
                for (int i = 0; i < NumberOfRoundShout; i++)
                {
                    while (_elapsedTime < (BurstTime / NumberOfRoundShout) )
                    {
                        _elapsedTime += Time.deltaTime;
                        yield return null;
                    }
                    //Debug.Log("time: " + _elapsedTime);
                    _elapsedTime = 0f;
                    Shoot();
                }
                Trap_Anim.SetTrigger("BackToIdle");
            break;

            default:
            Debug.Log("Something fucked up in trap behavior");
            break;
            }
        #endregion

        yield return null;
    }

    private void Shoot(){
        Instantiate(Projectile, ShootPoint.position, Quaternion.LookRotation(ShootPoint.forward, ShootPoint.up)).SetSpeed(ProjectileSpeed);
    }
}
