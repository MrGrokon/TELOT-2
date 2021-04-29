using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerMovementRigidbody : MonoBehaviour
{
    private Rigidbody _rb;
    private PhantomMode _phamtomMode;

    [SerializeField] private float speed;

    [SerializeField] private float dashCD;

    private float actualDashCD;

    [SerializeField] private float dashForce;
    [SerializeField] private float jumpForce; //Pas le jeu à la con hein
    [SerializeField] private float airControl;
    public Vector3 Motion;
    public bool onGround = true;
    [Header("Jump Improve Parameters")]
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float lowJumpMultiplier;
    public bool doubleJump = true;
    [Tooltip("Multiplicateur de la force du double saut")]
    [SerializeField] private float dJumpFactor = 1;
    
    [Header("Post Processing Parameters")]
    private ChromaticAberration CA;
    public PostProcessVolume volume;
    [SerializeField] private float chromaticLerpTime;
    private float actualChromaticLerpTimeValue;

    [Header("Particle System")] 
    private List<ParticleSystem> DashParticles;

    [Header("Sound Effect")]
    [SerializeField] private float stepInterval;

    private float actualStepInterval;

    [Header("Head Bobbing Parameters")]
    public GameObject BobbingObject;
    //[Range(0.5f, 5f)]
    public float HeadBobbingTime_Multiplier = 1f;
    [Range(0.001f, 0.1f)]
    public float HeadBobbing_MaxOffset = 0.3f;
    private float BobbingTime = 0f;
    private Vector3 BaseCameraPosition;

    public enum DashDirection{Forward, Left, Right, Backward};

    #region Unity Functions
    void Start()
    {
        #region dash particles
            DashParticles = new List<ParticleSystem>();
            DashParticles.Add(ObjectReferencer.Instance.DashParticule_Container.GetChild(0).GetComponent<ParticleSystem>());
            DashParticles.Add(ObjectReferencer.Instance.DashParticule_Container.GetChild(1).GetComponent<ParticleSystem>());
            DashParticles.Add(ObjectReferencer.Instance.DashParticule_Container.GetChild(2).GetComponent<ParticleSystem>());
            DashParticles.Add(ObjectReferencer.Instance.DashParticule_Container.GetChild(3).GetComponent<ParticleSystem>());
        #endregion

        BaseCameraPosition = BobbingObject.transform.localPosition;
        _rb = GetComponent<Rigidbody>();
        _phamtomMode = this.GetComponent<PhantomMode>();
        volume.profile.TryGetSettings(out CA);
        doubleJump = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }

    private void Update()
    {
        if(Input.GetButtonDown("Jump"))
            Jump();
        BetterJump();
        
        actualDashCD -= 1 * Time.deltaTime;
        //actualDashCD /dashCD * 100f;

        DetectGround();
        PostProcessValueManager();
        actualStepInterval -= 1 * Time.deltaTime;
        
    }

    private void OnCollisionEnter(Collision other)
    {
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.layer == 8)
        {
            StartCoroutine(DelayOffGround());
        }
    }
    #endregion

    private void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        if (onGround || GetComponent<WallRunningRigidbody>().OnWallRun)
        {
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                PlayFootstepSound();
                if (GetComponent<WallRunningRigidbody>().OnWallRun)
                {
                    Motion = (v * GetComponent<WallRunningRigidbody>().wallForwardRun);
                }
                else if(onGround)
                {
                    Motion = (h * transform.right + v * transform.forward).normalized;

                    //Head bobbing procedure
                    BobbingTime += Time.deltaTime;
                    Vector3 Headbob_Offset = new Vector3(Mathf.Sin(BobbingTime * HeadBobbingTime_Multiplier /2) * HeadBobbing_MaxOffset, Mathf.Sin(BobbingTime * HeadBobbingTime_Multiplier) * HeadBobbing_MaxOffset, 0f);
                    BobbingObject.transform.localPosition = BaseCameraPosition + Headbob_Offset;
                }
            }
            else
                Motion = Vector3.zero;
        }
        else
        {

            Motion = ((h * transform.right + v * transform.forward).normalized * airControl);
        }
        
        if(_phamtomMode.UsingPhantom){
            _rb.position += Motion * (speed * _phamtomMode.PhantomSpeedMultiplier) * (Time.deltaTime / _phamtomMode.PhantomTimeFlowModifier);
        }
        else{
            _rb.position += Motion * speed * Time.deltaTime;
        }
        

        if (Input.GetButton("Dash"))
        {
            Dash(Motion);
        }
    }

    private void Dash(Vector3 motion)
    {
        if (actualDashCD <= 0)
        {
            if(motion == this.transform.forward){
                //Debug.Log("dash: " + motion);
                CallDirectionalDashParticle(DashDirection.Forward);
            }
            else if(motion == this.transform.right){
                CallDirectionalDashParticle(DashDirection.Right);
            }
            else if(motion == -this.transform.right){
                CallDirectionalDashParticle(DashDirection.Left);
            }
            else if(motion == -this.transform.forward){
                CallDirectionalDashParticle(DashDirection.Backward);
            }

            _rb.AddForce(motion.normalized * dashForce, ForceMode.Impulse);
            actualDashCD = dashCD;
            CA.intensity.value = 1f;
            actualChromaticLerpTimeValue = 0;
            //DashParticles.Play();
            FMODUnity.RuntimeManager.PlayOneShot("event:/Movement/Dash"); 
        }
    }

    private void Jump()
    {
        if (onGround)
        {
            //_rb.AddForce((transform.up * 2 + Motion * 2.5f).normalized * jumpForce, ForceMode.Impulse);
            StartCoroutine(DelayedJump(UI_Feedbacks.Instance.OffsetTime));
            UI_Feedbacks.Instance.CallFeedback(UI_Feedbacks.FeedbackType.Jump);
            //onGround = false;
            FMODUnity.RuntimeManager.PlayOneShot("event:/Movement/Jump", transform.position);
        }
        else if (doubleJump && !GetComponent<WallRunningRigidbody>().OnWallRun)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Movement/Jump", transform.position);
            _rb.AddForce((transform.up + Motion).normalized * jumpForce * dJumpFactor, ForceMode.Impulse);
            doubleJump = false;
        }
    }

    private void BetterJump()
    {
        if (_rb.velocity.y < 0)
        {
            _rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (_rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            _rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private void DetectGround()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 2f))
        {
            if (hit.transform.gameObject.layer == 8)
            {
                if (onGround == false && GetComponent<Rigidbody>().velocity.y < 0)
                {
                    UI_Feedbacks.Instance.CallFeedback(UI_Feedbacks.FeedbackType.Jump);
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Movement/LandOnGround", transform.position);
                    print("Atté");
                }
                onGround = true;
                doubleJump = true;
            }
        }
    }

    private void PlayFootstepSound()
    {
        if (actualStepInterval <= 0)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Movement/Footstep", transform.position);
            if(GetComponent<WallRunningRigidbody>().OnWallRun)
                actualStepInterval = stepInterval / 2;
            else
            {
                actualStepInterval = stepInterval;
            }
        }
    }

    private void PostProcessValueManager()
    {
        if (actualChromaticLerpTimeValue < chromaticLerpTime)
        {
            CA.intensity.value = Mathf.Lerp(CA.intensity.value, 0, actualChromaticLerpTimeValue / chromaticLerpTime);
            actualChromaticLerpTimeValue += 1 * Time.deltaTime;
        }
    }

    public float GetSpeed()
    {
        return speed;
    }

    private void CallDirectionalDashParticle(DashDirection _dir){
        switch (_dir)
        {
            case DashDirection.Forward:
            DashParticles[0].Play();
            break;

            case DashDirection.Left:
            DashParticles[1].Play();
            break;

            case DashDirection.Right:
            DashParticles[2].Play();
            break;

            case DashDirection.Backward:
            DashParticles[3].Play();
            break;

            default:
            break;
        }
    }

    #region Coroutines
    private IEnumerator DelayOffGround()
    {
        yield return new WaitForSeconds(0.5f);
        onGround = false;
    }

    private IEnumerator DelayedJump(float _time){
        float _elapsedTime = 0.1f;
        while(_elapsedTime <= _time){
            _elapsedTime += Time.deltaTime;
            yield return null;
        }

        _rb.AddForce((transform.up * 2 + Motion * 2.5f).normalized * jumpForce, ForceMode.Impulse);
        onGround = false;
        yield return null;
        
    }
    #endregion
}
