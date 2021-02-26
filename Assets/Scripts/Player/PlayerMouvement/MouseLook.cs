using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Range(10f, 400f)]
    public float MouseSensitivity = 100f;
    
    [HideInInspector]
    public float Z_Rotation = 0f;

    private Transform PlayerBody;
    private Transform PlayerView;
    private float X_Rotation;
    private float Y_Rotation;

    private void Awake() {
        Cursor.lockState = CursorLockMode.Locked;

        PlayerBody = this.transform;
        PlayerView = this.GetComponentInChildren<Camera>().transform;
        X_Rotation = 0f;
    }

    private void Update()
    {
        //get Mouse Motion from Inputs
        float _mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
        float _mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;

        X_Rotation -= _mouseY;
        //Vertical Rotation Clamping
        X_Rotation = Mathf.Clamp(X_Rotation, -70f, 70f);
        Y_Rotation += _mouseX;
        Y_Rotation = Mathf.Clamp(Y_Rotation, -90f, 90f);
        
        //Apply rotations
        
        
        if (!GetComponent<WallRunningRigidbody>().OnWallRun)
        {
            PlayerView.localRotation = Quaternion.Euler(X_Rotation, 0f, Z_Rotation);
            PlayerBody.Rotate(this.transform.up * _mouseX);
        }
        else
        {
            PlayerView.localRotation = Quaternion.Euler(X_Rotation, Y_Rotation, 0);
        }
        
    }

    public void ResetCameraAndBody()
    {
        PlayerBody.eulerAngles = new Vector3(PlayerBody.eulerAngles.x, PlayerBody.eulerAngles.y,0);
        PlayerView.localRotation = Quaternion.identity;
    }
}
