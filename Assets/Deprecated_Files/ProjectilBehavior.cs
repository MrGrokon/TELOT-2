using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ProjectilBehavior : MonoBehaviour
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
    private void FixedUpdate()
    {
        transform.position += transform.forward * projectilSpeed * Time.deltaTime;
        Debug.DrawRay(transform.position, transform.forward, Color.red);
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 10f))
        {
            if (hit.transform.CompareTag("Ennemy"))
            {
                //Destroy(hit.transform.gameObject);
                hit.collider.gameObject.GetComponent<MonsterBehavior>().TakeDamage(10);
                ObjectReferencer.Instance.Avatar_Object.GetComponent<ProjectEnergie>().hitMarker.gameObject.SetActive(true);
            }
        }

        #region Projectile Autodestroy
        _elapsedLifeTime += Time.deltaTime;
        if(_elapsedLifeTime >= LifeTime){
            Destroy(this.gameObject);
        }
        #endregion
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Ennemy"))
        {
            Destroy(other.gameObject);
        }
        Destroy(gameObject);
        print("Touché qq chose");
    }

    #endregion

}
