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
    }

    private void Update() {
        #region Mouvement
        //get values from input systeme
        float Forward_Value = Input.GetAxis("Vertical");
        float Horizontal_Value = Input.GetAxis("Horizontal");

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
        float _elapsedTime = 0f;
        //what if i dash and gravity is already reduced ?
        _Physic.GravityForce = 0f;

        while( _elapsedTime < DashTime){

            //Imprive _Physic.drag over elapsetime/DashTIme
            _elapsedTime += Time.deltaTime;
            _controller.Move(Motion.normalized * DashForce * Time.deltaTime);
            yield return null;
        }

        _Physic.GravityForce = 9.81f;
        _CanMove = true;
        yield return null;
    }
    #endregion
}
