using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Range(1, 600)]
    public int RoundPerMinute = 60;

    [Range(1, 100)]
    public int DamagePerProjectiles = 10;

    [Range(1f, 150f)]
    public float ProjectileSpeed = 10f;

    public GameObject Projectile;

    public virtual void MainFire(){
        //RoundPerMinute/60f;
        
    }

    public virtual void AlternativeFire(){
        

    }
}
