using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockProjectiles : MonoBehaviour
{
    [Range(0.3f, 1.5f)]
    public float TimeToBeActive = 0.75f;
    public float ShieldHitboxRange = 1f;
    public LayerMask ProjectileLayerMask;

    private GameObject _Shield_Rendr;
    private Transform _Shield_Pivot;
    private EnergieStored _Energie;

    #region Unity Functions

        private void Awake() {
            _Shield_Rendr = GameObject.Find("ShieldDebug");
            _Shield_Pivot = GameObject.Find("Shield_Pivot").transform;
            _Shield_Rendr.SetActive(false);
            _Energie = this.GetComponent<EnergieStored>();
            if(_Energie == null){
                Debug.Log("EnergieStored not defined");
            }
        }

        private void Update() {
            if(Input.GetButtonDown("Fire2")){
                StartCoroutine(ActivateShield());
            }
        }

    #endregion

    #region Coroutines
        IEnumerator ActivateShield(){
            float _elapsedTime = 0f;
            _Shield_Rendr.SetActive(true);

            while(_elapsedTime < TimeToBeActive){
                _elapsedTime += Time.deltaTime;
                Collider[] _hits = Physics.OverlapSphere(_Shield_Pivot.position, ShieldHitboxRange, ProjectileLayerMask);
                
                foreach(var Projectiles in _hits){
                    Destroy(Projectiles);
                    _Energie.StoreEnergie();
                }

                yield return null;
            }

            _Shield_Rendr.SetActive(false);
            yield return null;
        }
    #endregion
}
