using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectEnergie : MonoBehaviour
{
    public GameObject Projectil_Object;
    [Range(5f, 15f)]
    public float AmountOfSpread = 10f;

    private EnergieStored _Energie;
    public Transform shotLocation;
    public float projectileSpeed;

    #region Unity Functions
        private void Awake() {
            _Energie = this.GetComponent<EnergieStored>();
            if(_Energie == null){
                Debug.Log("EnergieStored not defined");
            }
        }

        private void Update() {
            if(Input.GetButtonDown("Fire1") && _Energie.HasEnergieStored()){
                Debug.Log("Shoot");
                if(_Energie.GetEnergieAmountStocked() >= _Energie._energiePerShot)
                    StartCoroutine(ShootProcedure_Shootgun(_Energie._energiePerShot));
                else
                {
                    StartCoroutine(ShootProcedure_Shootgun(_Energie.GetEnergieAmountStocked()));
                }
            }
            else if (Input.GetButtonDown("Fire3") && _Energie.HasEnergieStored())
            {
                StartCoroutine(ShootProcedure_All(_Energie.GetEnergieAmountStocked()));
            }
        }
    #endregion

    #region Coroutines
        IEnumerator ShootProcedure_Shootgun(int _nmbOfPellet){
            Debug.Log("shoot " + _nmbOfPellet + " pellets");
            
            for (int i = 0; i < _nmbOfPellet; i++)
            {
                Quaternion Spread = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles + new Vector3( (Random.Range(-AmountOfSpread, AmountOfSpread)), (Random.Range(-AmountOfSpread, AmountOfSpread)), 0f));
                GameObject _proj = Instantiate(Projectil_Object, shotLocation.position, Spread);
                _proj.GetComponent<ProjectilBehavior>().SetSpeed(projectileSpeed);
            }

            _Energie.SpendEnergie(_Energie._energiePerShot);
            yield return null;
        }

        IEnumerator ShootProcedure_All(int _nmbOfPellet)
        {
            for (int i = 0; i < _nmbOfPellet; i++)
            {
                Quaternion Spread = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles + new Vector3( (Random.Range(-AmountOfSpread, AmountOfSpread)), (Random.Range(-AmountOfSpread, AmountOfSpread)), 0f));
                GameObject _proj = Instantiate(Projectil_Object, shotLocation.position, Spread);
                _proj.GetComponent<ProjectilBehavior>().SetSpeed(projectileSpeed);
            }

            _Energie.SpendEnergie(_Energie.GetEnergieAmountStocked());
            yield return null;
        }
    #endregion
}
