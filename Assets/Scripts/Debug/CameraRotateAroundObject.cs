using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotateAroundObject : MonoBehaviour
{
    public KeyCode GoUP = KeyCode.Z;
    public KeyCode GoDown = KeyCode.S;
    public Transform target;//the target object
    public float speedMod = 10.0f;//a speed modifier
    private Vector3 point;//the coord to the point where the camera looks at
   
    void Start () {//Set up things on the start method
        point = target.position;//get target's coords
    }
   
    void Update () {//makes the camera rotate around "point" coords, rotating around its Y axis, 20 degrees per second times the speed modifier
        transform.LookAt(point);//makes the camera look to it
        transform.RotateAround (point,new Vector3(0.0f,1.0f,0.0f),20 * Time.deltaTime * speedMod);
    
        if(Input.GetKey(GoUP)){
            this.transform.Translate(Vector3.up * 10f * Time.deltaTime);
        }
        if(Input.GetKey(GoDown)){
            this.transform.Translate(Vector3.down * 10f * Time.deltaTime);
        }
    }
}
