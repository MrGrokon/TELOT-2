using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Feedbacks : MonoBehaviour
{
    public static UI_Feedbacks Instance;

    public enum FeedbackType{
        Healing
    }


    private ParticleSystem Heal_PS;
    private ParticleSystem Halo_PS;
    
    public Color HealColor;

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
        Halo_PS = Camera.main.transform.GetChild(3).GetComponent<ParticleSystem>();
    }

    public void CallFeedback(FeedbackType _type){
        switch (_type)
        {
            case FeedbackType.Healing:
            //Halo_PS.main.startColor.gradientMin = new Color(HealColor.r, HealColor.g, HealColor.b);
            Halo_PS.Play();
            Heal_PS.Play();
            break;

            default:
            Debug.Log("CallFeedback(): Something fucked up");
            break;
        }
    }
}
