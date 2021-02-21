using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPhysic : MonoBehaviour
{
    public bool UseBetterJump = true;
    public float GravityForce = 9.81f;
    public float Drag = 0f;
    public float PressedJumpedGravityMultiplier = 0.8f;
    private float BaseGravity;

    [HideInInspector]
    public Vector3 _velocity;

    private TriggerManager _States;
    private CharacterController _Controller;

    private void Awake() {
        BaseGravity = GravityForce;
        _States = this.GetComponent<TriggerManager>();
        if(_States == null){
            Debug.Log("Critical Error: Trigger Manager undefined");
        }

        _Controller = this.GetComponent<CharacterController>();
        if(_Controller == null){
            Debug.Log("Critical Error: CharacterControler undefined");
        }
    }

    private void Update() {
        //Calculate Gravity Each Frames
        _velocity.y += -GravityForce * Time.deltaTime;
        //Apply drag modifier to velocity value;
        _velocity /= 1 + Drag * Time.deltaTime;

        if(UseBetterJump){
            if(Input.GetButton("Jump")){
                _velocity = _velocity * PressedJumpedGravityMultiplier;
            }
            if(Input.GetButtonUp("Jump")){
                _velocity = _velocity / PressedJumpedGravityMultiplier;
            }
        }

        if(_States.OnGround == true &&  _velocity.y < 0){
            _velocity.y = 0f;
            //Debug.Log("Fall stopped");
        }

        //Apply Gravity Scaled each Frames
        _Controller.Move(_velocity * Time.deltaTime);
    }

    public void StopVelocity(){
        _velocity = Vector3.zero;
    }

    public float GetBaseGravity(){
        return BaseGravity;
    }
}
