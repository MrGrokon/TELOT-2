using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemieProjectileBehavior : MonoBehaviour
{
    [Range(1f, 10f)]
    public float LifeTime = 5f;
    private float _elapsedLifeTime = 0f;

    private float projectilSpeed;
    [SerializeField] private int DamageDoned;

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

    private void FixedUpdate()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.red);
        if (Physics.Raycast(transform.position, Vector3.forward, out RaycastHit hit, 12f))
        {
            if (!hit.transform.CompareTag("Player"))
            {
                Destroy(transform.gameObject);
            }
        }
    }

    #endregion
    

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("collide with " + other.gameObject.name);
        Destroy(gameObject);
    }

    public int getDammage()
    {
        return DamageDoned;
    }
}
