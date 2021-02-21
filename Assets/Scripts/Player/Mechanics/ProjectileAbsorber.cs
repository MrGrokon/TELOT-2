using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAbsorber : MonoBehaviour
{
    public GameObject Shield;
    [Range(0.5f, 5f)]
    public float TimeForShieldToBeActive = 1.5f;

    #region Unity Functions
        private void Awake() {
            Shield.SetActive(false);
        }

        private void Update() {
            if(Input.GetButtonDown("Fire2")){
                StartCoroutine(ActiveShield());
            }
        }
    #endregion

    IEnumerator ActiveShield(){
        float _elapsedTime = 0f;

        Shield.SetActive(true);
        while(_elapsedTime < TimeForShieldToBeActive){
            _elapsedTime += Time.deltaTime;
            yield return null;
        }

        Shield.SetActive(false);
        yield return null;
    }
}
