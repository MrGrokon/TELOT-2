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
    private void Update()
    {
        transform.position += transform.forward * projectilSpeed * Time.deltaTime;
        Debug.DrawRay(transform.position, transform.forward, Color.red);

        if (Physics.Raycast(transform.position, transform.forward , out RaycastHit hit, 4f))
        {
            if (hit.transform.CompareTag("Ennemy"))
            {
                Destroy(hit.transform.gameObject);
                Destroy(this.gameObject);
            }
        }

        #region Projectile Autodestroy
        _elapsedLifeTime += Time.deltaTime;
        if(_elapsedLifeTime >= LifeTime){
            Destroy(this.gameObject);
        }
        #endregion
    }

    private void OnCollisionEnter(Collision col) {
        if(col.gameObject.tag != "Ennemy"){
            Destroy(this.gameObject);
        }
        else if (col.gameObject.CompareTag("Ennemy"))
        {
            Destroy(col.gameObject);
            Destroy(this.gameObject);
        }
    }
    #endregion

}
