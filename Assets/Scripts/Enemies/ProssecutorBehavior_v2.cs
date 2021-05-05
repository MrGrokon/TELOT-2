using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProssecutorBehavior_v2 : MonsterBehavior
{
    [Header("Général Prossécutor Parametter")]

    [Range(35f, 150f)]
    public float PlayerReachDistance = 50f;
    public float VerticaleAmplitude = 10f;
    public float HorizontalAmplitude = 30f;
    public float DistanceToSurface = 5f;

    private float _RandomSeed, _MotionSeed;
    private bool CanMove = true;
    private Animator _anim;

    [Header("Shockwave Parameters")]
    public float TimeBetweenTwoAttacks = 5f;
    public float ImpulseForce = 25f;
    public AnimationCurve ShockwaveGrowth;
    public ShockwaveBehavior ShockwaveObject;
    public float Shockwave_Duration = 1.5f;
    public float Shockwave_Distance = 3f;
    public float ChargingTimeBeforeShockwave = 0.5f;

    private float TimeSinceLastAttack = 0f;
    private bool _CanAttack = true;

    private bool ShockwaveIsLaunched = false;


    private FMOD.Studio.EventInstance Movement;

    public override void Awake()
    {
        base.Awake();
        _RandomSeed = Random.Range(-1f, 1f);
        _MotionSeed = Random.Range(0.5f, 1f);

        MotionSpeed *= _MotionSeed;

        _anim = this.GetComponent<Animator>();
        Movement = FMODUnity.RuntimeManager.CreateInstance("event:/Ennemy/Movement/DroneMovement");
        Movement.start();
    }

    public override void Update()
    {
        base.Update();
        this.transform.LookAt(ObjectReferencer.Instance.Avatar_Object.transform);
        
        
        #region Procecutor Movement toward Player
        Vector3 MovementVector = Vector3.zero;

        MovementVector = (ObjectReferencer.Instance.Avatar_Object.transform.position - this.transform.position) * MotionSpeed;
        Debug.DrawRay(this.transform.position, MovementVector, Color.red);
        if(Physics.SphereCast(this.transform.position, 0.5f, MovementVector, out RaycastHit _hit, 5f)){
            //Debug.Log("can't perfom base movement");
            if(_hit.collider.tag == "Ground"){
                MovementVector = Vector3.up * MotionSpeed *10f;
            }
            else if(_hit.collider.tag == "Pillars"){
                if(_RandomSeed >= 0){
                    MovementVector = this.transform.right * MotionSpeed * 10f;
                }
                else{
                    MovementVector = -this.transform.right * MotionSpeed *10f;
                }
            }
            else if(_hit.collider.tag == "Ennemy"){
                Debug.Log("Ennemy in the way");
                if(_RandomSeed >= 0){
                    MovementVector += this.transform.right * MotionSpeed;
                }
                else{
                    MovementVector += -this.transform.right * MotionSpeed;
                }
            }
            else{
                MovementVector = Vector3.zero;
            }                
        }
            #region Ossilation Movement
            if(Physics.Raycast(this.transform.position, -this.transform.up, DistanceToSurface) == false){
                //float VerticalOssilation = Mathf.Sin(Time.timeSinceLevelLoad * _RandomSeed) * VerticaleAmplitude;
                MovementVector += new Vector3(0f, Mathf.Sin(Time.timeSinceLevelLoad ) * _RandomSeed * VerticaleAmplitude, 0f);
                //MovementVector.y += VerticalOssilation;
            }
            else{
                MovementVector += Vector3.up * MotionSpeed;
            }

            if(Physics.Raycast(this.transform.position, this.transform.right, DistanceToSurface) == false && Physics.Raycast(this.transform.position, -this.transform.right, DistanceToSurface) == false){
                MovementVector += new Vector3(Mathf.Sin(Time.timeSinceLevelLoad ) * _RandomSeed * HorizontalAmplitude, 0f, 0f);
            }
            
            #endregion
        #endregion
            
        #region Obstacle Detection
        Physics.Raycast(this.transform.position, Vector3.down, out RaycastHit _hit2 , Mathf.Infinity, LayerMask.GetMask("Ground"));
        /*if(Vector3.Distance(this.transform.position , _hit2.point) > DistanceToSurface){
            MovementVector += Vector3.down;
        }
        else{
            MovementVector += (Vector3.up * MotionSpeed);
        }*/
        #endregion

        //Apply motion to Prossecutor
        if(CanMove){
            this.transform.Translate( MovementVector * Time.deltaTime, Space.World);
        }

        #region Shockwave Initialisation
        if(Vector3.Distance(this.transform.position, ObjectReferencer.Instance.Avatar_Object.transform.position) <= Shockwave_Distance && _CanAttack){
            if(ShockwaveIsLaunched == false){
                StartCoroutine(ShockwaveAttack());
            }
        }

        if(_CanAttack == false){
            TimeSinceLastAttack += Time.deltaTime;
            if(TimeSinceLastAttack >= TimeBetweenTwoAttacks){
                TimeSinceLastAttack = 0f;
                _CanAttack = true;
            }
        }
        #endregion
    }

    IEnumerator  ShockwaveAttack(){
        ShockwaveIsLaunched = true;
        _CanAttack = false;
        Debug.Log("Shockwave attack launched");
        _anim.SetTrigger("PlayerDetected");
        CanMove = false;
        float _elapsedTime = 0f;

        while(_elapsedTime < ChargingTimeBeforeShockwave){
            _elapsedTime += Time.deltaTime;
            yield return null;
        }

        _elapsedTime = 0f;
        _anim.SetTrigger("Shockwave");
        FMODUnity.RuntimeManager.PlayOneShot("event:/Ennemy/Shoot/DroneAttack");
        ShockwaveBehavior _shockwave_obj =  Instantiate(ShockwaveObject, this.transform.position, Quaternion.identity);
        _shockwave_obj.SetLifeTime(Shockwave_Duration);
        _shockwave_obj.SetGrowthCurve(ShockwaveGrowth);
        _shockwave_obj.SetDammage(dammage);
        _shockwave_obj.SetImpulseForce(ImpulseForce);

        while(_elapsedTime < Shockwave_Duration){
            _elapsedTime += Time.deltaTime;
            yield return null;
        }

        //init the shockwave

        CanMove = true;
        ShockwaveIsLaunched = false;
        yield return null;
    }
}
