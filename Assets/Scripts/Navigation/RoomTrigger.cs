using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{

    public int roomNumber;
    public List<EnemySpawner> Spawners_Wave1;
    public List<EnemySpawner> Spawners_Wave2;

    public float TimeUntilWaveTwo = 35f;
    private float _elapsedWaveTime = 0f;
    private bool WaveTwoLaunched = false;

    public List<Animator> DoorsAnimator;
    public bool activeRoom;

    [HideInInspector]
    public List<MonsterBehavior> monsters;
    private bool WaveIsActive = false;
    public bool hasAnotherWave;
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
            foreach (var _spawn in Spawners_Wave1)
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
            _elapsedWaveTime += Time.deltaTime;
            ProgressionFollow PF = UI_Feedbacks.Instance.GetComponent<ProgressionFollow>();
            PF.UpdateEnnemiesRemainingCount(monsters.Count);  
        }
        
        if(WaveIsActive == true && (monsters.Count == 0 || _elapsedWaveTime >= TimeUntilWaveTwo)){
            if(WaveTwoLaunched == false){
                WaveTwoLaunched = true;
                _elapsedWaveTime = Mathf.NegativeInfinity;
                foreach (var _spawn in Spawners_Wave2)
            {
                monsters.Add(_spawn.SpawnAnEnemy());
            }
            }
            else{
                Debug.Log("wave ended");
                foreach (var door in DoorsAnimator)
                {
                    door.SetTrigger("Opening");
                    _musicManager.ChangeSituation(2); 
                }

                activeRoom = false;
                this.enabled = false;
            }
        }
    }
}
