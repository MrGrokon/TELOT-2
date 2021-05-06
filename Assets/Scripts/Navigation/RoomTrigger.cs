using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public int roomNumber;
    public List<EnemySpawner> Spawners;
    public List<Animator> DoorsAnimator;
    public bool activeRoom;

    [HideInInspector]
    public List<MonsterBehavior> monsters;
    private bool WaveIsActive = false;
    private MusicManager _musicManager;

    private void OnTriggerEnter(Collider _col) {
        if(_col.tag == "Player" && WaveIsActive == false)
        {
            activeRoom = true;
            _musicManager = UI_Feedbacks.Instance.GetComponent<MusicManager>();
            _musicManager.StartMusic();

            ProgressionFollow PF = UI_Feedbacks.Instance.GetComponent<ProgressionFollow>();
            PF.UpdateRoomCount(roomNumber, 5);
            
            WaveIsActive = true;
            Debug.Log("Player enter a room");
            foreach (var _spawn in Spawners)
            {
                monsters.Add(_spawn.SpawnAnEnemy());
                //do some shit like closing door, etc ...
            }
            foreach (var door in DoorsAnimator)
            {
                door.SetTrigger("Closing");
            }
        }
    }

    private void Update() {
        if (activeRoom)
        {
            ProgressionFollow PF = UI_Feedbacks.Instance.GetComponent<ProgressionFollow>();
            PF.UpdateEnnemiesRemainingCount(monsters.Count);  
        }
        
        if(WaveIsActive == true && monsters.Count == 0){
            Debug.Log("wave ended");
            _musicManager.ChangeSituation(2);
            foreach (var door in DoorsAnimator)
            {
                door.SetTrigger("Opening");
            }

            activeRoom = false;
            this.enabled = false;
        }
    }
}
