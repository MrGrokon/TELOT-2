using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAllChildren_PS : MonoBehaviour
{
    public KeyCode ActivationKey = KeyCode.Space;

    private List<ParticleSystem> PS_list = new List<ParticleSystem>();

    void Start()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            PS_list.Add(this.transform.GetChild(i).GetComponent<ParticleSystem>());
        }
    }

    private void Update() {
        if(Input.GetKeyDown(ActivationKey)){
            foreach (var _ps in PS_list)
            {
                _ps.Play();
            }
        }
    }
}
