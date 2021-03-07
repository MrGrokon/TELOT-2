using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehavior : MonsterBehavior
{
    enum State
    {
        Idle,
        Attack
    }

    [Header("Initiate by Hand")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject projectileThrower;

    [Header("AI Properties")]
    [SerializeField] private State _state;
    public float attackInterval;
    private float attackCooldown;
    public float attackDistance;
    public float ProjectileSpeed = 50;

    [Header("Debug")]
    public float distanceToPlayer;

    // Update is called once per frame
    override public void Update()
    {
        if (Vector3.Distance(ObjectReferencer.Instance.Avatar_Object.transform.position , transform.position) <= attackDistance)
        {
            SwitchState(State.Attack);
        }

        if (attackCooldown > 0)
        {
            attackCooldown -= 1 * Time.deltaTime;
        }

        distanceToPlayer = Vector3.Distance(ObjectReferencer.Instance.Avatar_Object.transform.position, transform.position);

        UpdateState();
    }

    private void SwitchState(State newState)
    {
        _state = newState;
        EnterState();
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

    private void UpdateState()
    {
        switch (_state)
        {
            case State.Attack:
                transform.LookAt(ObjectReferencer.Instance.Avatar_Object.transform);
                if (attackCooldown <= 0)
                {
                    Shoot();
                }
                break;
        }
    }

    private void Shoot()
    {
        var direction = (ObjectReferencer.Instance.Avatar_Object.transform.position - transform.position).normalized;
        var proj = Instantiate(projectilePrefab, direction * 2 + projectileThrower.transform.position, projectileThrower.transform.rotation);
        proj.GetComponent<EnemieProjectileBehavior>().SetSpeed(ProjectileSpeed);
        attackCooldown = attackInterval;
    }
}
