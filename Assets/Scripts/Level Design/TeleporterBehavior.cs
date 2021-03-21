using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterBehavior : MonoBehaviour
{
    [Range(1f, 20f)]
    public float TimeForTeleporterToReload = 5f;

    private TeleporterData MyDatas;
    private bool IsValide = true, IsActive = true;
    private float _elapsedTime = 0f;


    private void Awake() {
        MyDatas = this.transform.parent.GetComponent<TeleporterData>();
    }

    private void Update() {
        if(IsActive == false){
            _elapsedTime += Time.deltaTime;
            if(_elapsedTime >= TimeForTeleporterToReload){
                IsActive = true;
            }
        }
    }


    private void OnTriggerEnter(Collider _col)
    {
        if(_col.gameObject.tag == "Player" && IsValide == true && IsActive == true){
            _col.transform.position = MyDatas.OtherSide.GetTeleporterPoint().position;
            _col.transform.rotation = MyDatas.OtherSide.GetTeleporterPoint().rotation;

            StartCooldown();
            MyDatas.OtherSide.GetComponentInChildren<TeleporterBehavior>().StartCooldown();
        }
    }

    public void StartCooldown(){
        IsActive = false;
        _elapsedTime = 0f;
    }
}
