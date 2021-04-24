using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class WallRunningRigidbody : MonoBehaviour
{
    [Header("Basic Wallrun Parameters")]
    [Range(1f, 5f)]
    public float WallDistanceDetection = 1f;
    public LayerMask RunnableWallLayer;
    private float _elapsedTime = 0f;

    [HideInInspector]
    public bool OnWallRun = false;
    [HideInInspector]
    public bool canWallRun = true;
    private bool WallOnRight;
    private bool WallOnLeft;
    private Collider WallRunnedOn;

    [Header("Jump From a wall")]
    [Range(5f, 30f)]
    public float JumpForce = 20f;
    [Range(0.5f, 5f)]
    public float JumpPersistance = 2f;
    Vector3 LastWall_normal = Vector3.zero;
    [HideInInspector]
    public Vector3 wallForwardRun;

    [Header("Wall Run Feedbacks")] 
    private float interpolationTime;

    private float lastZrotationByWallrun;
    [SerializeField] private float interpolationSpeed;
    FMOD.Studio.EventInstance playerMoveWallRun;

    public float fovOnWall;
    private float fovNormal;

    private Rigidbody _rb;
    private Vector3 Motion;

    [Header("Wallrun Gravity")]
    public AnimationCurve GravityOverWallrunTime_curve;
    public float TimeToDetachFromWall = 3f;
    [Range(-40, -5)]
    public float gravityOnWall;
    public float lastSpacePress;
    private bool wallRunAtState;
    private Vector3 baseGravity = Physics.gravity;
    
    [Header("Post Processing Parameters")]
    private ChromaticAberration CA;
    public PostProcessVolume volume;
    [SerializeField] private float chromaticLerpTime;
    private float actualChromaticLerpTimeValue;
    public Text wallRunDebug;
    [Tooltip("Temps avant que le joueur ne soit considérer comme hors du mur quand il l'a quitté")]
    public float delayOffWall;

    private bool canOffWall;
    
    
   

    // Start is called before the first frame update
    void Start()
    {
        /*playerMoveWallRun = FMODUnity.RuntimeManager.CreateInstance("event:/Movement/WallRunMovement");
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(playerMoveWallRun, transform, GetComponent<Rigidbody>());*/
        _rb = GetComponent<Rigidbody>();
        fovNormal = Camera.main.fieldOfView;
        volume.profile.TryGetSettings(out CA);
    }

    void Update()
    {
        //wallRunDebug.text = "Last space input :" + lastSpacePress + "\n" + "  wall run state at this time : " + wallRunAtState + "\n" + "Global wall run state :" + OnWallRun;
        lastSpacePress += 1 * Time.deltaTime;
        if (Input.GetButtonDown("Jump"))
        {
            wallRunAtState = OnWallRun;
            lastSpacePress = 0;
        }

        if(Input.GetButtonDown("Jump") && OnWallRun)
        {
            _rb.AddForce((Vector3.up + LastWall_normal * 2 + transform.forward).normalized * (JumpForce * 2.5f), ForceMode.Impulse);
            GetComponent<PlayerMovementRigidbody>().Motion = Vector3.zero;
            WallOnLeft = false;
            WallOnRight = false;
            canWallRun = false;
            StartCoroutine(ReactivateDoubleJump());
            GetComponentInChildren<MouseLook>().ResetBody();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Motion = transform.forward * v + transform.right * h;
        PostProcessValueManager();
        WallRunFeedbacks();
        if (canWallRun)
        {
            if (WallOnRight || WallOnLeft)
            {
                Physics.gravity = new Vector3(0,gravityOnWall,0);
                StartCoroutine(LowGravityDrag());
                OnWallRun = true;
                if (WallOnLeft)
                {
                    float CameraZ = Mathf.Lerp(0, -30, interpolationTime);
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, CameraZ);
                    lastZrotationByWallrun = -30;
                }
                else
                {
                    float CameraZ = Mathf.Lerp(0, 30, interpolationTime);
                    transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, CameraZ);
                    lastZrotationByWallrun = 30;
                }

            }
            else
            {
                StartCoroutine(ResetGravity());
                OnWallRun = false;
            }
        }
        else
        {
            StartCoroutine(ResetGravity());
            OnWallRun = false;
        }
        
        _elapsedTime += Time.deltaTime;
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.layer == 9 && canWallRun)
        {
            canOffWall = false;
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
                GetComponent<MouseLook>().locked = true;
                GetComponent<PlayerMovementRigidbody>().doubleJump = true;
                GetComponentInChildren<MouseLook>().ResetBody();
                GetComponentInChildren<MouseLook>().Z_Rotation = 0;

                Debug.DrawRay(transform.position, wallForwardRun, Color.red, Mathf.Infinity);
                playerMoveWallRun.start();
                StartCoroutine(OffWallDelay());
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if(other.gameObject.layer == 9 && canOffWall)
        {
           StartCoroutine(wallRunDelayOff());
        }
    }
    


    private void WallRunFeedbacks()
    {
        if (OnWallRun)
        {
            interpolationTime += interpolationSpeed * Time.deltaTime;
        }
        else if(!OnWallRun)
        {
            interpolationTime -= interpolationSpeed * Time.deltaTime;
            float Z = Mathf.Lerp(0,lastZrotationByWallrun,  interpolationTime);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Z);
        }
        Camera.main.fieldOfView =
            Mathf.Lerp(fovNormal, fovOnWall, interpolationTime);
        GameObject.Find("DrawAlwaysCamera").GetComponent<Camera>().fieldOfView = Mathf.Lerp(fovNormal, fovOnWall, interpolationTime);
        interpolationTime = Mathf.Clamp(interpolationTime, 0, 1f);
        
        
    }

    private IEnumerator ReactivateDoubleJump()
    {
        yield return new WaitForSeconds(0.2f);
        GetComponent<PlayerMovementRigidbody>().doubleJump = true;
        canWallRun = true;
    }

    private IEnumerator OffWallDelay()
    {
        yield return new WaitForSeconds(0.5f);
        canOffWall = true;
    }
    
    private void PostProcessValueManager()
    {
        if (actualChromaticLerpTimeValue < chromaticLerpTime)
        {
            CA.intensity.value = Mathf.Lerp(CA.intensity.value, 0, actualChromaticLerpTimeValue / chromaticLerpTime);
            actualChromaticLerpTimeValue += 1 * Time.deltaTime;
        }
    }

    private IEnumerator wallRunDelayOff()
    {
        yield return new WaitForSeconds(delayOffWall);
        WallOnLeft = false;
        WallOnRight = false;
        CA.intensity.value = 0;
        playerMoveWallRun.stop(STOP_MODE.ALLOWFADEOUT);
    }

    #region Custom Gravity during wallrun
        IEnumerator LowGravityDrag(){
            float _elapsedTime = 0f;
            while(_elapsedTime < TimeToDetachFromWall){
                _elapsedTime += Time.deltaTime;
                Physics.gravity = new Vector3(0f, GravityOverWallrunTime_curve.Evaluate(_elapsedTime /TimeToDetachFromWall) * baseGravity.y , 0f);
                yield return null;
            }
            yield return null;
            //Physics.gravity 
        }

        IEnumerator ResetGravity(){
            StopCoroutine(LowGravityDrag());
            Physics.gravity = baseGravity;
            yield return null;
        }
    #endregion
    
    

}
