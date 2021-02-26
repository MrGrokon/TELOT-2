using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerManager : MonoBehaviour
{
    [Header("Ground Parameters")]
    public LayerMask FloorLayer;

    [Header("WallRun Parameters")]
    [Range(0.1f, 3f)] [SerializeField]
    private float WallDistanceDetection = 1f;
    public LayerMask RunnableWallLayer;
    [HideInInspector]
    public Vector3 LastWall_Normal;

    [HideInInspector]
    public bool WallOnRight = false, WallOnLeft = false;
    

    [Header("Booleans States")]
    #region Booleans that'll be passed to other scripts, handled over there
        public bool OnGround;
        public bool IsOnWall;
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
            OnGround = true;
        }
        else{
            OnGround = false;
        }
        #endregion

        #region Detect & Assign the wall i walk on
        if(OnGround == false){
            WallOnRight = Physics.Raycast(this.transform.position, this.transform.right, out RaycastHit RightHit, WallDistanceDetection, RunnableWallLayer.value);
            WallOnLeft = Physics.Raycast(this.transform.position, -this.transform.right, out RaycastHit LeftHit, WallDistanceDetection, RunnableWallLayer.value);
            
            if(WallOnLeft == true){
                //Debug.Log("wall on left");
                LastWall_Normal = LeftHit.normal;
                IsOnWall = true;
            }

            else if(WallOnRight == true){
                //Debug.Log("wall on right");
                LastWall_Normal = RightHit.normal;
                IsOnWall = true;
            }

            else{
                //Debug.Log("no wall to walk on");
                IsOnWall = false;
            }
        }
        #endregion
    }
}   
