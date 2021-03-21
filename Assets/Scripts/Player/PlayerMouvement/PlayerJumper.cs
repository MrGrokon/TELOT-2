using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumper : MonoBehaviour
{
    [Header("Jump Parameter")]
    [Range(5f, 30f)]
    public float JumpForce = 20f;
    [Range(0.5f, 2f)]
    public float JumpPersistance = 2f;
    public AnimationCurve JumpForceOvertime;

    private TriggerManager _States;
    private CustomPhysic _Physic;
    private CharacterController _Controller;

    private void Awake() {
        _States = this.GetComponent<TriggerManager>();
        if(_States == null){
            Debug.Log("Critical Error: TriggerManager not found");
        }

        _Physic = this.GetComponent<CustomPhysic>();
        if(_Physic == null){
            Debug.Log("Critical Error: CustomPhysic not found");
        }

        _Controller = this.GetComponent<CharacterController>();
        if(_Controller == null){
            Debug.Log("Critical Error: CharacterControler not found");
        }
    }

    private void Update() {
        #region Jump
        if(Input.GetButtonDown("Jump")){
            if(_States.OnGround == true){
                //_Physic._velocity.y += Mathf.Sqrt(JumpHeight * -2f * -_Physic.GravityForce);
                StartCoroutine(Persitant_Jump());
                Debug.Log("Jump from ground");
            }
            else if(_States.IsOnWall == true && _States.LastWall_Normal != Vector3.zero){
                Debug.Log("Jump From a wall");
                StartCoroutine(Persitant_Jump((_States.LastWall_Normal + Vector3.up).normalized));
            }
        }
        #endregion
        
    }

    #region Coroutines
    IEnumerator Persitant_Jump(Vector3 JumpDirection){
        float _elapsedTime = 0f;
        float _actualForce = JumpForce;

        while(_elapsedTime < JumpPersistance){
            _elapsedTime += Time.deltaTime;
            _actualForce = JumpForceOvertime.Evaluate(_elapsedTime/JumpPersistance) * JumpForce;
            _Controller.Move( JumpDirection * _actualForce * Time.deltaTime);
            yield return null;
        }
        yield return null;
    }

    IEnumerator Persitant_Jump(){
        float _elapsedTime = 0f;
        float _actualForce = JumpForce;

        while(_elapsedTime < JumpPersistance){
            _elapsedTime += Time.deltaTime;
            _actualForce = JumpForceOvertime.Evaluate(_elapsedTime/JumpPersistance) * JumpForce;
            _Controller.Move(this.transform.up * _actualForce * Time.deltaTime);
            yield return null;
        }
        yield return null;
    }
    #endregion
}
