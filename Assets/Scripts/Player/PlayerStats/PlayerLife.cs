using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerLife : MonoBehaviour
{

    [SerializeField] private float lifePoint;
    public float startingLifePoint;
    private Slider lifeText;

    private void Start()
    {
        lifeText = GameObject.Find("LifeSlider").GetComponent<Slider>();
        lifePoint = startingLifePoint;
        lifeText.value = lifePoint / startingLifePoint;
    }


    private void Update()
    {
        if(lifePoint <= 0)
            SceneManager.LoadScene(0);
        lifeText.value = lifePoint / startingLifePoint;
    }

    public void TakeDammage(float dmg)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/PlayerHit"); 
        lifePoint -= dmg;
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
