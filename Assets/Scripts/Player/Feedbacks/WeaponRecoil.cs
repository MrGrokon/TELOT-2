using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    public Transform recoilMod;
    public GameObject weapon;
    public float maxRecoil_z = -20;
    public float recoilSpeed = 10;
    //public float recoil = 0.0f;
    public AnimationCurve WeaponVerticalRecoil_Curve;

     
    /*void Update() {

        recoiling();
    }
     
    void recoiling() {
        if(recoil > 0)
        {
            var maxRecoil = Quaternion.Euler (0, 0, maxRecoil_z);
            // Dampen towards the target rotation
            recoilMod.rotation = Quaternion.Slerp(recoilMod.rotation, maxRecoil, Time.deltaTime * recoilSpeed);
            float Z = recoilMod.localEulerAngles.z;
            weapon.transform.localEulerAngles = weapon.transform.localEulerAngles + new Vector3(0, 0, Z);
            recoil -= Time.deltaTime;
        }
        else
        {
            recoil = 0;
            var minRecoil = Quaternion.Euler (0, 0, 0);
            // Dampen towards the target rotation
            recoilMod.rotation = Quaternion.Slerp(recoilMod.rotation, minRecoil,Time.deltaTime * recoilSpeed / 2);
            float Z = recoilMod.localEulerAngles.z;
            weapon.transform.localEulerAngles = weapon.transform.localEulerAngles + new Vector3(0, 0, Z);
            this.enabled = false;
        }
    }*/

    public IEnumerator RecoilMethod(float _RecoilTime){
        float _elapsedRecoilTime = 0f;
        float VerticalOffset = 0f;
        Debug.Log("Recoil method start");
        Vector3 BaseEulerAngle = weapon.transform.localEulerAngles;

        while (_elapsedRecoilTime <= _RecoilTime)
        {
             _elapsedRecoilTime += Time.deltaTime;
            VerticalOffset = WeaponVerticalRecoil_Curve.Evaluate(_elapsedRecoilTime / _RecoilTime) * maxRecoil_z;
            weapon.transform.localEulerAngles = BaseEulerAngle + new Vector3(0, 0, VerticalOffset);
            yield return null;
        }
        Debug.Log("Recoil method ended");
        yield return null;
    }

}
