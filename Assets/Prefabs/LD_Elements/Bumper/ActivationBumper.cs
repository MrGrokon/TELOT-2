using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationBumper : MonoBehaviour
{
    //Fetch the Animator
    public Animator Sequence;
    // Use this to decide if the GameObject can jump or not
    public bool TriggerBool = false;
    public bool OnPlateform1 = false;
    public Collider TriggerBall;
    public float BumpForce = 10f;


    void Start()
    {
        Sequence = gameObject.GetComponent<Animator>();
    }


    void OnTriggerEnter(Collider other)
    {
        Sequence.SetBool("Trigger", true);
        TriggerBool = true;
    }
    void OnTriggerExit(Collider other)
    {
        Sequence.SetBool("Trigger", false);
        TriggerBool = false;
    }

    private void Update()
    {
        if(OnPlateform1==true)
        {
            /*if(GameObject.Find("Cylinder.064").GetComponent<Propulsion>()._col.gameObject.tag == "Player")
            {

            }
            Sequence.SetBool("OnPlateform", true);*/
            //col.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * BumpForce, ForceMode.Impulse);
        }
        if (OnPlateform1 == false)
        {
            Sequence.SetBool("OnPlateform", false);
        }
    }
}




