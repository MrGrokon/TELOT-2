﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public enum EnemyType
    {
        Turret,
        Dummy,
        Prossecutor
    }

    public EnemyType MyType;

    public void SpawnAnEnemy(){
        GameObject _enemy = null;

        switch (MyType)
        {
            case EnemyType.Turret:
            _enemy = ObjectReferencer.Instance.TurretEnemy_prefab;
            break;

            case EnemyType.Dummy:
            _enemy = ObjectReferencer.Instance.DummyEnemy_prefab;
            break;

            case EnemyType.Prossecutor:
            _enemy = ObjectReferencer.Instance.TurretEnemy_prefab;
            break;

            default:
            Debug.Log("Something fucked up");
            break;
        }

        //feedback
        Instantiate(_enemy, this.transform.position, this.transform.rotation);
    }
}
