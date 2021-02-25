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
    
    [Header("Jump From a wall")]
    [Range(5f, 30f)]
    public float JumpForce = 20f;
    [Range(0.5f, 5f)]
    public float JumpPersistance = 2f;
    

    private Rigidbody _rb;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        Vector3 LastWall_normal = Vector3.zero;
        

        #region Detect & Assign the wall i walk on
        WallOnRight = Physics.Raycast(this.transform.position, this.transform.right, out RaycastHit RightHit, WallDistanceDetection, RunnableWallLayer.value);
        WallOnLeft = Physics.Raycast(this.transform.position, -this.transform.right, out RaycastHit LeftHit, WallDistanceDetection, RunnableWallLayer.value);
            
        if(WallOnLeft == true){
            WallRunnedOn = LeftHit.collider;
            LastWall_normal = transform.right;
        }

        if(WallOnRight == true){
            WallRunnedOn = RightHit.collider;
            LastWall_normal = -transform.right;
        }
        #endregion
        Debug.DrawRay(transform.position, (LastWall_normal + Vector3.up).normalized * 3, Color.red);
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
        if(Input.GetButtonDown("Jump") && LastWall_normal != Vector3.zero){
            Debug.Log("Jump From a wall");
            _rb.AddForce((LastWall_normal + Vector3.up).normalized * (JumpForce * 2), ForceMode.Impulse);
        }
    }
    
    #region Coroutines
    IEnumerator Persitant_Jump(Vector3 JumpDirection){
        float _elapsedTime = 0f;

        while(_elapsedTime < JumpPersistance){
            _elapsedTime += Time.deltaTime;
            
            yield return null;
        }
        yield return null;
    }
    #endregion
}
