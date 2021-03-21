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

    public void setDammage(float dmg)
    {
        lifePoint -= dmg;
    }

    public float getLifePoint()
    {
        return lifePoint;
    }

    public void AddLifePoint(int lp)
    {
        lifePoint += lp;
        lifePoint = Mathf.Clamp(lifePoint, 0, 100);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("EnemyProjectile"))
        {
            Destroy(other.gameObject);
            lifePoint -= other.transform.GetComponent<EnemieProjectileBehavior>().getDammage();
            FMODUnity.RuntimeManager.PlayOneShot("event:/PlayerHit"); 
        }
    }
}
