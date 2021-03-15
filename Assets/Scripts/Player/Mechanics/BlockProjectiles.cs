﻿using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.UI;

public class BlockProjectiles : MonoBehaviour
{
    [Range(0.3f, 5f)]
    public float TimeToBeActive = 0.75f;
    public float ShieldHitboxRange = 1f;
    public LayerMask ProjectileLayerMask;

    private GameObject _Shield_Rendr;
    private Transform _Shield_Pivot;
    private EnergieStored _Energie;
    private bool Shielding = false;
    private float _elapsedTime = 0f;
    public Slider shieldRemainSlider;
    public int energieStoredPerShot;
    public float shieldEnergy;
    public bool shieldDepleted = false;
    public bool AbsorptionByMovement;
    public float depletationFactor = 1;

    private FMOD.Studio.EventInstance shieldIdle;

    #region Unity Functions

        private void Awake() {
            shieldIdle = FMODUnity.RuntimeManager.CreateInstance("event:/Shield/ShieldIdle");
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(shieldIdle, transform,
                GetComponent<Rigidbody>());
            _Shield_Rendr = GameObject.Find("ShieldDebug");
            _Shield_Pivot = GameObject.Find("Shield_Pivot").transform;
            _Shield_Rendr.SetActive(false);
            _Energie = this.GetComponent<EnergieStored>();
            shieldRemainSlider.value = TimeToBeActive / TimeToBeActive;
            if(_Energie == null){
                Debug.Log("EnergieStored not defined");
            }
        }

        private void Update() {
            ActivateShield();
            
            if (GetComponent<PlayerMovementRigidbody>().Motion != Vector3.zero)
            {
                if(!Shielding)
                    shieldEnergy += 0.5f * Time.deltaTime;
                
            }
            else
            {
                shieldEnergy += 0.1f * Time.deltaTime;
            }
            shieldEnergy = Mathf.Clamp(shieldEnergy, 0, TimeToBeActive);
            if(Input.GetButtonDown("Fire2") && shieldEnergy > 0 && !shieldDepleted)
            {
                Shielding = !Shielding;
                StartCoroutine(ShieldSoundManager());
                if (!Shielding)
                {
                    Shielding = false;
                    _Shield_Rendr.SetActive(false);
                    shieldIdle.stop(STOP_MODE.ALLOWFADEOUT);
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Shield/ShieldOff", transform.position);
                }
            }

            if (shieldEnergy >= TimeToBeActive)
                shieldDepleted = false;
            if (AbsorptionByMovement)
            {
                if (GetComponent<PlayerMovementRigidbody>().Motion != Vector3.zero)
                    depletationFactor = 1;
                else
                {
                    depletationFactor = 2;
                }
            }
            
        }

        private void ActivateShield()
        {
            shieldRemainSlider.value = shieldEnergy / TimeToBeActive;
            if (Shielding)
            {
                if (shieldEnergy > 0)
                {
                    _Shield_Rendr.SetActive(true);
                    shieldEnergy -= 1 * Time.deltaTime * depletationFactor;
                    Collider[] _hits = Physics.OverlapSphere(_Shield_Pivot.position, ShieldHitboxRange, ProjectileLayerMask);
                    /*Collider[] _hitsR = Physics.OverlapSphere(transform.position + transform.right, ShieldHitboxRange, ProjectileLayerMask);
                    Collider[] _hitsL = Physics.OverlapSphere(transform.position - transform.right, ShieldHitboxRange, ProjectileLayerMask);*/
                
                    foreach(var Projectiles in _hits)
                    {
                        Destroy(Projectiles);
                        _Energie.StoreEnergie(energieStoredPerShot);
                        FMODUnity.RuntimeManager.PlayOneShot("event:/Shield/ShieldTanking"); 
                    }
                   /*foreach(var Projectiles in _hitsR)
                    {
                        Destroy(Projectiles);
                        _Energie.StoreEnergie(energieStoredPerShot);
                        FMODUnity.RuntimeManager.PlayOneShot("event:/Shield/ShieldTanking"); 
                    }
                    foreach(var Projectiles in _hitsL)
                    {
                        Destroy(Projectiles);
                        _Energie.StoreEnergie(energieStoredPerShot);
                        FMODUnity.RuntimeManager.PlayOneShot("event:/Shield/ShieldTanking"); 
                    }*/
                }
                else if(shieldEnergy <= 0) 
                {
                    Shielding = false;
                    _Shield_Rendr.SetActive(false);
                    shieldIdle.stop(STOP_MODE.ALLOWFADEOUT);
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Shield/ShieldOff", transform.position);
                    shieldEnergy = 0;
                    shieldDepleted = true;
                }
                
            }
        }

        IEnumerator ShieldSoundManager()
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Shield/ShieldOn", transform.position);
            yield return new WaitForSeconds(1);
            shieldIdle.start();
        }

    #endregion
    
}
