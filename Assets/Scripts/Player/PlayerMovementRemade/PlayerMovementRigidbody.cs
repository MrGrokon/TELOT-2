using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerMovementRigidbody : MonoBehaviour
{
    private Rigidbody _rb;

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
    public ParticleSystem DashParticle;

    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        volume.profile.TryGetSettings(out CA);
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
        DetectGround();
        PostProcessValueManager();
    }

    private void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        if (onGround || GetComponent<WallRunningRigidbody>().OnWallRun)
        {
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                if (GetComponent<WallRunningRigidbody>().OnWallRun)
                {
                    Motion = (v * GetComponent<WallRunningRigidbody>().wallForwardRun);
                }
                else if(onGround)
                {
                    Motion = (h * transform.right + v * transform.forward).normalized;
                }
                
            }
            else
                Motion = Vector3.zero;
        }
        else
        {
            Motion = ((h * transform.right + v * transform.forward).normalized * airControl);
        }

        _rb.position += Motion * speed * Time.deltaTime;

        if (Input.GetButton("Dash"))
        {
            Dash(Motion);
        }
    }

    private void Dash(Vector3 motion)
    {
        if (actualDashCD <= 0)
        {
            _rb.AddForce(motion.normalized * dashForce, ForceMode.Impulse);
            actualDashCD = dashCD;
            CA.intensity.value = 1f;
            actualChromaticLerpTimeValue = 0;
            DashParticle.Play();
            FMODUnity.RuntimeManager.PlayOneShot("event:/Movement/Dash"); 
        }
    }

    private void Jump()
    {
        if (onGround)
        {
            print("Simple jump");
            _rb.AddForce((transform.up * 2 + Motion * 2.5f).normalized * jumpForce, ForceMode.Impulse);
            onGround = false;
            FMODUnity.RuntimeManager.PlayOneShot("event:/Movement/Jump", transform.position);
        }
        else if (doubleJump && !GetComponent<WallRunningRigidbody>().OnWallRun)
        {
            print("Double jump");
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
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 3f))
        {
            if (hit.transform.gameObject.layer == 8)
            {
                if (onGround == false)
                {
                    FMODUnity.RuntimeManager.PlayOneShot("event:/Movement/LandOnGround", transform.position);
                }
                onGround = true;
                doubleJump = true;
            }
        }
        else
        {
            onGround = false;
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

    private void OnCollisionEnter(Collision other)
    {
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }

    public float GetSpeed()
    {
        return speed;
    }

    



}
