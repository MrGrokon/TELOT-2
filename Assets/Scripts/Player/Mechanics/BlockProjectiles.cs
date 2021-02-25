using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockProjectiles : MonoBehaviour
{
    
    [Range(1f, 3f)]
    public float ShieldArea = 1.5f;
    [Range(0.5f, 1.5f)]
    public float ShieldTime = 1f;

    public LayerMask EnemieProjectileLayer;

    private Transform _counter_Pivot;
    private bool IsDebuging = false;
    private EnergieStored _Energie;
    public Text shieldStateText;
    public GameObject shieldDebugProps;

    private void Awake() {
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
            shieldDebugProps.SetActive(true);
            shieldStateText.text = "Shield State : " + "On";
        }
        
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

        IsDebuging = false;
        shieldStateText.text = "Shield State : " + "Off";
        shieldDebugProps.SetActive(false);
            Debug.Log("Finish shield");

        yield return null;
    }

}
