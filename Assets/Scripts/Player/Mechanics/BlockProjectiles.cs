using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.UI;

public class BlockProjectiles : MonoBehaviour
{
    private Animator WeaponAnimator;

    [Range(0.3f, 5f)]
    public float TimeToBeActive = 0.75f;
    public float ShieldHitboxRange = 1f;
    public LayerMask ProjectileLayerMask;

    private GameObject _Shield_Rendr;
    private Transform _Shield_Pivot;
    private EnergieStored _Energie;
    private bool Shielding = false;
    private float _elapsedTime = 0f;
    private Slider shieldRemainSlider;
    public Image SliderFillImage;
    public int energieStoredPerShot;
    public float shieldEnergy;
    public bool shieldDepleted = false;
    public bool AbsorptionByMovement;
    public float depletationFactor = 1;

    private FMOD.Studio.EventInstance shieldIdle;

    #region Unity Functions

        private void Awake() {
            WeaponAnimator = GameObject.Find("Shotgun_Pivot").GetComponent<Animator>();
            shieldRemainSlider = GameObject.Find("Slider").GetComponent<Slider>();

            shieldIdle = FMODUnity.RuntimeManager.CreateInstance("event:/Shield/AbsorptionIdle");
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
            {
                SliderFillImage.color = new Color(134, 255, 107);
                shieldDepleted = false;
            }
                
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
                    WeaponAnimator.SetTrigger("ShieldActivate");
                    shieldEnergy -= 1 * Time.deltaTime * depletationFactor;
                    Collider[] _hits = Physics.OverlapSphere(_Shield_Pivot.position, ShieldHitboxRange, ProjectileLayerMask);

                    foreach(var Projectiles in _hits)
                    {
                        Destroy(Projectiles);
                        _Energie.StoreEnergie(energieStoredPerShot);
                        FMODUnity.RuntimeManager.PlayOneShot("event:/Shield/ShieldTanking"); 
                    }
                }
                else if(shieldEnergy <= 0) 
                {
                    Shielding = false;
                    _Shield_Rendr.SetActive(false);
                    shieldIdle.stop(STOP_MODE.ALLOWFADEOUT);
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Shield/ShieldOff", transform.position);
                    shieldEnergy = 0;
                    shieldDepleted = true;
                    SliderFillImage.color = Color.gray;
                }
                
            }
        }

        IEnumerator ShieldSoundManager()
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Shield/AbsorptionOn", transform.position);
            yield return new WaitForSeconds(0.15f);
            shieldIdle.start();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_Shield_Pivot.position, ShieldHitboxRange);
        }

        #endregion
    
}
