using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBehavior_v2 : MonoBehaviour
{
    [Header("Init by hand")]
    public GameObject Projectile_Prefab;

    [Header("Trap Parameters")]
    public bool DetectMonster = false;
    public float TimeForTrapToBeActive = 1.5f;
    public int NumberBurst = 10, NumberOfPalletPerBurst = 5;
    public float TrapPalletLifeTime = 2f;

    private Animator TrapAnimator;
    private Transform _shootPoint;
    private ParticleSystem Halo_PS;

    private void Awake() {
        TrapAnimator = this.transform.parent.GetComponent<Animator>();
        _shootPoint = this.transform.parent.GetChild(2).transform;
        Halo_PS = this.GetComponentInChildren<ParticleSystem>();
    }

    private void OnTriggerEnter(Collider _col) {
        if(DetectMonster){
            if(_col.CompareTag("Player") || _col.CompareTag("Ennemy")){
                TrapTriggered();
            }
        }
        else{
            if(_col.CompareTag("Player"))
            {
               TrapTriggered();
            }
        }
    }  

    private void TrapTriggered(){
        TrapAnimator.SetTrigger("TrapTriggered");
    }

    IEnumerator TrapShootingProcedure(){
        float _elapsedTime = 0f;
        int palletIndex = 1;

        while(_elapsedTime <= TimeForTrapToBeActive){
            _elapsedTime += Time.deltaTime;

            if(_elapsedTime > TimeForTrapToBeActive/NumberBurst * palletIndex){
                palletIndex++;
                ShootPallet_Randomly();
                Debug.Log("Burst fired");
            }
            yield return null;
        }
        TrapAnimator.SetTrigger("TrapStopFiring");
        yield return null;
    }

    private void ShootPallet_Randomly(){
        for (int i = 0; i < NumberOfPalletPerBurst; i++)
        {
            Quaternion _spread = Quaternion.Euler(new Vector3(Random.Range(-45f, 45f), Random.Range(0f, 360f), 0f));
            Vector3 RandomVerticalOffset = new Vector3(0f,Random.Range(-1,1),0f);
            EnemieProjectileBehavior _proj = Instantiate(Projectile_Prefab, _shootPoint.position + RandomVerticalOffset, _spread).GetComponent<EnemieProjectileBehavior>();
            _proj.SetSpeed(60f);
            _proj.OverideLifeTime(TrapPalletLifeTime);

        }
    }

    #region Animator Events
        public void StartShooting(){
            StartCoroutine(TrapShootingProcedure());
        }

        public void Stop_PS(){
            Halo_PS.Stop();
        }

        public void Start_PS(){
            Halo_PS.Play();
        }
    #endregion
}
