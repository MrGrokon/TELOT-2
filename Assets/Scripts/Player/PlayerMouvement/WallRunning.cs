using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Génériques")]
    [Range(0.1f, 3f)]
    public float WallDistanceDetection = 1f;
    public LayerMask RunnableWallLayer;
    public bool WallOnRight = false, WallOnLeft = false;

    [Header("Aderence Variables")]
    public AnimationCurve GravityVariationOverTime;
    [Range(1f, 10f)]
    public float TimeToStickToWall = 5f;
    private float _elapsedTime = 0f;

    [Header("Jump From a wall")]
    [Range(5f, 30f)]
    public float JumpForce = 20f;
    [Range(0.5f, 5f)]
    public float JumpPersistance = 2f;

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
        if(_States.OnGround == false){
            Vector3 LastWall_normal = Vector3.zero;

            #region Detect & Assign the wall i walk on
            WallOnRight = Physics.Raycast(this.transform.position, this.transform.right, out RaycastHit RightHit, WallDistanceDetection, RunnableWallLayer.value);
            WallOnLeft = Physics.Raycast(this.transform.position, -this.transform.right, out RaycastHit LeftHit, WallDistanceDetection, RunnableWallLayer.value);
            
            if(WallOnLeft == true){
                WallRunnedOn = LeftHit.collider;
                LastWall_normal = LeftHit.normal;
            }

            if(WallOnRight == true){
                WallRunnedOn = RightHit.collider;
                LastWall_normal = RightHit.normal;
            }
            #endregion

            if( WallOnLeft || WallOnRight){
                //Loop on first frame of wall run
                if(TriggerEnterWallRun){
                    //Debug.Log("Trigger Wall Run");
                    _Physic.StopVelocity();
                    TriggerEnterWallRun = false;
                    //TODO: feedbacks
                    if(WallOnLeft){
                        StartCoroutine(FeedbackManager.Instance.AngularCameraRotation(FeedbackManager.CameraDirection.Left));
                    }
                    else
                        StartCoroutine(FeedbackManager.Instance.AngularCameraRotation(FeedbackManager.CameraDirection.Right));
                }

                _elapsedTime += Time.deltaTime;
                _Physic.GravityForce = GravityVariationOverTime.Evaluate(_elapsedTime/TimeToStickToWall) * _Physic.GetBaseGravity();

                //todo: input detection overide
                if(Input.GetButtonDown("Jump") && LastWall_normal != Vector3.zero){
                    Debug.Log("Jump From a wall");
                    StartCoroutine(Persitant_Jump((LastWall_normal + Vector3.up).normalized));
                }

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

    #region Coroutines
        IEnumerator Persitant_Jump(Vector3 JumpDirection){
            float _elapsedTime = 0f;

            while(_elapsedTime < JumpPersistance){
                _elapsedTime += Time.deltaTime;
                _Controller.Move( JumpDirection * JumpForce * Time.deltaTime);
                yield return null;
            }
            yield return null;
        }
    #endregion
}

