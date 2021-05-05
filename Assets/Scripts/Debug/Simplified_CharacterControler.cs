using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simplified_CharacterControler : MonoBehaviour
{
    public float PlayerSpeed = 10f;

    void Update()
    {
        if(Input.GetKey(KeyCode.Z)){
            this.transform.Translate(this.transform.forward * PlayerSpeed * Time.deltaTime, Space.Self);
        }

        if(Input.GetKey(KeyCode.D)){
            this.transform.Translate(this.transform.right * PlayerSpeed * Time.deltaTime, Space.Self);

        }

        if(Input.GetKey(KeyCode.S)){
            this.transform.Translate( - this.transform.forward * PlayerSpeed * Time.deltaTime, Space.Self);
        }

        if(Input.GetKey(KeyCode.Q)){
            this.transform.Translate( - this.transform.right * PlayerSpeed * Time.deltaTime, Space.Self);
        }
    }
}
