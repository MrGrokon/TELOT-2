using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerManager : MonoBehaviour
{
    [Header("Trigger Parameters")]
    public LayerMask FloorLayer;

    [Header("Booleans States")]
    #region Booleans that'll be passed to other scripts, handled over there
        public bool OnGround;
    #endregion

    private Transform _floorDetector;

    private void Awake() {
        _floorDetector = this.transform.Find("Floor_Collider");
        if(_floorDetector == null){
            Debug.Log("Critical Error: Floor Collider not defined");
        }
    }

    private void Update() {
        #region On Ground Management
        if(Physics.OverlapSphere(_floorDetector.position, 0.3f, FloorLayer.value).Length > 0)
        {
            //Debug.Log("I walk on something");
            OnGround = true;
        }
        else{
            //Debug.Log("I don't walk on anything");
            OnGround = false;
        }
        #endregion
    }
}   
