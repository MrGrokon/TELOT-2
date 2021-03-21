using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetShootBoolean_AnimEvent : MonoBehaviour
{
    public void RefreshShootBoolean(){
        ObjectReferencer.Instance.Avatar_Object.GetComponent<ProjectEnergie>().ResetShootBoolean();
    }
}
