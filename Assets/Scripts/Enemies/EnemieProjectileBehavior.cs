﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemieProjectileBehavior : MonoBehaviour
{
    [Range(1f, 10f)]
    public float LifeTime = 5f;
    private float _elapsedLifeTime = 0f;

    private float projectilSpeed;
    private int DamageDoned;

    #region Setters
    public void SetSpeed(float _speed){
    projectilSpeed = _speed;
    }

    public void SetDamageAmountDealed(int _dmg){
        DamageDoned = _dmg;
    }
    #endregion
    
    #region Unity Functions
    private void Update() {
        this.transform.Translate(Vector3.forward * projectilSpeed * Time.deltaTime, Space.Self);

        #region Projectile Autodestroy
        _elapsedLifeTime += Time.deltaTime;
        if(_elapsedLifeTime >= LifeTime){
            Destroy(this.gameObject);
        }
        #endregion
    }
    #endregion

    void OnCollisionEnter(Collision other)
    {
        //Debug.Log("collide with " + other.gameObject.name);
        Destroy(gameObject);
    }
}