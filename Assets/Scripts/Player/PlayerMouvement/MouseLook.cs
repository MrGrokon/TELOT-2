﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    private PhantomMode _phantomMode;
    [Range(10f, 400f)]
    public float MouseSensitivity = 100f;
    
    [HideInInspector]
    public float Z_Rotation = 0f;

    private Transform PlayerBody;
    private Transform PlayerView;
    private float X_Rotation;
    public bool locked = false;

    private void Awake() {
        Cursor.lockState = CursorLockMode.Locked;

        PlayerBody = this.transform;
        PlayerView = Camera.main.transform;
        X_Rotation = 0f;
        _phantomMode = this.GetComponent<PhantomMode>();
    }

    private void Update()
    {
        //get Mouse Motion from Inputs
        float _mouseX, _mouseY; 

        if(_phantomMode && _phantomMode.UsingPhantom){
             _mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * (Time.deltaTime / _phantomMode.PhantomTimeFlowModifier);
            _mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * (Time.deltaTime / _phantomMode.PhantomTimeFlowModifier);
        }
        else{
            _mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;
            _mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.deltaTime;
        }
        
        X_Rotation -= _mouseY;
        //Vertical Rotation Clamping
        X_Rotation = Mathf.Clamp(X_Rotation, -70f, 70f);

        Z_Rotation += _mouseX;
    

            //Apply rotations
        if (GetComponent<WallRunningRigidbody>() && !GetComponent<WallRunningRigidbody>().OnWallRun)
        {
            PlayerView.localRotation = Quaternion.Euler(X_Rotation, 0f, 0);
            PlayerBody.Rotate(this.transform.up * _mouseX);
            //PlayerBody.eulerAngles = new Vector3(0, PlayerBody.eulerAngles.y, 0);
        }
        else
        {
            PlayerView.localRotation = Quaternion.Euler(X_Rotation, Z_Rotation, 0);
        }


    }

    public void ResetBody()
    {
        PlayerBody.eulerAngles = new Vector3(0, PlayerView.eulerAngles.y,PlayerBody.eulerAngles.z);
        //PlayerView.rotation = Quaternion.identity;
        locked = false;
    }

    public void ResetRotation()
    {
        X_Rotation = 0;
        Z_Rotation = 0;
    }
}
