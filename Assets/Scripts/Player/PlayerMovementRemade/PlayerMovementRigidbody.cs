using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementRigidbody : MonoBehaviour
{
    private Rigidbody _rb;

    [SerializeField] private float speed;

    [SerializeField] private float dashCD;

    private float actualDashCD;

    [SerializeField] private float dashForce;
    [SerializeField] private float jumpForce; //Pas le jeu à la con hein
    [SerializeField] private float airControl;
    private float actualAirControl;
    public Vector3 Motion;
    public bool onGround = true;
    [Header("Jump Improve Parameters")]
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float lowJumpMultiplier;

    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        Move();
        if(Input.GetKeyDown(KeyCode.Space))
            Jump();
        BetterJump();
        actualDashCD -= 1 * Time.deltaTime;
    }

    private void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        if (onGround || GetComponent<WallRunningRigidbody>().OnWallRun)
        {
            actualAirControl = airControl;
            if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) 
                Motion = (h * transform.right + v * transform.forward) * actualAirControl;
        }
        else
        {
            actualAirControl = airControl / 4;
        }
        

        

        _rb.position += Motion * speed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift))
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
        }
    }

    private void Jump()
    {
        if (onGround)
        {
            _rb.AddForce(((transform.up + Motion).normalized + transform.up).normalized * jumpForce, ForceMode.Impulse);
            onGround = false;
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

    private void OnCollisionStay(Collision other)
    {
        if (onGround == false)
        {
            if (other.gameObject.layer == 8)
            {
                onGround = true;
                
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == 8)
        {
            _rb.velocity = Vector3.zero;
        }
    }


    private void OnCollisionExit(Collision other)
    {
        /*RaycastHit ray;
        if (Physics.Raycast(transform.position, -transform.up, out ray, 5f))
        {
            if (Vector3.Dot(ray.normal, transform.up) == 1)
            {
                onGround = false;
            }
            else
            {
                print("Pas une surface" + Vector3.Dot(ray.normal, transform.up));
            } 
        }*/
        if (other.gameObject.layer == 8)
        {
            onGround = false;
        }
    }
}
