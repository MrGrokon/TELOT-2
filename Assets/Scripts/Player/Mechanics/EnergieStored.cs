using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnergieStored : MonoBehaviour
{
    [Range(3, 500)]
    public int MaxEnergieStorable = 5;
    public TextMeshProUGUI EnergieFeedback_Text;

    private int _energieStored = 0;
    public int _energiePerShot;
    public int startingEnergie;

    #region Unity Functions

        private void Start()
        {
            //EnergieFeedback_Text = GameObject.Find("Ammo3DText").GetComponent<TextMesh>();
            _energieStored = startingEnergie;
        }

    private void Update() {
            if(HasEnergieStored() == false){
                EnergieFeedback_Text.text = "0";
            }
            else{
                EnergieFeedback_Text.text = (_energieStored / 10).ToString();
            }            
        }
    #endregion

    #region Custom Functions
        public bool HasEnergieStored(){
            if(_energieStored>0){
                return true;
            }
            return false;
        }

        public void StoreEnergie(int EnergieQT){
            if(_energieStored + EnergieQT > MaxEnergieStorable){
                Debug.Log("No more energie storable");
                _energieStored = MaxEnergieStorable;
            }
            else
            {
                UI_Feedbacks.Instance.CallFeedback(UI_Feedbacks.FeedbackType.Reload);
                _energieStored += EnergieQT;
                Debug.Log("stock energie");
            }
        }

        public void SpendEnergie(int energie){
            _energieStored -= energie;
        }

        public int GetEnergieAmountStocked(){
            //Debug.Log(_energieStored + " Energie Stocked");
            return _energieStored;
        }

        public void AddEnergie(int ammo)
        {
            UI_Feedbacks.Instance.CallFeedback(UI_Feedbacks.FeedbackType.Reload);
            _energieStored = Mathf.Clamp(_energieStored, 0, MaxEnergieStorable);
            _energieStored += ammo;
            _energieStored = Mathf.Clamp(_energieStored, 0, MaxEnergieStorable);
        }

        public void SetGodmode()
        {
            _energieStored = 99999;
        }

        public void UnsetGodmode()
        {
            _energieStored = startingEnergie;
        }
    #endregion
}