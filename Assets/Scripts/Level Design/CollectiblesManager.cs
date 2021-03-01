using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblesManager : MonoBehaviour
{
    enum T
    {
        Ammunition,
        Health
    }

    [SerializeField] private T collectibleType;

    [Range(1,3)]
    [SerializeField] private int collectibleLevel;
    // Start is called before the first frame update
    void Update()
    {
        transform.Rotate(Vector3.up * 70f * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            switch (collectibleType)
            {
                case T.Ammunition:
                    other.GetComponent<EnergieStored>().SetEnergieStored(5);
                    break;
                case T.Health:
                    //+20 Player Health;
                    break;
            }
            Destroy(gameObject);
        }
    }
}
