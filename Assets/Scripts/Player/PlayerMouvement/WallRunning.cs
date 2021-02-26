using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Aderence Variables")]
    public AnimationCurve GravityVariationOverTime;
    [Range(1f, 10f)]
    public float TimeToStickToWall = 5f;
    private float _elapsedTime = 0f;

    private Collider WallRunnedOn;
    private bool TriggerEnterWallRun = true;
    private TriggerManager _States;
    private CustomPhysic _Physic;
    private CharacterController _Controller;

    #region Init
    private void Awake() {
        _States = this.GetComponent<TriggerManager>();
        if(_States == null){
            Debug.Log("Critical Error: TriggerManager not found");
        }
        _Physic = this.GetComponent<CustomPhysic>();
        if(_States == null){
            Debug.Log("Critical Error: CustomPhysic not found");
        }
        _Controller = this.GetComponent<CharacterController>();
        if(_Controller == null){
            Debug.Log("Critical Error: Character Controller not found");
        }
    }
    #endregion
    
    private void Update() {

        if(_States.IsOnWall){
            if(TriggerEnterWallRun == true){
                _Physic.StopVelocity();
                TriggerEnterWallRun = false;

                if(_States.WallOnLeft){
                    StartCoroutine(FeedbackManager.Instance.AngularCameraRotation(FeedbackManager.CameraDirection.Left));
                }
                if(_States.WallOnRight){
                    StartCoroutine(FeedbackManager.Instance.AngularCameraRotation(FeedbackManager.CameraDirection.Right));
                }
            }

            //Apply a modified gravity to the character
             _elapsedTime += Time.deltaTime;
            _Physic.GravityForce = GravityVariationOverTime.Evaluate(_elapsedTime/TimeToStickToWall) * _Physic.GetBaseGravity();
        }

        else{
            //Reset TrigerEnterWallRun for multiple uses
            if(TriggerEnterWallRun == false){
                StartCoroutine(FeedbackManager.Instance.ResetCameraAngle());
                TriggerEnterWallRun = true;        
                
                //Opti
                _elapsedTime = 0f;
                _Physic.GravityForce = _Physic.GetBaseGravity();
            }            
        }
    }
}