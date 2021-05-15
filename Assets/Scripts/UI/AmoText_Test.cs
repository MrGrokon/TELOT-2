using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmoText_Test : MonoBehaviour
{
    public Gradient ColorOverEnergieAmount;
    [Range(0.01f, 1f)]
    public float BlinkTime = 0.25f;

    private EnergieStored _energie;
    private TextMeshProUGUI _tm_pro;
    private bool isBlinking =false;

    void Start()
    {
        _energie = ObjectReferencer.Instance.Avatar_Object.GetComponent<EnergieStored>();
        _tm_pro = this.GetComponent<TextMeshProUGUI>();
        //StartCoroutine(Blink());
    }

    private void Update() {
        float EnergiePersent = (float)_energie.GetEnergieAmountStocked() / (float)_energie.MaxEnergieStorable;
        _tm_pro.color = ColorOverEnergieAmount.Evaluate(EnergiePersent);
        
        if(EnergiePersent <= 0.33f){
            isBlinking = true;
        }
        else{
            isBlinking = false;
        }
    }

    IEnumerator Blink(){
        float elapsedTime = 0f;
        while (true)
        {
            if(isBlinking){
                while(elapsedTime <= BlinkTime){
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                if(_tm_pro.color.a == 0f){
                    _tm_pro.color = new Color(_tm_pro.color.r, _tm_pro.color.g, _tm_pro.color.b, 1f);
                   
                }
                else if(_tm_pro.color.a == 1f){
                    _tm_pro.color = new Color(_tm_pro.color.r, _tm_pro.color.g, _tm_pro.color.b, 0f);
                }
                elapsedTime =0f;
            }
            yield return null;
        }
    }
}
