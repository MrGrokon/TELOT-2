using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlWrist : MonoBehaviour
{
    public Slider ControlLife;

    public Slider PhantomMode;

    public Image LifeImage;
    public Image PhantomImage;

    public GameObject Wrist;
    public float speed;

    // Update is called once per frame
    void Update()
    {
        LifeImage.fillAmount = ControlLife.value;
        PhantomImage.fillAmount = PhantomMode.value;
        MoveAround();
    }

    private void MoveAround()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            transform.RotateAround(Wrist.transform.position,  Vector3.up, speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.RotateAround(Wrist.transform.position,  Vector3.up, -speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.Z))
        {
            transform.RotateAround(Wrist.transform.position,  Vector3.right, speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            transform.RotateAround(Wrist.transform.position,  Vector3.right, -speed * Time.deltaTime);
        }
    }
}
