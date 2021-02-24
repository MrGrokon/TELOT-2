using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunningRigidbody : MonoBehaviour
{

    private bool WallOnRight;
    private bool WallOnLeft;
    private Collider WallRunnedOn;
    private bool TriggerEnterWallRun = true;
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
            LastWall_normal = LeftHit.normal;
        }

        if(WallOnRight == true){
            WallRunnedOn = RightHit.collider;
            LastWall_normal = RightHit.normal;
        }
        #endregion

        if (WallOnRight || WallOnLeft)
        {
            _rb.useGravity = false;
            _rb.velocity = Vector3.zero;
            if(WallOnLeft){
                StartCoroutine(FeedbackManager.Instance.AngularCameraRotation(FeedbackManager.CameraDirection.Left));
            }
            else
                StartCoroutine(FeedbackManager.Instance.AngularCameraRotation(FeedbackManager.CameraDirection.Right));
        }
        else
        {
            _rb.useGravity = true;
        }
        
        _elapsedTime += Time.deltaTime;
        if(Input.GetButtonDown("Jump") && LastWall_normal != Vector3.zero){
            Debug.Log("Jump From a wall");
            StartCoroutine(Persitant_Jump((LastWall_normal + Vector3.up).normalized));
        }
    }
    
    #region Coroutines
    IEnumerator Persitant_Jump(Vector3 JumpDirection){
        float _elapsedTime = 0f;

        while(_elapsedTime < JumpPersistance){
            _elapsedTime += Time.deltaTime;
            _rb.AddForce( JumpDirection * JumpForce * Time.deltaTime);
            yield return null;
        }
        yield return null;
    }
    #endregion
}
