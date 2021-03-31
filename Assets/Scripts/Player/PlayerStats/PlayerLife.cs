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
    private Text lifeText;

    private void Start()
    {
        lifeText = GameObject.Find("LifeText").GetComponent<UnityEngine.UI.Text>();
        lifePoint = startingLifePoint;
        lifeText.text = lifePoint + " / " + startingLifePoint;
    }


    private void Update()
    {
        if(lifePoint <= 0)
            SceneManager.LoadScene(0);
        lifeText.text = lifePoint + " / " + startingLifePoint;
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
        lifePoint = Mathf.Clamp(lifePoint, 0, 100);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("EnemyProjectile"))
        {
            Destroy(other.gameObject);
            TakeDammage(other.transform.GetComponent<EnemieProjectileBehavior>().getDammage());  
        }
    }
}
