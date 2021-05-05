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
    [SerializeField]private float attackCooldown;
    public float attackDistance;
    public float minimumDistance;
    public float distance;
    [SerializeField] private LineRenderer shootFeedback;
    [Range(15,100)]
    [Tooltip("Force de recul infligée au joueur")]
    [SerializeField] private float knockbackForce;

    public Transform shotLocation;

    [Header("Debug")]
    public float distanceToPlayer;

    [Header("Feedback Properties")] 
    [Tooltip("Couleur de départ, quand la visée vient d'être commencé")]
    [SerializeField] private Color nonShootReadyColor;
    [Tooltip("Couleur de fin, quand le shoot va être initié")]
    [SerializeField] private Color shootReadyColor;

    private GameObject Player;

    [Header("Absorption related")]
    [SerializeField] private int energieGivePerShot;
    [Tooltip("Entre -1 et 1 : voir le dot product sur la doc d'unity")]
    [SerializeField] private float absorptionAngle;
    // Start is called before the first frame update
    private void Start()
    {
        Player = ObjectReferencer.Instance.Avatar_Object;
        
        WalkEvent = FMODUnity.RuntimeManager.CreateInstance("event:/Ennemy/Movement/SniperMovement");

        WalkDescription = FMODUnity.RuntimeManager.GetEventDescription("event:/Ennemy/Movement/SniperMovement");
        WalkDescription.getParameterDescriptionByName("isWalking", out pd);
        parameterID = pd.id;
        
        ChargeShotEvent = FMODUnity.RuntimeManager.CreateInstance("event:/Ennemy/Shoot/SniperChargeShot");

        ChargeShotDescription = FMODUnity.RuntimeManager.GetEventDescription("event:/Ennemy/Shoot/SniperChargeShot");
        ChargeShotDescription.getParameterDescriptionByName("ChargeLevel", out pd2);
        parameterID2 = pd2.id;
    }

    override public void Update()
    {
        if (attackCooldown < attackInterval)
        {
            attackCooldown += 1 * Time.deltaTime;
            
            float charge = Mathf.Lerp(0, 1, attackInterval / attackCooldown);
            ChargeShotEvent.setParameterByID(parameterID2 , charge);
        }
        Debug.DrawRay(transform.position, (Player.transform.position - transform.position) * 5, Color.red);
        distanceToPlayer = Vector3.Distance(Player.transform.position, transform.position);

        UpdateState();
    }
    
    private void EnterState()
    {
        switch (_state)
        {
            case State.Idle:
                transform.rotation = Quaternion.identity;
                break;
        }
    }

    
    
    private void SwitchState(State newState)
    {
        _state = newState;
        walkEventStarted = false;
        WalkEvent.stop(STOP_MODE.IMMEDIATE);
        EnterState();
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
                else if(distanceToPlayer <= minimumDistance)
                    SwitchState(State.Flee);
                break;
            case State.Aiming:
                transform.LookAt(Player.transform.position);
                if (Physics.Raycast(transform.position, Player.transform.position - transform.position,
                    out RaycastHit hit, Mathf.Infinity))
                {
                    shootFeedback.SetPosition(0, transform.position);
                    shootFeedback.SetPosition(1, hit.point);
                }
                shootFeedback.material.color = Color.Lerp(nonShootReadyColor, shootReadyColor, attackCooldown/attackInterval);
                if(distanceToPlayer > attackDistance)
                    SwitchState(State.Idle);
                else if(distanceToPlayer <= minimumDistance)
                    SwitchState(State.Flee);
                else if (attackCooldown >= attackInterval)
                {
                    SwitchState(State.Attack);   
                }
                break;
            case State.Flee:
                ChargeShotEvent.setParameterByID(parameterID2 , -50);
                Vector3 FleeDirection = transform.position - Player.transform.position;
                transform.rotation = Quaternion.LookRotation(FleeDirection, Vector3.up);
                _NavMeshAgent.SetDestination(FleeDirection);
                
                if (!walkEventStarted)
                {
                    walkEventStarted = true;
                    WalkEvent.start();
                }
                    
                if(distanceToPlayer > attackDistance)
                    SwitchState(State.Idle);
                else if (distanceToPlayer <= attackDistance && distanceToPlayer > minimumDistance)
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
}
