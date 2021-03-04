using System;
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

    [Header("Wall Run Feedbacks")] 
    public float interpolationTime;

    public float fovOnWall;
    private float fovNormal;

    private Rigidbody _rb;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        fovNormal = Camera.main.fieldOfView;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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
            //StartCoroutine(FeedbackManager.Instance.ResetCameraAngle());
            _rb.useGravity = true;
            OnWallRun = false;
            transform.eulerAngles = new Vector3(0,transform.eulerAngles.y,0);
        }
        if(Input.GetButtonDown("Jump") && LastWall_normal != Vector3.zero && OnWallRun){
            _rb.AddForce((Vector3.up * 4f + Camera.main.transform.forward * 5.3f).normalized * (JumpForce * 2), ForceMode.Impulse);
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
                //StartCoroutine(FeedbackManager.Instance.AngularCameraRotation(FeedbackManager.CameraDirection.Left));
                /*orientation.transform.rotation = Quaternion.LookRotation(LeftHit.normal, Vector3.up);
                wallForwardRun = -orientation.transform.right;*/
            }

            if(WallOnRight == true){
                WallRunnedOn = RightHit.collider;
                LastWall_normal = -transform.right;
                orientation.transform.rotation = Quaternion.LookRotation(RightHit.normal, Vector3.up);
                wallForwardRun = orientation.transform.right;
                //StartCoroutine(FeedbackManager.Instance.AngularCameraRotation(FeedbackManager.CameraDirection.Right));
            }
            #endregion
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
            WallOnLeft = false;
            WallOnRight = false;
            GetComponent<MouseLook>().ResetCameraAndBody();
        }
    }


    private void WallRunFeedbacks()
    {
        if (OnWallRun && GetComponent<PlayerMovementRigidbody>().Motion.magnitude > 0)
        {
            interpolationTime += 2 * Time.deltaTime;
        }
        else if(GetComponent<PlayerMovementRigidbody>().Motion.magnitude == 0 || !OnWallRun)
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
    
}
