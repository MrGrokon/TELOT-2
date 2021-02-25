using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectEnergie : MonoBehaviour
{
    public GameObject PlayerProjectile;

    [Range(0.1f, 1f)]
    public float TimeBetweenShots = 0.3f;
    [Range(10f, 80f)]
    public float ProjectileSpeed = 50f;

    private EnergieStored _Energie;

    private void Awake() {
        _Energie = this.GetComponent<EnergieStored>();
        if(_Energie == null){
            Debug.Log("EnergieStored not defined");
        }
    }

    private void Update() {
        if(Input.GetButtonDown("Fire1")){
            StartCoroutine(ShootProcedure());
        }
    }

    IEnumerator ShootProcedure(){
        Debug.Log("Shoot Procedure strart");

        for (int i = 0; i < _Energie.GetEnergieStored(); i++)
        {
            float _elapsedTime = 0f; // reseting _elapsedtime here create a delay on 1st projectile.
            while(_elapsedTime < TimeBetweenShots){
                _elapsedTime += Time.deltaTime;
                yield return null;
            }
            _Energie.SpendEnergie(1);
            Debug.Log("ShootFired");
            var proj = Instantiate(PlayerProjectile, Camera.main.transform.position + transform.forward * 2f, Camera.main.transform.rotation);
            proj.GetComponent<ProjectilBehavior>().SetSpeed(ProjectileSpeed);
            yield return null;
        }

        Debug.Log("Shoot Procedure end");
        yield return null;
    }
}
