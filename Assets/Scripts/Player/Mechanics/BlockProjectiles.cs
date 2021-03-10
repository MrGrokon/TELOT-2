using System.Collections;
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
            if(_Energie == null){
                Debug.Log("EnergieStored not defined");
            }
        }

        private void Update() {
            ActivateShield();
            shieldRemainSlider.value = (TimeToBeActive - _elapsedTime) / TimeToBeActive;
            if(Input.GetButtonDown("Fire2") && !Shielding)
            {
                Shielding = true;
                _elapsedTime = 0;
                StartCoroutine(ShieldSoundManager());
            }
        }

        private void ActivateShield()
        {
            if (Shielding)
            {
                if (_elapsedTime < TimeToBeActive)
                {
                    _Shield_Rendr.SetActive(true);
                    _elapsedTime += 1 * Time.deltaTime;
                    Collider[] _hits = Physics.OverlapSphere(_Shield_Pivot.position, ShieldHitboxRange, ProjectileLayerMask);
                
                    foreach(var Projectiles in _hits)
                    {
                        Destroy(Projectiles);
                        _Energie.StoreEnergie(energieStoredPerShot);
                        FMODUnity.RuntimeManager.PlayOneShot("event:/Shield/ShieldTanking"); 
                    }
                }
                else if(_elapsedTime >= TimeToBeActive) 
                {
                    Shielding = false;
                    _Shield_Rendr.SetActive(false);
                    shieldIdle.stop(STOP_MODE.ALLOWFADEOUT);
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Shield/ShieldOff", transform.position);
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
