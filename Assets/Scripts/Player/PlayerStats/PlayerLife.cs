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
    public Image lifeText;
    private FMOD.Studio.EventInstance HeartBeatEvent;
    private FMOD.Studio.EventDescription heartbeatDescription;
    private FMOD.Studio.PARAMETER_DESCRIPTION pd;
    FMOD.Studio.PARAMETER_ID parameterID;

    private void Start()
    {
        lifePoint = startingLifePoint;  //Je mets les points de vie du joueur au maximum
        lifeText.fillAmount = lifePoint / startingLifePoint;
        HeartBeatEvent = FMODUnity.RuntimeManager.CreateInstance("event:/Player/LowHpHeart"); //Je créer une instance de mon
                                                                                              //son afin de le garden en mémoire
        HeartBeatEvent.start();  //Je joue l'instance de mon son

        heartbeatDescription = FMODUnity.RuntimeManager.GetEventDescription("event:/Player/LowHpHeart"); //Je récupère la description
                                                                                                         //de mon évènement
        heartbeatDescription.getParameterDescriptionByName("Health", out pd); //Et je stockes son paramètre appelé
                                                                              //Health dans une variable de sortie
        parameterID = pd.id; //J'assigne son paramètre à une variable interne
    }


    private void Update()
    {
        HeartBeatEvent.setParameterByID(parameterID , (lifePoint / startingLifePoint) * 100);
        if (lifePoint <= 0)
        {
            Death();
        }
            
        lifeText.fillAmount = lifePoint / startingLifePoint;
    }

    public void TakeDammage(float dmg)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Player/Hit"); 
        lifePoint -= dmg;
    }

    public void Death()
    {
        FeedbackManager.Instance.GetComponent<MusicManager>().StopMusic();
        HeartBeatEvent.stop(STOP_MODE.ALLOWFADEOUT);
        GetComponent<BlockProjectiles>().shieldIdle.stop(STOP_MODE.ALLOWFADEOUT);
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
