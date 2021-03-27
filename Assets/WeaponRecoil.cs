﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    public Transform recoilMod;
    public GameObject weapon;
    public float maxRecoil_x = -20;
    public float recoilSpeed = 10;
    public float recoil = 0.0f;
     
    void Update() {
        if(Input.GetMouseButtonDown(0) && GetComponent<EnergieStored>().HasEnergieStored())
        {
            //every time you fire a bullet, add to the recoil.. of course you can probably set a max recoil etc..
            recoil += 0.1f;
        }
     
        recoiling();
    }
     
    void recoiling() {
        if(recoil > 0)
        {
            var maxRecoil = Quaternion.Euler (maxRecoil_x, 0, 0);
            // Dampen towards the target rotation
            recoilMod.rotation = Quaternion.Slerp(recoilMod.rotation, maxRecoil, Time.deltaTime * recoilSpeed);
            float X = recoilMod.localEulerAngles.x;
            weapon.transform.localEulerAngles = weapon.transform.localEulerAngles + new Vector3(X, 0, 0);
            recoil -= Time.deltaTime;
        }
        else
        {
            recoil = 0;
            var minRecoil = Quaternion.Euler (0, 0, 0);
            // Dampen towards the target rotation
            recoilMod.rotation = Quaternion.Slerp(recoilMod.rotation, minRecoil,Time.deltaTime * recoilSpeed / 2);
            float X = recoilMod.localEulerAngles.x;
            weapon.transform.localEulerAngles = weapon.transform.localEulerAngles + new Vector3(X, 0, 0);
        }
    }

}
