using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProssecutorBehavior_v2 : MonsterBehavior
{
    [Header("Général Prossécutor Parametter")]

    [Range(35f, 150f)]
    public float PlayerReachDistance = 50f;
    public float VerticaleAmplitude = 10f;
    public float PerfectDistFromGround = 5f;

    private float _RandomSeed, _MotionSeed;

    [Header("Shoot Mecaniques Parameters")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 30f;
    public float TimeBetweenShoots = 5f;
    public float EngageDistance = 30f;
    private float _elapsedShootTime = 0f;
    private bool _canShoot = false;

    public override void Awake()
    {
        base.Awake();
        _RandomSeed = Random.Range(-1f, 1f);
        _MotionSeed = Random.Range(0.5f, 1f);

        MotionSpeed *= _MotionSeed;
    }

    public override void Update()
    {
        base.Update();
        this.transform.LookAt(ObjectReferencer.Instance.Avatar_Object.transform);
        
        
        #region Procecutor Movement toward Player
        Vector3 MovementVector = Vector3.zero;

        //if(Vector3.Distance(this.transform.position, ObjectReferencer.Instance.Avatar_Object.transform.position) > PlayerReachDistance){
            MovementVector = (ObjectReferencer.Instance.Avatar_Object.transform.position - this.transform.position) * MotionSpeed;
            Debug.DrawRay(this.transform.position, MovementVector, Color.red);
            if(Physics.Raycast(this.transform.position, MovementVector, out RaycastHit _hit, 3f)){
                Debug.Log("can't perfom base movement");
                if(_hit.collider.tag == "Ground"){
                    MovementVector = Vector3.up * MotionSpeed *4f;
                }
                else if(_hit.collider.tag == "Pillars"){
                    if(_RandomSeed >= 0){
                        MovementVector = this.transform.right * MotionSpeed;
                    }
                    else{
                        MovementVector = -this.transform.right * MotionSpeed;
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
        //}

        float VerticalOssilation = Mathf.Sin(Time.timeSinceLevelLoad * _RandomSeed) * VerticaleAmplitude;

        MovementVector.y += VerticalOssilation;
        #endregion
        
        #region Obstacle Detection
        Physics.Raycast(this.transform.position,Vector3.down, out RaycastHit _hit2 , Mathf.Infinity, LayerMask.GetMask("Ground"));
        if(Vector3.Distance(this.transform.position , _hit2.point) > PerfectDistFromGround){
            MovementVector += Vector3.down;
        }
        else{
            MovementVector += (Vector3.up * MotionSpeed);

        }
        #endregion
        
        //Apply motion to Prossecutor
        this.transform.Translate( MovementVector * Time.deltaTime, Space.World);
    
        #region Shoot if clear line of sight
        if(_canShoot == false){
            _elapsedShootTime += Time.deltaTime;
            if(_elapsedShootTime >= (TimeBetweenShoots * _MotionSeed)){
                _canShoot = true;
            }
        }
        else if(_canShoot == true){
            /*
            Physics.Raycast(this.transform.position, this.transform.forward, out RaycastHit _shootHit ,EngageDistance);
            if(_shootHit.collider.tag == "Player"){
                Shoot();
            }
            */
            Shoot();
        }
        #endregion
    }

    void Shoot()
    {
        //Debug.Log("prosecutor attack");
        _canShoot = false;
        _elapsedShootTime = 0f;
        GameObject _proj = Instantiate(projectilePrefab, this.transform.position, this.transform.rotation);
        _proj.GetComponent<EnemieProjectileBehavior>().SetSpeed(projectileSpeed);
    }
}
