using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualyActivateMine : MonoBehaviour
{
    public KeyCode ActivationKey = KeyCode.Space;
    public float TimeBetweenTwoActivation = 3f;
    
    private TrapBehavior_v2 _mineScript;
    private float _elapsedTime = 0f;
    private bool IsActivated = false;

    void Start()
    {
        _mineScript = this.GetComponentInChildren<TrapBehavior_v2>();
        if(_mineScript == null){
            Debug.Log("TrapBehavior_V2 not found");
        }
    }

    private void Update() {
        _elapsedTime += Time.deltaTime;

        if(Input.GetKeyDown(ActivationKey) && _elapsedTime >= TimeBetweenTwoActivation){
            _elapsedTime = 0f;
            _mineScript.TrapTriggered();
        }
    }
}
