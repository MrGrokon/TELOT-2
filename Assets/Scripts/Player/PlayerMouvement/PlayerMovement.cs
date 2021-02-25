using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Mouvement Variables")]
    [Range(1f, 50f)]
    public float PlayerSpeed = 10f;
    private bool _CanMove = true;

    [Header("Dash Variables")]
    public float DashForce = 60f;
    [Range(0.1f, 3f)]
    public float DashTime = 0.5f;

    private CharacterController _controller;
    private TriggerManager _States;
    private CustomPhysic _Physic;
    private WallRunning _wallManager;

    private bool isDashing = false;
    
    private float timeToLerp = 800;
    private float elapsedTimeToLerp;
    float Forward_Value = 0;
    float Horizontal_Value = 0;

    private Vector3 _Motion;

    private void Awake() {
        _controller = this.GetComponent<CharacterController>();
        if(_controller == null){
            Debug.Log("Critical Error: CharacterController undefined");
        }
        _States = this.GetComponent<TriggerManager>();
        if(_States == null){
            Debug.Log("Critical Error: TriggerManager undefined");
        }
        _Physic = this.GetComponent<CustomPhysic>();
        if(_Physic == null){
            Debug.Log("Critical Error: CustomPhysic undefined");
        }

        _wallManager = GetComponent<WallRunning>();
    }

    private void Update() {
        #region Mouvement
        //get values from input systeme
        if (_States.OnGround ||_wallManager.WallOnLeft ||_wallManager.WallOnRight)
        {
            Forward_Value = Input.GetAxis("Vertical");
            Horizontal_Value = Input.GetAxis("Horizontal");
        }
        else
        {
            if (isDashing == false)
            {
                if(Forward_Value > 0f)
                    Forward_Value -= Input.GetAxis("Vertical") * Time.deltaTime;
                if(Horizontal_Value > 0f)
                    Horizontal_Value -= Input.GetAxis("Horizontal") * Time.deltaTime; 
            }
            
        }
        
        

        Vector3 _Motion = (this.transform.right * Horizontal_Value + this.transform.forward * Forward_Value);
        //Vector3 _Motion = (this.transform.right * Horizontal_Value + this.transform.forward * Forward_Value).normalized;
        
        if(_CanMove == true){
            _controller.Move(_Motion * PlayerSpeed * Time.deltaTime);
        }
        #endregion
    
        #region Dash
        if(Input.GetButtonDown("Dash") ){
            Debug.Log("Dash");
            if(_Motion != Vector3.zero){
                StartCoroutine(MotionLocking_Dash(_Motion));
                StartCoroutine(FeedbackManager.Instance.ChangeCameraFOVDuringDash(DashTime, _Motion));
            }
            else{
                StartCoroutine(MotionLocking_Dash(this.transform.forward));
                StartCoroutine(FeedbackManager.Instance.ChangeCameraFOVDuringDash(DashTime, this.transform.forward));

            }
        }
        #endregion
    }

    #region Coroutines
    IEnumerator MotionLocking_Dash(Vector3 Motion, bool UseGravity = false){
        _CanMove = false;
        isDashing = true;
        float _elapsedTime = 0f;
        //what if i dash and gravity is already reduced ?
        _Physic.GravityForce = 0f;

        while( _elapsedTime < DashTime){

            //Imprive _Physic.drag over elapsetime/DashTIme
            _elapsedTime += Time.deltaTime;
            _controller.Move(Motion.normalized * DashForce * Time.deltaTime);
            yield return null;
        }

        _Physic.GravityForce = _Physic.GetBaseGravity();
        _CanMove = true;
        isDashing = false;
        yield return null;
    }
    #endregion

    private Vector3 LerpMotionToGravity(Vector3 _Motion)
    {
        elapsedTimeToLerp++;
        return Vector3.Lerp(_Motion, transform.forward * 2 + new Vector3(0, -2, 0), elapsedTimeToLerp / timeToLerp);
    }
    
}
