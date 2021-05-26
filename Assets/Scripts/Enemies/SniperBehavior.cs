using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

public class SniperBehavior : MonsterBehavior
{
    
    enum State
    {
        Idle,
        Attack,
        Flee,
        Aiming
    }
    private FMOD.Studio.EventInstance WalkEvent;
    private FMOD.Studio.EventDescription WalkDescription;
    private FMOD.Studio.PARAMETER_DESCRIPTION pd;
    FMOD.Studio.PARAMETER_ID parameterID;
    
    private FMOD.Studio.EventInstance ChargeShotEvent;
    private FMOD.Studio.EventDescription ChargeShotDescription;
    private FMOD.Studio.PARAMETER_DESCRIPTION pd2;
    FMOD.Studio.PARAMETER_ID parameterID2;
    private bool walkEventStarted= false;
    
    [Header("AI Properties")]
    [SerializeField] private State _state;
    public float attackInterval; 
    [SerializeField]private float attackCooldown = 0f;
    public float attackDistance;
    public float minimumDistance;
    public float distance;
    [Range(15,100)]
    [Tooltip("Force de recul infligée au joueur")]
    [SerializeField] private float knockbackForce;

    public Transform shotLocation;

    [Header("Debug")]
    public float distanceToPlayer;

    private GameObject Player;

    [Header("Absorption related")]
    [SerializeField] private int energieGivePerShot;
    [Tooltip("Entre -1 et 1 : voir le dot product sur la doc d'unity")]
    [SerializeField] private float absorptionAngle;
    
    [Header("Feedbacks Related")]
    private SniperLaserRendering_Behavior _laser;
    private Animator _anim;
    private bool fleeing = false;

    private void Start()
    {
        _laser = this.GetComponentInChildren<SniperLaserRendering_Behavior>();
        Player = ObjectReferencer.Instance.Avatar_Object;
        
        WalkEvent = FMODUnity.RuntimeManager.CreateInstance("event:/Ennemy/Movement/SniperMovement");

        WalkDescription = FMODUnity.RuntimeManager.GetEventDescription("event:/Ennemy/Movement/SniperMovement");
        WalkDescription.getParameterDescriptionByName("isWalking", out pd);
        parameterID = pd.id;
        
        ChargeShotEvent = FMODUnity.RuntimeManager.CreateInstance("event:/Ennemy/Shoot/SniperChargeShot");

        ChargeShotDescription = FMODUnity.RuntimeManager.GetEventDescription("event:/Ennemy/Shoot/SniperChargeShot");
        ChargeShotDescription.getParameterDescriptionByName("ChargeLevel", out pd2);
        parameterID2 = pd2.id;
        
        _anim = this.GetComponentInChildren<Animator>();
    }

    override public void Update()
    {
        if (attackCooldown < attackInterval && fleeing == false)
        {
            attackCooldown += Time.deltaTime;
            
            float charge = Mathf.Lerp(0, 1, attackInterval / attackCooldown);
            ChargeShotEvent.setParameterByID(parameterID2 , charge);
        }
        Debug.DrawRay(transform.position, (Player.transform.position - transform.position) * 5, Color.red);
        distanceToPlayer = Vector3.Distance(Player.transform.position, transform.position);

        UpdateState();
    }    
    
    private void SwitchState(State newState)
    {  
        if(newState != _state){
            switch(newState){
                case State.Idle:
                fleeing = false;
                transform.rotation = Quaternion.identity;
                _anim.SetTrigger("Stop");
                break;

                case State.Flee:
                fleeing = true;
                _laser.StopAiming();
                _anim.SetTrigger("Move");
                break;

                case State.Attack:
                fleeing = false;
                _laser.StopAiming();
                _anim.SetTrigger("Aim");
                break;

                case State.Aiming:
                fleeing = false;
                StartCoroutine(_laser.StartShootChrono(attackInterval));
                break;
            }   
            _state = newState;
            walkEventStarted = false;
            WalkEvent.stop(STOP_MODE.IMMEDIATE);
        }
    }

    private void UpdateState()
    {
        switch (_state)
        {
            case State.Idle:
                if (distanceToPlayer <= attackDistance && distanceToPlayer > minimumDistance)
                {
                    SwitchState(State.Aiming);
                }

                else if(distanceToPlayer <= minimumDistance){
                    SwitchState(State.Flee);
                }  
                break;


            case State.Aiming:
                transform.LookAt(Player.transform.position);
                _laser.AimAt(ObjectReferencer.Instance.Avatar_Object.transform);
                if(distanceToPlayer > attackDistance){
                    SwitchState(State.Idle);
                }
                    
                else if(distanceToPlayer <= minimumDistance){
                    SwitchState(State.Flee);
                }

                else if (attackCooldown >= attackInterval){
                    SwitchState(State.Attack);   
                }
                break;


            case State.Flee:
                ChargeShotEvent.setParameterByID(parameterID2 , -50);
                Vector3 FleeDirection = transform.position - Player.transform.position;
                //transform.rotation = Quaternion.LookRotation(FleeDirection, Vector3.up);
                _NavMeshAgent.SetDestination(FleeDirection);
                
                if (!walkEventStarted)
                {
                    walkEventStarted = true;
                    WalkEvent.start();
                }
                    
                /*if(distanceToPlayer > attackDistance){
                    
                    SwitchState(State.Idle);
                }
                else*/ if (distanceToPlayer <= attackDistance && distanceToPlayer > minimumDistance)
                {
                    SwitchState(State.Aiming);
                }
                break;


            case State.Attack:
                Shoot();
                break;
        }
    }
    
    private void Shoot()
    {
        _anim.SetTrigger("Shoot");
        if (Physics.Raycast(transform.position, Player.transform.position - transform.position,out RaycastHit hit, Mathf.Infinity))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Ennemy/Shoot/SniperShot", transform.position);
            ChargeShotEvent.setParameterByID(parameterID2 , -50);
            if (hit.transform.CompareTag("Player"))
            {
                if (hit.transform.GetComponent<BlockProjectiles>().Shielding)
                {
                    float D = Vector3.Dot(hit.transform.position, transform.position);
                    if (D > absorptionAngle)
                    {
                        hit.transform.GetComponent<EnergieStored>().AddEnergie(energieGivePerShot);
                        FMODUnity.RuntimeManager.PlayOneShot("event:/Shield/ShieldTanking"); 
                    }
                    else
                    {
                        hit.transform.GetComponent<PlayerLife>().TakeDammage(dammage);
                        print("Dot : " +D);
                    }
                }
                else
                {
                    hit.transform.GetComponent<PlayerLife>().TakeDammage(dammage);
                }
               
            }
            else
            {
                print(hit.transform);
            }
        }
        attackCooldown = 0;
        SwitchState(State.Aiming);
    }

    public override void Die()
    {
        base.Die();
        _anim.SetTrigger("Die");
    }
}
