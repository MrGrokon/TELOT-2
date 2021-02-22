using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergieStored : MonoBehaviour
{
    [Range(1, 10)]
    public int MaxAmountOfEnergieStored = 5;

    public int _actualEnergieStored = 0;

    #region Energie Related Functions
        public void StoreEnergie(int OptionalAmount = 1){
            Debug.Log("Energie is stocked");
            if((_actualEnergieStored + OptionalAmount) >= MaxAmountOfEnergieStored){
                _actualEnergieStored = MaxAmountOfEnergieStored;
            }
            else{
                _actualEnergieStored += OptionalAmount;
            }
        }

        public bool HasEnergieStored(){
            if(_actualEnergieStored >= 1){
                return true;
            }
            return false;
        }

        public void SpendEnergie(int OptionalAmount = -1){
            if(HasEnergieStored()){
                if(OptionalAmount <= 0){
                    _actualEnergieStored = 0;
                }
                else{
                    if((_actualEnergieStored - OptionalAmount) <= 0){
                        _actualEnergieStored = 0;
                    }
                    else{
                        _actualEnergieStored -= OptionalAmount;
                    }
                }
            }
        }
    #endregion

    
}
