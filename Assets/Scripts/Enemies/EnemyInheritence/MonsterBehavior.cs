using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBehavior : MonoBehaviour
{
    [Range(1, 1000)]
    public int StartHealth = 100;

    private int _Health;
    
    #region Unity Base Functions
        public virtual void Awake() {
            _Health = StartHealth;
        }

        virtual public void Update() {
            // base update shits
        }
    #endregion
    


    #region Health Related Functions
        virtual public void TakeDamage(int _amount){
            if(_Health - _amount < 0){
                Debug.Log( this.name + " died");
                _Health = 0;
                Die();
            }
            else
            _Health -= _amount;
        }

        public bool IsAlive(){
            if(_Health > 0){
                return true;
            }
            return false;
        }

        void Die(){
            // dying feedbacks
        }
    #endregion
}
