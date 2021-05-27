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

    
    public List<MonsterBehavior> monsters;
    private bool WaveIsActive = false;
    private MusicManager _musicManager;

    private void OnTriggerEnter(Collider _col) {
        if(_col.tag == "Player" && WaveIsActive == false)
        {
            activeRoom = true;
            _musicManager = UI_Feedbacks.Instance.GetComponent<MusicManager>();
            if (_musicManager.musicStarted == false)
            {
                _musicManager.StartMusic();
                _musicManager.musicStarted = true;  
            }
            ProgressionFollow PF = UI_Feedbacks.Instance.GetComponent<ProgressionFollow>();
            PF.UpdateRoomCount(roomNumber, 5);
            
            WaveIsActive = true;
            Debug.Log("Player enter a room");
            if (roomNumber > 0)
            {
                UI_Feedbacks.Instance.GetComponent<RoomLoader>().DesactivateRoom();
            }
            foreach (var _spawn in Spawners_Wave1)
            {
                //monsters.Add(_spawn.SpawnAnEnemy());
                _spawn.SetRoomParent(this);
                _spawn.LaunchSpawnProcedure();
                //do some shit like closing door, etc ...
            }
            foreach (var door in DoorsAnimator)
            {
                door.SetTrigger("Closing");
            }
        }
    }

    public void ManualyAddMonster(MonsterBehavior _monster){
        monsters.Add(_monster);
    }

    private void Update() {
        if (activeRoom)
        {
            _elapsedWaveTime += Time.deltaTime;
            ProgressionFollow PF = UI_Feedbacks.Instance.GetComponent<ProgressionFollow>();
            PF.UpdateEnnemiesRemainingCount(monsters.Count);  
        }
        /*if(WaveIsActive == true && (monsters.Count == 0 || _elapsedWaveTime >= TimeUntilWaveTwo)){
            if(WaveTwoLaunched == false){
                WaveTwoLaunched = true;
                _elapsedWaveTime = Mathf.NegativeInfinity;
                foreach (var _spawn in Spawners_Wave2)
                {
                //monsters.Add(_spawn.SpawnAnEnemy());
                _spawn.SetRoomParent(this);
                _spawn.LaunchSpawnProcedure();
            }
            }
            else{
                Debug.Log("wave ended");
                _musicManager.ChangeSituation(2);
                foreach (var door in DoorsAnimator)
                {
                    if(_elapsedWaveTime < 0f){
                        door.SetTrigger("Opening");
                    }
                }
                activeRoom = false;
                //this.enabled = false;
            }
        }*/

        if(WaveIsActive && (monsters.Count == 0 || _elapsedWaveTime >= TimeUntilWaveTwo) ){
            if(WaveTwoLaunched == false && _elapsedWaveTime >= 5f){
                 WaveTwoLaunched = true;
                //_elapsedWaveTime = Mathf.NegativeInfinity;
                _elapsedWaveTime = 0f;
                foreach (var _spawn in Spawners_Wave2)
                {
                //monsters.Add(_spawn.SpawnAnEnemy());
                _spawn.SetRoomParent(this);
                _spawn.LaunchSpawnProcedure();
                }
            }

            if(WaveTwoLaunched == true && _elapsedWaveTime >= 5f){
                //Debug.Log("wave ended");
                _musicManager.ChangeSituation(2);
                _musicManager.musicStarted = false;
                foreach (var door in DoorsAnimator)
                {
                    door.SetTrigger("Opening");
                }
                UI_Feedbacks.Instance.GetComponent<RoomLoader>().ActivateRoom(roomNumber+1);
                activeRoom = false;
                if (roomNumber == 4)
                {
                    UI_Feedbacks.Instance.GetComponent<GameTime>().StopChrono();
                }
                //this.enabled = false;
            }
        }
    }
}
