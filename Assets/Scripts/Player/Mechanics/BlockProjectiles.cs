using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockProjectiles : MonoBehaviour
{
    
    [Range(1f, 3f)]
    public float ShieldArea = 1.5f;
    [Range(0.5f, 1.5f)]
    public float ShieldTime = 1f;

    public LayerMask EnemieProjectileLayer;

    private Transform _counter_Pivot;
    private GameObject _shield_rendr;
    private bool IsDebuging = false;
    private EnergieStored _Energie;

    private void Awake() {
        _shield_rendr = GameObject.Find("Shield_Rendr");
        _shield_rendr.SetActive(false);

        _counter_Pivot = GameObject.Find("Shield_Pivot").transform;
        if(_counter_Pivot == null){
            Debug.Log("CounterPivot not defined");
        }
        _Energie = this.GetComponent<EnergieStored>();
        if(_Energie == null){
            Debug.Log("EnergieStored not defined");
        }
    }

    private void Update() {
        if(Input.GetButtonDown("Fire2")){
            StartCoroutine(ActiveShield());
        }
    }

    private void OnDrawGizmos() {
        if(IsDebuging == true){
            Gizmos.DrawWireSphere(_counter_Pivot.position, ShieldArea);
        }
    }

    IEnumerator ActiveShield(bool _debugState = true){
        if(_debugState){
            Debug.Log("Start shield");
            IsDebuging = true;
        }
        _shield_rendr.SetActive(true);
        
        float _elapsedTime = 0f;

        while(_elapsedTime < ShieldTime){
            _elapsedTime += Time.deltaTime;

            Collider[] _ObjectsBlocked = Physics.OverlapSphere(_counter_Pivot.position, ShieldArea, EnemieProjectileLayer.value);
            if(_ObjectsBlocked.Length > 0){
                foreach (var proj in _ObjectsBlocked)
                {
                    Destroy(proj.gameObject);
                    _Energie.StoreEnergie();
                    
                }
            }
            yield return null;
        }

        _shield_rendr.SetActive(false);
        IsDebuging = false;
            Debug.Log("Finish shield");

        yield return null;
    }

}
