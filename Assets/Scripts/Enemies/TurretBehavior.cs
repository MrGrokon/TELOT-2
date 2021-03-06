﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

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
    [SerializeField] private float attackCooldown;
    public float attackDistance;
    public float ProjectileSpeed = 50;
    [Range(2,15)]
    [Tooltip("Taille de la rafale")]
    public int burst;

    [Tooltip("Interval de temps entre chaque projectile de la rafale")]
    [SerializeField] private float burstInterval;
    private float actualBurstInterval;
    private bool bursted = true;


    private GameObject Player;

    public VisualEffect VFX_Loading;
    public VisualEffect VFX_TurretFlare;
    private bool Vfx_triggerOnce = false;
    private LaserRendering_Behavior _laser;

    [Header("Body Parts")]
    public Transform Turret_Body;
    public Transform Turret_Head;
    private Animator _animator;

    private void Start()
    {
        //VFX_Loading = this.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<VisualEffect>();
        //VFX_TurretFlare = this.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<VisualEffect>();
        Player = ObjectReferencer.Instance.Avatar_Object;
        _laser = this.GetComponentInChildren<LaserRendering_Behavior>();
        _animator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    override public void Update()
    {
        

        if (attackCooldown > 0)
        {
            attackCooldown -= 1 * Time.deltaTime;
            if(Vfx_triggerOnce == false){
                Vfx_triggerOnce = true;
                VFX_Loading.SendEvent("StartLoading");
                _animator.SetTrigger("TurretSpotPlayer");
                StartCoroutine(this.GetComponentInChildren<LaserRendering_Behavior>().StartShootChrono(attackCooldown));
            }

        }
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
                //transform.LookAt(predictedPosition(Player.transform.position, projectileThrower.transform.position, Player.transform.GetComponent<PlayerMovementRigidbody>().Motion * Player.transform.GetComponent<PlayerMovementRigidbody>().GetSpeed(), ProjectileSpeed));
                //transform.LookAt(Player.transform.position);

                Turret_Body.transform.LookAt(new Vector3(Player.transform.position.x, this.transform.position.y, Player.transform.position.z));
                //Turret_Head.LookAt(new Vector3(this.transform.position.x, Player.transform.position.y, Player.transform.position.z));
                Turret_Head.LookAt(Player.transform);


                if (attackCooldown <= 0 && bursted)
                {
                    Shoot();
                }
                break;
            case State.Idle:
                if (Vector3.Distance(Player.transform.position , transform.position) <= attackDistance)
                {
                    SwitchState(State.Attack);
                }
                break;
        }
    }

    private void Shoot()
    {
        StartCoroutine(Burst());
    }

    /*private Vector3 predictedPosition(Vector3 targetPosition, Vector3 shooterPosition, Vector3 targetVelocity,
        float projectileSpeed)
    {
        Vector3 displacement = targetPosition - shooterPosition;
        float targetMoveAngle = Vector3.Angle(-displacement, targetVelocity) * Mathf.Deg2Rad;
        //if the target is stopping or if it is impossible for the projectile to catch up with the target (Sine Formula)
        if (targetVelocity.magnitude == 0 || targetVelocity.magnitude > projectileSpeed &&
            Mathf.Sin(targetMoveAngle) / projectileSpeed > Mathf.Cos(targetMoveAngle) / targetVelocity.magnitude)
        {
            //Debug.Log("Position prediction is not feasible.");
            return targetPosition;
        }

        //also Sine Formula
        float shootAngle = Mathf.Asin(Mathf.Sin(targetMoveAngle) * targetVelocity.magnitude / projectileSpeed);
        return targetPosition + targetVelocity * displacement.magnitude /
            Mathf.Sin(Mathf.PI - targetMoveAngle - shootAngle) * Mathf.Sin(shootAngle) / targetVelocity.magnitude;
    }*/

    IEnumerator Burst()
    {
        VFX_Loading.SendEvent("StopLoading");
        Vfx_triggerOnce = false;
        bursted = false;
        for (int i = 0; i < burst; i++)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Ennemy/Shoot/TurretShot", transform.position);
            var proj = Instantiate(projectilePrefab, projectileThrower.transform.position, projectileThrower.transform.rotation);
            proj.GetComponent<EnemieProjectileBehavior>().SetSpeed(ProjectileSpeed);
            VFX_TurretFlare.SendEvent("Shoot");
            yield return new WaitForSeconds(burstInterval);
        }
        attackCooldown = attackInterval;
        bursted = true;
    }
}
