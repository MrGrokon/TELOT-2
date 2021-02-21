using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullifyProjectiles : MonoBehaviour
{
    //private SomethingToStock energie;

    void OnCollisionEnter(Collision _col)
    {
        if(_col.gameObject.tag == "Projectile"){
            Debug.Log("a projectile is blocked");
            Destroy(_col.gameObject);
            ObjectReferencer.Instance.Avatar_Object.GetComponent<EnergieStored>().StoreEnergie();
        }
    }
}
