using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class BlockProjectiles : MonoBehaviour
{
    private Animator Weapon_Animator;

    [Range(0.3f, 5f)]
    public float TimeToBeActive = 0.75f;
    public float ShieldHitboxRange = 1f;
    public LayerMask ProjectileLayerMask;

    private GameObject _Shield_Rendr;
    private Transform _Shield_Pivot;
    private EnergieStored _Energie;
    public bool Shielding = false;
    private float _elapsedTime = 0f;
    public Image SliderFillImage;
    public int energieStoredPerShot;
    public float shieldEnergy;
    public bool shieldDepleted = false;
    public bool AbsorptionByMovement;
    public float depletationFactor = 1;

    public VisualEffect VFXAbs;
    private Material VFX_Distorded_mat;

    private FMOD.Studio.EventInstance shieldIdle;

    #region Unity Functions

        private void Awake() {
            Weapon_Animator = GameObject.Find("Shotgun_Pivot").GetComponent<Animator>();

            shieldIdle = FMODUnity.RuntimeManager.CreateInstance("event:/Absorption/AbsorptionIdle");
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(shieldIdle, transform,
                GetComponent<Rigidbody>());
            _Shield_Rendr = GameObject.Find("ShieldDebug");
            _Shield_Pivot = GameObject.Find("Shield_Pivot").transform;
            //_Shield_Rendr.SetActive(false);
            _Energie = this.GetComponent<EnergieStored>();
            SliderFillImage.fillAmount = TimeToBeActive / TimeToBeActive;
            VFXAbs.SendEvent("StopShield");
            VFX_Distorded_mat = _Shield_Rendr.GetComponent<MeshRenderer>().material;
            VFX_Distorded_mat.SetFloat("_Activness", 0f);
            if(_Energie == null){
                Debug.Log("EnergieStored not defined");
                   
            }
        }

        private void Update() {
            Weapon_Animator.SetFloat("ChargeLevel", SliderFillImage.fillAmount);
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
                if (Shielding)
                {
                    VFXAbs.SendEvent("StartShield");
                    StartCoroutine(Shield_Material_Manager(true));
                }
                else
                {
                    VFXAbs.SendEvent("StopShield");
                    StartCoroutine(Shield_Material_Manager(false));
                }
                StartCoroutine(ShieldSoundManager());
                if (!Shielding)
                {
                    Shielding = false;
                    //_Shield_Rendr.SetActive(false);
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
            SliderFillImage.fillAmount = shieldEnergy / TimeToBeActive;
            if (Shielding)
            {
                if (shieldEnergy > 0)
                {
                    //_Shield_Rendr.SetActive(true);
                    Weapon_Animator.SetTrigger("ShieldActivate");

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
                    Weapon_Animator.SetTrigger("ShieldActivate");
                    VFXAbs.SendEvent("StopShield");
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

        /*private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_Shield_Pivot.position, ShieldHitboxRange);
        }*/

        IEnumerator Shield_Material_Manager(bool Active){
            float _elapsedTime = 0f, TimeToGrow = 0.25f;
            while(_elapsedTime <= TimeToGrow){
                _elapsedTime += Time.deltaTime;
                if(Active == true){
                    VFX_Distorded_mat.SetFloat("_Activness", Mathf.Lerp(0f, 1f, _elapsedTime/TimeToGrow));
                }
                else if(Active == false){
                    VFX_Distorded_mat.SetFloat("_Activness", Mathf.Lerp(1f , 0f, _elapsedTime/TimeToGrow));
                }
                yield return null;
            }
            yield return null;
        }

        #endregion
    
}
