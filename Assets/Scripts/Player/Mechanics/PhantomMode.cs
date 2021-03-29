using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomMode : MonoBehaviour
{
    [Range(0.01f,1f)]
    public float PhantomTimeFlowModifier = 0.5f;
    [HideInInspector]
    public bool UsingPhantom = false;

    [Range(1f, 3f)]
    public float PhantomSpeedMultiplier = 1.8f;
    public float PhantomTime = 5f;

    //change later to animation curve
    public float TimeToLerpToNormal = 1f;

    #region unity function
    private void Update() {
        if((Input.GetButtonDown("Fire3")) && UsingPhantom == false){
            StartCoroutine(PhantomModeLaunch());
        }
    }
    #endregion

    IEnumerator PhantomModeLaunch(){
        Vector3 baseGravity = Physics.gravity;
        UsingPhantom = true;
        float _elapsedTime = 0f;
        Time.timeScale = PhantomTimeFlowModifier;
        Physics.gravity /= PhantomTimeFlowModifier;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        //Tempo during full time of slow mo
        while(_elapsedTime < PhantomTime){
            _elapsedTime += Time.deltaTime / PhantomTimeFlowModifier;
            yield return null;
        }
        
        //Lerp back to normal value
        _elapsedTime = 0f;
        while(_elapsedTime < TimeToLerpToNormal){
            _elapsedTime += Time.deltaTime / PhantomTimeFlowModifier;
            float _value = Mathf.Lerp(PhantomTimeFlowModifier, 1f, _elapsedTime / TimeToLerpToNormal);
            Time.timeScale = _value;
            Physics.gravity = new Vector3(0f, baseGravity.y / _value, 0f);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return null;
        }

        //values back to normals
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        Physics.gravity = baseGravity;
        UsingPhantom = false;
        yield return null;
    }
}
