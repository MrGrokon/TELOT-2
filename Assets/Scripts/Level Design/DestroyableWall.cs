using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableWall : MonoBehaviour
{

    void OnCollisionEnter(Collision _col)
    {
        if(_col.gameObject.tag == "PlayerProjectile"){
            Destroy(this.gameObject);
            Destroy(_col.gameObject);

            //Feedbacks
        }
    }
}