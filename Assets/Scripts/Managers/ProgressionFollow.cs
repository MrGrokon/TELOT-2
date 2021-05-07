using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProgressionFollow : MonoBehaviour
{

    public TextMeshProUGUI roomCount;
    public TextMeshProUGUI ennemiesRemaining;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateRoomCount(int actualRoom, int maxRoom)
    {
        roomCount.text = "Room : " + actualRoom + " / " + maxRoom;
    }

    public void UpdateEnnemiesRemainingCount(int ennemiesRemaining)
    {
        this.ennemiesRemaining.text = "" + ennemiesRemaining;
    }
}
