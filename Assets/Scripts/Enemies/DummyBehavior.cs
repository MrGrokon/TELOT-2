using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyBehavior : MonsterBehavior
{

    // Update is called once per frame
    override public void Update()
    {
        base.Update();

        transform.LookAt(GameObject.FindWithTag("Player").transform);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }
}
