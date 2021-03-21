using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectEnergie : MonoBehaviour
{
    public GameObject Projectil_Object;
    [Range(1f, 30f)]
    public float AmountOfSpread = 10f;

    private EnergieStored _Energie;
    private bool _canShoot = true;
    public Transform shotLocation;
    public float projectileSpeed;
    public RawImage hitMarker;
    private float timeToHideHit = 0.3f;
    public float shotDistance;

    #region Unity Functions
        private void Awake() {
            _Energie = this.GetComponent<EnergieStored>();
            if(_Energie == null){
                Debug.Log("EnergieStored not defined");
            }
        }

        private void Update() {
            if(Input.GetButtonDown("Fire1") && _Energie.HasEnergieStored() && _canShoot){
                //Debug.Log("Shoot");
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
            HideHit();
        }
    #endregion

    #region Coroutines
        IEnumerator ShootProcedure_Shootgun(int _nmbOfPellet){
            _canShoot = false;
            ObjectReferencer.Instance.Crossair_Object.GetComponent<Animator>().SetTrigger("Shoot");
            StartCoroutine(this.GetComponent<TraumaInducer>().StartScreenShake());

            Debug.Log("shoot " + _nmbOfPellet + " pellets");
            FMODUnity.RuntimeManager.PlayOneShot("event:/Shoot/PrimaryShot");
            for (int i = 0; i < _nmbOfPellet; i++)
            {
                Quaternion Spread = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles + new Vector3( (Random.Range(-AmountOfSpread, AmountOfSpread)), (Random.Range(-AmountOfSpread, AmountOfSpread)), 0f));
                GameObject _proj = Instantiate(Projectil_Object, shotLocation.position, Spread);
                _proj.GetComponent<ProjectilBehavior>().SetSpeed(projectileSpeed);
                Debug.DrawRay(shotLocation.position, _proj.transform.forward * shotDistance, Color.red, Mathf.Infinity);
                
                /*if (Physics.Raycast(shotLocation.position, _proj.transform.forward, out RaycastHit hit,shotDistance))
                {
                    if (hit.transform.CompareTag("Ennemy"))
                    {
                        ObjectReferencer.Instance.Avatar_Object.GetComponent<ProjectEnergie>().hitMarker.gameObject
                            .SetActive(true);
                    }
                }*/
            }

            _Energie.SpendEnergie(_Energie._energiePerShot);
            yield return null;
        }

        IEnumerator ShootProcedure_All(int _nmbOfPellet)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Shoot/SecondaryShot");
            for (int i = 0; i < _nmbOfPellet; i++)
            {
                Quaternion Spread = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles + new Vector3( (Random.Range(-AmountOfSpread, AmountOfSpread)), (Random.Range(-AmountOfSpread, AmountOfSpread)), 0f));
                GameObject _proj = Instantiate(Projectil_Object, shotLocation.position, Spread);
                _proj.GetComponent<ProjectilBehavior>().SetSpeed(projectileSpeed);
                /*if (Physics.Raycast(shotLocation.position, _proj.transform.forward, out RaycastHit hit,shotDistance))
                {
                    if (hit.transform.CompareTag("Ennemy"))
                    {
                        ObjectReferencer.Instance.Avatar_Object.GetComponent<ProjectEnergie>().hitMarker.gameObject
                            .SetActive(true);
                    }
                }*/
            }

            _Energie.SpendEnergie(_Energie.GetEnergieAmountStocked());
            yield return null;
        }
        
        private void HideHit()
        {
            if (ObjectReferencer.Instance.Avatar_Object.GetComponent<ProjectEnergie>().hitMarker.gameObject.activeSelf)
            {
                timeToHideHit -= 1 * Time.deltaTime;
                if (timeToHideHit <= 0)
                {
                    ObjectReferencer.Instance.Avatar_Object.GetComponent<ProjectEnergie>().hitMarker.gameObject.SetActive(false);
                    timeToHideHit = 0.3f;
                }
            }
            
        }
    #endregion

    #region Reset shooter boolean
        public void ResetShootBoolean(){
            _canShoot = true;
        }
    #endregion
}
