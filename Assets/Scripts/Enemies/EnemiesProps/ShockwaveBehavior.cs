using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveBehavior : MonoBehaviour
{
    public AnimationCurve _growthCurve;

    private float TimeToGoFullSize = 0f;
    private float _elapsedTime = 0f;

    private Vector3 MaxScale;
    private Vector3 ActualScale = Vector3.zero;

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

    public void SetLifeTime(float _time){
        TimeToGoFullSize = _time;
    }

    public void SetGrowthCurve(AnimationCurve _curve){
        _growthCurve = _curve;
    }
    #endregion 
}
