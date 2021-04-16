using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveBehavior : MonoBehaviour
{
    [HideInInspector]
    public AnimationCurve _growthCurve;

    private float TimeToGoFullSize = 0f;
    private float _elapsedTime = 0f;

    private Vector3 MaxScale;
    private Vector3 ActualScale = Vector3.zero;

    private bool _damageAlreadyDone = false;
    private int DammageAmount;
    private float ImpulseAmount;

    #region Scaling Over Life Time
    private void Awake() {
        MaxScale = this.transform.localScale;
        this.transform.localScale = Vector3.zero;
    }

    private void Update() {
        if(TimeToGoFullSize != 0f){
            _elapsedTime += Time.deltaTime;
            if(_elapsedTime < TimeToGoFullSize){
                float ScaleValue = _growthCurve.Evaluate(_elapsedTime / TimeToGoFullSize) * MaxScale.x;
                ActualScale = new Vector3(ScaleValue, ScaleValue, ScaleValue);
                this.transform.localScale = ActualScale;
            }
            else{
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider _trig) {
        if(_trig.tag == "Player" && _damageAlreadyDone == false){
            Debug.Log("Player in shockwave");
            _damageAlreadyDone = true;
            _trig.GetComponent<PlayerLife>().TakeDammage(DammageAmount);
            Vector3 ExplosionDir = (_trig.transform.position - this.transform.position).normalized + Vector3.up;
            _trig.GetComponent<Rigidbody>().AddForce(ExplosionDir.normalized * ImpulseAmount, ForceMode.Impulse);
        }
    }

    public void SetLifeTime(float _time){
        TimeToGoFullSize = _time;
    }

    public void SetGrowthCurve(AnimationCurve _curve){
        _growthCurve = _curve;
    }

    public void SetDammage(int _amount){
        DammageAmount = _amount;
    }

    public void SetImpulseForce(float _amount){
        ImpulseAmount = _amount;
    }
    #endregion 
}
