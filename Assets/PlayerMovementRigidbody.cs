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

    public bool onGround = true;
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
        actualDashCD -= 1 * Time.deltaTime;
    }

    private void Move()
    {
        
        if (onGround)
        {
            actualAirControl = airControl;
        }
        else
        {
            actualAirControl = airControl / 4;
        }
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 Motion = (h * transform.right + v * transform.forward) * actualAirControl;

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
            _rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            onGround = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (onGround == false)
        {
            RaycastHit ray;
            if (Physics.Raycast(transform.position, -transform.up, out ray, 5f))
            {
                if (Vector3.Dot(ray.normal, transform.up) == 1)
                {
                    onGround = true;
                }
                else
                {
                    print("Pas une surface" + Vector3.Dot(ray.normal, transform.up));
                } 
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        RaycastHit ray;
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
        }
    }
}
