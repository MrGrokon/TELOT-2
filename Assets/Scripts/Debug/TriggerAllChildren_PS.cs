using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAllChildren_PS : MonoBehaviour
{
    public KeyCode ActivationKey = KeyCode.Space;
    public bool IsActivatedAutomaticly = false;
    public float TimeBetweenActivation = 3f;
    private float _elapsedTime = 0f;

    private List<ParticleSystem> PS_list = new List<ParticleSystem>();

    void Start()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            PS_list.Add(this.transform.GetChild(i).GetComponent<ParticleSystem>());
        }
    }

    private void Update() {
        if(IsActivatedAutomaticly){
            _elapsedTime += Time.deltaTime;
            if(_elapsedTime >= TimeBetweenActivation){
                _elapsedTime = 0f;
                ActivatedAllChilds_PS();
            }
        }

        if(Input.GetKeyDown(ActivationKey)){
            ActivatedAllChilds_PS();
        }
    }

    private void ActivatedAllChilds_PS(){
        foreach (var _ps in PS_list)
            {
                _ps.Play();
            }
    }
}
