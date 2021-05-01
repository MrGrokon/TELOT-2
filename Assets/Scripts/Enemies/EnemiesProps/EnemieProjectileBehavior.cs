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
        

        #region Projectile Autodestroy
        _elapsedLifeTime += Time.deltaTime;
        if(_elapsedLifeTime >= LifeTime){
            Destroy(this.gameObject);
        }
        #endregion
    }

    private void FixedUpdate()
    {
        this.transform.Translate(Vector3.forward * projectilSpeed * Time.deltaTime, Space.Self);
        Debug.DrawRay(transform.position, transform.forward, Color.red);
        if (Physics.Raycast(transform.position, Vector3.forward * 2, out RaycastHit hit, 1f))
        {
            
            if (hit.transform.CompareTag("Shield"))
            {
                hit.transform.GetComponentInParent<EnergieStored>().StoreEnergie( hit.transform.GetComponentInParent<BlockProjectiles>().energieStoredPerShot);
            }
            else if (hit.transform.CompareTag("Player"))
            {
                hit.transform.GetComponent<PlayerLife>().TakeDammage(DamageDoned);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            other.transform.GetComponent<PlayerLife>().TakeDammage(DamageDoned);
        }
        else if (other.transform.CompareTag("Shield"))
        {
            other.transform.GetComponentInParent<EnergieStored>().StoreEnergie( other.transform.GetComponentInParent<BlockProjectiles>().energieStoredPerShot);
        }
        else
        {
            print(other.transform.tag);
            Destroy(gameObject);
        }
    }

    #endregion

    public int getDammage()
    {
        return DamageDoned;
    }
}
