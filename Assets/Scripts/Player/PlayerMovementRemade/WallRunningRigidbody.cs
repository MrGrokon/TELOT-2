﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunningRigidbody : MonoBehaviour
{

    private bool WallOnRight;
    private bool WallOnLeft;
    private Collider WallRunnedOn;
    public bool OnWallRun = false;
    public float WallDistanceDetection = 1f;
    public LayerMask RunnableWallLayer;
    private float _elapsedTime = 0f;
    public GameObject orientation;
    
    [Header("Jump From a wall")]
    [Range(5f, 30f)]
    public float JumpForce = 20f;
    [Range(0.5f, 5f)]
    public float JumpPersistance = 2f;
    Vector3 LastWall_normal = Vector3.zero;
    public GameObject WallOnRun;
    public Vector3 wallForwardRun;
    

    private Rigidbody _rb;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (WallOnRight || WallOnLeft)
        {
            _rb.useGravity = false;
            _rb.velocity = Vector3.zero;
            OnWallRun = true;
            if(WallOnLeft){
                //StartCoroutine(FeedbackManager.Instance.AngularCameraRotation(FeedbackManager.CameraDirection.Left));
                transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,-30);
            }
            else
            {
                //StartCoroutine(FeedbackManager.Instance.AngularCameraRotation(FeedbackManager.CameraDirection.Right));
                transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,30);
            }
                
        }
        else
        {
            //StartCoroutine(FeedbackManager.Instance.ResetCameraAngle());
            _rb.useGravity = true;
            OnWallRun = false;
            transform.eulerAngles = new Vector3(0,transform.eulerAngles.y,0);
        }
        
        _elapsedTime += Time.deltaTime;
        if(Input.GetButtonDown("Jump") && LastWall_normal != Vector3.zero && OnWallRun){
            Debug.Log("Jump From a wall");
            WallOnLeft = false;
            WallOnRight = false;
            
            _rb.AddForce((LastWall_normal * 5 + Vector3.up * 2.2f).normalized * (JumpForce * 2), ForceMode.Impulse);
            //_rb.AddForce(( Vector3.up).normalized * (JumpForce * 2), ForceMode.Impulse);
            if(_rb.velocity.magnitude > 0)
                _rb.AddForce(-_rb.transform.forward * 45, ForceMode.Impulse);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.layer == 9)
        {
            #region Detect & Assign the wall i walk on
            WallOnRight = Physics.Raycast(this.transform.position, this.transform.right, out RaycastHit RightHit, WallDistanceDetection, RunnableWallLayer.value);
            WallOnLeft = Physics.Raycast(this.transform.position, -this.transform.right, out RaycastHit LeftHit, WallDistanceDetection, RunnableWallLayer.value);
            if(WallOnLeft == true){
                WallRunnedOn = LeftHit.collider;
                LastWall_normal = transform.right;
                wallForwardRun = Vector3.ProjectOnPlane(transform.forward, LeftHit.normal);
                /*orientation.transform.rotation = Quaternion.LookRotation(LeftHit.normal, Vector3.up);
                wallForwardRun = -orientation.transform.right;*/
            }

            if(WallOnRight == true){
                WallRunnedOn = RightHit.collider;
                LastWall_normal = -transform.right;
                orientation.transform.rotation = Quaternion.LookRotation(RightHit.normal, Vector3.up);
                wallForwardRun = orientation.transform.right;
            }
            #endregion
            transform.rotation = Quaternion.LookRotation(wallForwardRun, Vector3.up);
            Camera.main.transform.rotation = Quaternion.identity;
            GetComponent<MouseLook>().locked = true;
            GetComponent<MouseLook>().ResetRotation();
            Debug.DrawRay(transform.position, wallForwardRun, Color.red, Mathf.Infinity);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if(other.gameObject.layer == 9)
        {
            WallOnLeft = false;
            WallOnRight = false;
            GetComponent<MouseLook>().ResetCameraAndBody();
        }
    }
    
}
