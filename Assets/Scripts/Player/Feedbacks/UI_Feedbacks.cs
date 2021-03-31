using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Feedbacks : MonoBehaviour
{
    public static UI_Feedbacks Instance;

    public enum FeedbackType{
        Healing
    }

    public ParticleSystem Heal_PS;

    private void Awake() {
        #region Singleton Instance
        if(Instance == null){
            Instance = this;
        }
        else{
            Destroy(this.gameObject);
        }
        #endregion
    
        Heal_PS = Camera.main.transform.GetChild(2).GetComponent<ParticleSystem>();
    }

    public void CallFeedback(FeedbackType _type){
        switch (_type)
        {
            case FeedbackType.Healing:
            Heal_PS.Play();
            break;

            default:
            Debug.Log("CallFeedback(): Something fucked up");
            break;
        }
    }
}
