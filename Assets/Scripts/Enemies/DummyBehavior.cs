using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyBehavior : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(GameObject.FindWithTag("Player").transform);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }
}
