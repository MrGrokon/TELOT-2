using System.Collections;
using System.Collections.Generic;
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

        #region Projectile Autodestroy
        _elapsedLifeTime += Time.deltaTime;
        if(_elapsedLifeTime >= LifeTime){
            Destroy(this.gameObject);
        }
        #endregion
    }

    private void OnCollisionEnter(Collision col) {
        if(col.gameObject.tag != "Enemie"){
            Destroy(this.gameObject);
        }
        else{
            //do some damage
        }
    }
    #endregion

}
