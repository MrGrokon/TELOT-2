using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProgressionFollow : MonoBehaviour
{

    public TextMeshProUGUI roomCount;
    public TextMeshProUGUI ennemiesRemaining;

    public void UpdateRoomCount(int actualRoom, int maxRoom)
    {
        roomCount.text = "Room : " + actualRoom + " / " + maxRoom;
    }

    public void UpdateEnnemiesRemainingCount(int ennemiesRemaining)
    {
        this.ennemiesRemaining.text = "" + ennemiesRemaining;
    }
}
