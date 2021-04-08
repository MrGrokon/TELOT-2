using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectEnergie : MonoBehaviour
{
    [Header("Init by hand")]
    public GameObject Projectil_Object;
    public Transform shotLocation;

    [Header("General Parameters")]
    public bool isDebuging = false;
    private bool _canShoot = true;
    private EnergieStored _Energie;
    
    [Range(1f, 30f)]
    public float AmountOfSpread = 10f;
    [Range(30,300)]
    public int RateOfFire = 60;
    private float TimeBetweenShootConverted;
    public float projectileSpeed;
    
    [HideInInspector]
    public RawImage hitMarker;
    private float timeToHideHit = 0.3f;
    public float shotDistance;

    [Header("Feedbacks Parameters")]
    public CrossairFeedbackType FeedbackType = CrossairFeedbackType.Expend;
    [Tooltip("will define the strick value of the size of my crossair")]
    public AnimationCurve CrossairScale_Curve;

    public enum CrossairFeedbackType{
        Expend,
        ExpendAndRotate
    }


    #region Unity Functions
        private void Awake() {
            TimeBetweenShootConverted = 1/(RateOfFire/60f);
            _Energie = this.GetComponent<EnergieStored>();
            if(_Energie == null){
                Debug.Log("EnergieStored not defined");
            }
        }

        private void Start() {
            hitMarker = GameObject.Find("Hitmarker").GetComponent<RawImage>();
        }

        private void Update() {
            if(Input.GetButtonDown("Fire1") && _Energie.HasEnergieStored() && _canShoot){
                //Debug.Log("Shoot");
                GetComponent<WeaponRecoil>().enabled = true;
                GetComponent<WeaponRecoil>().recoil += 0.1f;
                if(_Energie.GetEnergieAmountStocked() >= _Energie._energiePerShot)
                    StartCoroutine(ShootProcedure_Shootgun(_Energie._energiePerShot));
                else
                {
                    StartCoroutine(ShootProcedure_Shootgun(_Energie.GetEnergieAmountStocked()));
                }
            }
            /*else if (Input.GetButtonDown("Fire3") && _Energie.HasEnergieStored())
            {
                StartCoroutine(ShootProcedure_All(_Energie.GetEnergieAmountStocked()));
            }*/
            HideHit();
        }
    #endregion

    #region Coroutines
        IEnumerator ShootProcedure_Shootgun(int _nmbOfPellet){
            //_canShoot = false;
            //ObjectReferencer.Instance.Crossair_Object.GetComponent<Animator>().SetTrigger("Shoot");
            StartCoroutine(CrossairFeedbacks(TimeBetweenShootConverted, FeedbackType));
            StartCoroutine(this.GetComponent<TraumaInducer>().StartScreenShake());

            Debug.Log("shoot " + _nmbOfPellet + " pellets");
            FMODUnity.RuntimeManager.PlayOneShot("event:/Shoot/PrimaryShot");
            for (int i = 0; i < _nmbOfPellet; i++)
            {
                Quaternion Spread = Quaternion.Euler(Camera.main.transform.rotation.eulerAngles + new Vector3( (Random.Range(-AmountOfSpread, AmountOfSpread)), (Random.Range(-AmountOfSpread, AmountOfSpread)), 0f));
                GameObject _proj = Instantiate(Projectil_Object, shotLocation.position, Spread);
                _proj.GetComponent<ProjectilBehavior>().SetSpeed(projectileSpeed);
                if(isDebuging){
                    Debug.DrawRay(shotLocation.position, _proj.transform.forward * shotDistance, Color.red, Mathf.Infinity);
                }
                
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
        
        IEnumerator CrossairFeedbacks(float TimeOfFeedbacks, CrossairFeedbackType _type){
            float _elapsedTime = 0f;
            _canShoot = false;

            switch (_type)
            {
                case CrossairFeedbackType.Expend:
                    while(_elapsedTime < TimeOfFeedbacks){
                        _elapsedTime += Time.deltaTime;
                        float EstimatedValue = CrossairScale_Curve.Evaluate(_elapsedTime / TimeOfFeedbacks);
                        ObjectReferencer.Instance.Crossair_Object.transform.localScale = new Vector3(EstimatedValue, EstimatedValue, 0f);
                        yield return null;
                    }
                break;

                case CrossairFeedbackType.ExpendAndRotate:
                    while(_elapsedTime < TimeOfFeedbacks){
                        _elapsedTime += Time.deltaTime;

                        float EstimatedValue = CrossairScale_Curve.Evaluate(_elapsedTime / TimeOfFeedbacks);
                        ObjectReferencer.Instance.Crossair_Object.transform.localScale = new Vector3(EstimatedValue, EstimatedValue, 0f);

                        float RotationValue = Mathf.Lerp(0, 360, _elapsedTime / TimeOfFeedbacks);
                        ObjectReferencer.Instance.Crossair_Object.transform.rotation = Quaternion.Euler(0f, 0f, RotationValue);
                        yield return null;
                    }
                break;

                default:
                Debug.Log("Something fucked up in CrossairFeedback");
                break;
            }
            
            _canShoot = true;
            yield return null;
        }

        private void HideHit()
        {
            if (ObjectReferencer.Instance.Avatar_Object.GetComponent<ProjectEnergie>().hitMarker.gameObject.activeSelf)
            {
                timeToHideHit -= 1 * Time.deltaTime;
                if (timeToHideHit <= 0)
                {
                    ObjectReferencer.Instance.Avatar_Object.GetComponent<ProjectEnergie>().hitMarker.gameObject
                        .SetActive(false);
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
