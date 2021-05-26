using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemySpawner : MonoBehaviour
{
    private bool _hasSpawn = false;
    private RoomTrigger _parentRoom;
    private VisualEffect _spawn_VFX;

    private GameObject _enemyToSpawn;

    private float SpawnDelay = 3f;

    public enum EnemyType
    {
        Turret,
        Dummy,
        Prossecutor,
        Sniper
    }

    public EnemyType MyType;

    public void LaunchSpawnProcedure(){
        //GameObject _enemy = null;
        if(_hasSpawn == false){
            _hasSpawn = true;
            _spawn_VFX.SendEvent("SpawnEnemy");
            StartCoroutine(DelayedSpawn());

            switch (MyType)
            {
                case EnemyType.Turret:
                _enemyToSpawn = ObjectReferencer.Instance.TurretEnemy_prefab;
                break;

                case EnemyType.Dummy:
                _enemyToSpawn = ObjectReferencer.Instance.DummyEnemy_prefab;
                break;

                case EnemyType.Prossecutor:
                _enemyToSpawn = ObjectReferencer.Instance.ProsecutorEnemy_prefab;
                break;

                case EnemyType.Sniper:
                _enemyToSpawn = ObjectReferencer.Instance.SniperEnemy_prefab;
                break;

                default:
                Debug.Log("Something fucked up");
                break;
            }
            //feedback
            //return Instantiate(_enemy, this.transform.position, this.transform.rotation).GetComponent<MonsterBehavior>();
        }
    }

    public void SetRoomParent(RoomTrigger _room){
        _parentRoom = _room;
    }
    private void Start()
    {
        _spawn_VFX = this.GetComponent<VisualEffect>();
    }

    IEnumerator DelayedSpawn(){
        float _elapsedTime = 0f;
        
        while(_elapsedTime < SpawnDelay){
            _elapsedTime += Time.deltaTime;
            yield return null;
        }
        _spawn_VFX.SendEvent("StopLoading");
        _parentRoom.ManualyAddMonster(Instantiate(_enemyToSpawn, this.transform.position, this.transform.rotation).GetComponent<MonsterBehavior>());
        print("Spawning");
        yield return null;
    }
}
