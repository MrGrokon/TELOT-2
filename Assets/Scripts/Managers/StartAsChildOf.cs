using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAsChildOf : MonoBehaviour
{
    public string NameOfParent;

    private void Awake() {
        if(NameOfParent != null){
            GameObject _parent_temps = GameObject.Find(NameOfParent);
            if(_parent_temps != null){
                this.transform.SetParent(_parent_temps.transform);
            }
        }
    }
}
