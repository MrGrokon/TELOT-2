using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blaster : Weapon
{
    public override void MainFire()
    {
        base.MainFire();
        Instantiate(Projectile, Camera.main.transform.position, Camera.main.transform.rotation).GetComponent<ProjectilBehavior>().SetSpeed(ProjectileSpeed);
        Debug.Log("Shotgun main fire");
    }

    public override void AlternativeFire()
    {
        base.AlternativeFire();
        Debug.Log("Shotgun alternative fire");
    }
}