using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblesManager : MonoBehaviour
{
    enum T
    {
        Ammunition,
        Health,
        Phantom
    }

    [SerializeField] private T collectibleType;

    [Range(1,3)]
    [SerializeField] private int collectibleLevel;

    [Tooltip("Correspond à la quantité donnée par niveau tel que le nombre à l'index 0 correspond au montant donnée pour un collectible de niveau 1")]
    [SerializeField] private int[] ammoAmountByLevel;
    [SerializeField] private int[] lifeAmountByLevel;
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
                    if(other.GetComponent<EnergieStored>().GetEnergieAmountStocked() < other.GetComponent<EnergieStored>().MaxEnergieStorable)
                        other.GetComponent<EnergieStored>().AddEnergie(ammoAmountByLevel[collectibleLevel-1]);
                        Destroy(this.gameObject);
                    break;
                case T.Health:
                    if(other.GetComponent<PlayerLife>().getLifePoint() < other.GetComponent<PlayerLife>().startingLifePoint)
                        other.GetComponent<PlayerLife>().AddLifePoint(lifeAmountByLevel[collectibleLevel-1]);
                        Destroy(this.gameObject);
                    break;
                case T.Phantom:
                    if(other.GetComponent<PhantomMode>().HasPhantomAmmoStored == false){
                        other.GetComponent<PhantomMode>().ReloadPhantomMode();
                        Destroy(this.gameObject);
                    }
                    break;
            }
        }
    }
}
