using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomMode : MonoBehaviour
{
    public KeyCode PhantomKey;
    [Range(0f,1f)]
    public float PhantomTimeFlowModifier = 0.5f;
    private bool UsingPhantom = false;

    public float PhantomTime = 5f;
    //public float TimeToLerpToNormal = 1f;

    private void Update() {
        if(Input.GetKeyDown(PhantomKey) && UsingPhantom == false){
            StartCoroutine(PhantomModeLaunch());
        }
    }

    IEnumerator PhantomModeLaunch(){
        UsingPhantom = true;
        float _elapsedTime = 0f;
        Time.timeScale = PhantomTimeFlowModifier;

        while(_elapsedTime < PhantomTime){
            _elapsedTime = Time.deltaTime / PhantomTimeFlowModifier;
            yield return null;
        }
        
        Time.timeScale = 1f;
        UsingPhantom = false;
        yield return null;
    }
}
