using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public List<EnemySpawner> Spawners;

    private void OnTriggerEnter(Collider _col) {
        if(_col.tag == "Player"){
            Debug.Log("Player enter a room");
            foreach (var _spawn in Spawners)
            {
                _spawn.SpawnAnEnemy();
                //do some shit like closing door, etc ...
            }
        }
    }
}
