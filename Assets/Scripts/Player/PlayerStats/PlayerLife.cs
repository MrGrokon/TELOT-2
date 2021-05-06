using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FMOD.Studio;

public class PlayerLife : MonoBehaviour
{

    [SerializeField] private float lifePoint;
    public float startingLifePoint;
    private Slider lifeText;
    private FMOD.Studio.EventInstance HeartBeatEvent;
    private FMOD.Studio.EventDescription heartbeatDescription;
    private FMOD.Studio.PARAMETER_DESCRIPTION pd;
    FMOD.Studio.PARAMETER_ID parameterID;

    private void Start()
    {
        lifeText = GameObject.Find("LifeSlider").GetComponent<Slider>();
        lifePoint = startingLifePoint;
        lifeText.value = lifePoint / startingLifePoint;
        HeartBeatEvent = FMODUnity.RuntimeManager.CreateInstance("event:/Player/LowHpHeart");
        HeartBeatEvent.start();

        heartbeatDescription = FMODUnity.RuntimeManager.GetEventDescription("event:/Player/LowHpHeart");
        heartbeatDescription.getParameterDescriptionByName("Health", out pd);
        parameterID = pd.id;
    }


    private void Update()
    {
        HeartBeatEvent.setParameterByID(parameterID , (lifePoint / startingLifePoint) * 100);
        if (lifePoint <= 0)
        {
            Death();
        }
            
        lifeText.value = lifePoint / startingLifePoint;
    }

    public void TakeDammage(float dmg)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Player/Hit"); 
        lifePoint -= dmg;
    }

    private void Death()
    {
        FeedbackManager.Instance.GetComponent<MusicManager>().StopMusic();
        SceneManager.LoadScene(0);
    }

    public float getLifePoint()
    {
        return lifePoint;
    }

    public void AddLifePoint(int lp)
    {
        //feedbacks
        UI_Feedbacks.Instance.CallFeedback(UI_Feedbacks.FeedbackType.Healing);
        //Mecha
        lifePoint += lp;
        lifePoint = Mathf.Clamp(lifePoint, 0, startingLifePoint);
    }

    public void SetGodmode()
    {
        lifePoint = 999999.0f;
        startingLifePoint = 999999.0f;
    }
    
    public void UnsetGodmode()
    {
        lifePoint = startingLifePoint;
        startingLifePoint = 100;
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("EnemyProjectile"))
        {
            Destroy(other.gameObject);
            TakeDammage(other.transform.GetComponent<EnemieProjectileBehavior>().getDammage());  
        }
    }*/
}
