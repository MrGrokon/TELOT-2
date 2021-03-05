﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

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

    [Header("Wall Run Feedbacks")] 
    public float interpolationTime;

    public float fovOnWall;
    private float fovNormal;

    private Rigidbody _rb;
    public Vector3 Motion;
    
    [Header("Post Processing Parameters")]
    private ChromaticAberration CA;
    public PostProcessVolume volume;
    [SerializeField] private float chromaticLerpTime;
    private float actualChromaticLerpTimeValue;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        fovNormal = Camera.main.fieldOfView;
        volume.profile.TryGetSettings(out CA);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Motion = transform.forward * v + transform.right * h;
        PostProcessValueManager();
        WallRunFeedbacks();
        if (WallOnRight || WallOnLeft)
        {
            _rb.useGravity = false;
            _rb.velocity = Vector3.zero;
            OnWallRun = true;
            if(WallOnLeft){
                
                transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,-30);
            }
            else
            {
                transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,30);
            }
                
        }
        else
        {
            _rb.useGravity = true;
            OnWallRun = false;
        }
        if(Input.GetButtonDown("Jump") && LastWall_normal != Vector3.zero && OnWallRun)
        {
            
            _rb.AddForce((Vector3.up + Motion*2).normalized * (JumpForce * 2), ForceMode.Impulse);
            GetComponent<PlayerMovementRigidbody>().Motion = Vector3.zero;
            WallOnLeft = false;
            WallOnRight = false;
            StartCoroutine(ReactivateDoubleJump());
        }
        
        _elapsedTime += Time.deltaTime;
        
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
            }

            if(WallOnRight == true){
                WallRunnedOn = RightHit.collider;
                LastWall_normal = -transform.right;
                wallForwardRun = Vector3.ProjectOnPlane(transform.forward, RightHit.normal);
            }
            #endregion
            CA.intensity.value = 0.25f;
            actualChromaticLerpTimeValue = 0;
            transform.rotation = Quaternion.LookRotation(wallForwardRun, Vector3.up);
            Camera.main.transform.rotation = Quaternion.identity;
            GetComponent<MouseLook>().locked = true;
            GetComponent<MouseLook>().ResetRotation();
            GetComponent<PlayerMovementRigidbody>().doubleJump = false;
            Debug.DrawRay(transform.position, wallForwardRun, Color.red, Mathf.Infinity);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if(other.gameObject.layer == 9)
        {
            print("Je sors");
            WallOnLeft = false;
            WallOnRight = false;
            GetComponent<MouseLook>().ResetCameraAndBody();
            CA.intensity.value = 0;
        }
    }


    private void WallRunFeedbacks()
    {
        if (OnWallRun)
        {
            interpolationTime += 2 * Time.deltaTime;
        }
        else if(!OnWallRun)
        {
            interpolationTime -= 2 * Time.deltaTime;
        }
        Camera.main.fieldOfView =
            Mathf.Lerp(fovNormal, fovOnWall, interpolationTime);
        interpolationTime = Mathf.Clamp(interpolationTime, 0, 1);
    }

    private IEnumerator ReactivateDoubleJump()
    {
        yield return new WaitForSeconds(0.2f);
        GetComponent<PlayerMovementRigidbody>().doubleJump = true;
    }
    
    private void PostProcessValueManager()
    {
        if (actualChromaticLerpTimeValue < chromaticLerpTime)
        {
            CA.intensity.value = Mathf.Lerp(CA.intensity.value, 0, actualChromaticLerpTimeValue / chromaticLerpTime);
            actualChromaticLerpTimeValue += 1 * Time.deltaTime;
        }
    }
    
}
