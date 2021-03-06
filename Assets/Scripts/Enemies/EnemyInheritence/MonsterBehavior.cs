﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterBehavior : MonoBehaviour
{
    [Header("Health Parameters")]
    private bool isDead = false;
    [Range(1, 1000)]
    public int StartHealth = 100;
    private int _Health;
    public GameObject DyingEffect;

    [Header("Weaponery Parameters")]
    public int dammage;

    [Header("Mouvement Parameter")]
    [Range(0.1f, 5f)]
    public float MotionSpeed = 10f;

    public NavMeshAgent _NavMeshAgent;

    

    
    #region Unity Base Functions
        public virtual void Awake() {
            _Health = StartHealth;
            if (GetComponent<NavMeshAgent>())
            {
                _NavMeshAgent = GetComponent<NavMeshAgent>();
                _NavMeshAgent.speed = MotionSpeed;
            }
        }

        virtual public void Update() {
            // base update shits
        }
    #endregion
    


    #region Health Related Functions
        virtual public void TakeDamage(int _amount){
            if(_Health - _amount < 0){
                Debug.Log( this.name + " died");
                _Health = 0;
                Die();
            }
            else
            _Health -= _amount;
        }

        public bool IsAlive(){
            if(_Health > 0){
                return true;
            }
            return false;
        }

        virtual public void Die(){
            if(isDead==false){
                isDead = true;
                FMODUnity.RuntimeManager.PlayOneShot("event:/Ennemy/Death/TurretDrone Death", transform.position);
                Destroy(this.gameObject);
                GameObject[] triggerlist = GameObject.FindGameObjectsWithTag("RoomTrigger");
                foreach (var _trigger in triggerlist)
                {
                    RoomTrigger roomTrigger = _trigger.GetComponent<RoomTrigger>();
                    if(roomTrigger.monsters.Contains(this)){
                        roomTrigger.monsters.Remove(this);
                    }
                }
                // dying feedbacks
                
                Instantiate(DyingEffect, this.transform.position + (Vector3.up * 2f), Quaternion.identity);
            }
        }
    #endregion
}
