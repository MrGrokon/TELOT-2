using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.VFX;

public class ProjectilBehavior : MonoBehaviour
{  
    [Range(1f, 10f)]
    public float LifeTime = 5f;
    private float _elapsedLifeTime = 0f;
    private VisualEffect Projectil_OnHitSurface_VFX;
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
    void Start()
    {
        Projectil_OnHitSurface_VFX = GameObject.Find("VFX_Hit").GetComponent<VisualEffect>();
    }

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
                Destroy(this.gameObject);
            }
            else{
                //Projectil_OnHitSurface_VFX.SendEvent("SpawnImpact");
                //Projectil_OnHitSurface_VFX.SetVector3("WorldSpace_Position", hit.point);

                /*Projectil_OnHitSurface_VFX.SetVector3(1, new Vector3());
                Projectil_OnHitSurface_VFX.Play();*/
            }
        }

        #region Projectile Autodestroy
        _elapsedLifeTime += Time.deltaTime;
        if(_elapsedLifeTime >= LifeTime){
            Destroy(this.gameObject);
        }
        #endregion
    }

    #endregion

}
