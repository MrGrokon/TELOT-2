using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGameWhenButtonPressed : MonoBehaviour
{
    public KeyCode PauseButton = KeyCode.T;

    private bool GameIsPaused = false;

    private void Update() {
        if(Input.GetKeyDown(PauseButton)){
            if(GameIsPaused){
                GameIsPaused = false;
                Time.timeScale = 1f;
            }
            else{
                GameIsPaused = true;
                Time.timeScale = 0f;
            }
        }
    }
}
