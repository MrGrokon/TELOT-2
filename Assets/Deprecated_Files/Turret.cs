using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    enum State
    {
        Idle,
        Attack
    }

    public GameObject projectilePrefab;
    public GameObject projectileThrower;
    [Header("AI Properties")]
    [SerializeField] private State _state;
    public float attackInterval;
    private float attackCooldown;
    public float attackDistance;
    private GameObject Player;
    [Header("Debug")]
    public float distanceToPlayer;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(Player.transform.position, transform.position) <= attackDistance)
        {
            SwitchState(State.Attack);
        }

        if (attackCooldown > 0)
        {
            attackCooldown -= 1 * Time.deltaTime;
        }

        distanceToPlayer = Vector3.Distance(Player.transform.position, transform.position);

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
                transform.LookAt(Player.transform);
                if (attackCooldown <= 0)
                {
                    Shoot();
                }
                break;
        }
    }

    private void Shoot()
    {
        var direction = (Player.transform.position - transform.position).normalized;
        var proj = Instantiate(projectilePrefab, direction * 2 + projectileThrower.transform.position, Quaternion.identity);
        proj.GetComponent<Rigidbody>().AddForce(11000 * direction);
        attackCooldown = attackInterval;
    }
}
