using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandlingSystem : MonoBehaviour
{
    public Weapon WeaponUsed;

    private float _TimeBetweenShoots = 0f;
    //UI

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Fire1")){
            _TimeBetweenShoots += Time.deltaTime;
            if(_TimeBetweenShoots >= 1/(WeaponUsed.RoundPerMinute/60f)){
                _TimeBetweenShoots = 0f;
                WeaponUsed.MainFire();
            }
        }
        
        if(Input.GetButton("Fire2")){
            WeaponUsed.AlternativeFire();
        }
    }
}
