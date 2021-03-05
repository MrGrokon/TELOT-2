using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergieStored : MonoBehaviour
{
    [Range(3, 500)]
    public int MaxEnergieStorable = 5;
    public UnityEngine.UI.Text EnergieFeedback_Text;

    private int _energieStored = 0;
    public int _energiePerShot;
    public int startingEnergie;

    #region Unity Functions

        private void Start()
        {
            _energieStored = startingEnergie;
        }

    private void Update() {
            if(HasEnergieStored() == false){
                EnergieFeedback_Text.text = "No Energie";
            }
            else{
                EnergieFeedback_Text.text = _energieStored.ToString();
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

        public void StoreEnergie(int EnergieQT = 1){
            if(_energieStored + EnergieQT > MaxEnergieStorable){
                Debug.Log("No more energie storable");
                _energieStored = MaxEnergieStorable;
            }
            else
            {
                _energieStored += EnergieQT;
                Debug.Log("stock energie");
            }
        }

        public void SpendEnergie(int energie){
            _energieStored -= energie;
        }

        public int GetEnergieAmountStocked(){
            Debug.Log(_energieStored + " Energie Stocked");
            return _energieStored;
        }

        public void AddEnergie(int ammo)
        {
            _energieStored = Mathf.Clamp(_energieStored, 0, MaxEnergieStorable);
            _energieStored += ammo;
            _energieStored = Mathf.Clamp(_energieStored, 0, MaxEnergieStorable);
        }
    #endregion
}