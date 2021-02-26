using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackEnergieValue : MonoBehaviour
{
    private Text _txt;

    private void Awake() {
        _txt = this.GetComponent<Text>();
    }

    private void LateUpdate() {
        _txt.text = ObjectReferencer.Instance.Avatar_Object.GetComponent<EnergieStored>()._actualEnergieStored.ToString();
    }
}
