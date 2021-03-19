using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public List<EnemySpawner> Spawners;
    public List<Animator> DoorsAnimator;

    [HideInInspector]
    public List<MonsterBehavior> monsters;
    private bool WaveIsActive = false;

    private void OnTriggerEnter(Collider _col) {
        if(_col.tag == "Player" && WaveIsActive == false){
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
        if(WaveIsActive == true && monsters.Count == 0){
            Debug.Log("wave ended");
            foreach (var door in DoorsAnimator)
            {
                door.SetTrigger("Opening");
            }
        }
    }
}
