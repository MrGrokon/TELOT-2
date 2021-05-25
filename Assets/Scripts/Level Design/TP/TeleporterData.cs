using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterData : MonoBehaviour
{
    public TeleporterData OtherSide;

    private Transform TP_Point;

    private void Awake() {
        TP_Point = this.transform.GetChild(1);
    }

    public Transform GetTeleporterPoint(){
        return TP_Point;
    }
}
