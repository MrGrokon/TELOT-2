using System;
using System.Collections;
using System.Collections.Generic;
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

    [Header("Debug")]
    public float distanceToPlayer;

    [Header("Feedback Properties")] 
    [Tooltip("Couleur de départ, quand la visée vient d'être commencé")]
    [SerializeField] private Color nonShootReadyColor;
    [Tooltip("Couleur de fin, quand le shoot va être initié")]
    [SerializeField] private Color shootReadyColor;

    private GameObject Player;
    // Start is called before the first frame update
    private void Start()
    {
        Player = ObjectReferencer.Instance.Avatar_Object;
    }

    override public void Update()
    {
        if (attackCooldown < attackInterval)
        {
            attackCooldown += 1 * Time.deltaTime;
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
                Vector3 FleeDirection = transform.position - Player.transform.position;
                transform.rotation = Quaternion.LookRotation(FleeDirection, Vector3.up);
                _NavMeshAgent.SetDestination(FleeDirection);
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
            if (hit.transform.CompareTag("Player"))
            {
                Player.GetComponent<PlayerLife>().TakeDammage(dammage);
                Player.GetComponent<Rigidbody>().AddForce((Player.transform.position - transform.position).normalized * knockbackForce, ForceMode.Impulse);
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
