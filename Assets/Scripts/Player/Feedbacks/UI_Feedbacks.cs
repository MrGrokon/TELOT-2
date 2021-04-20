using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class UI_Feedbacks : MonoBehaviour
{
    public static UI_Feedbacks Instance;

    public enum FeedbackType{
        Healing,
        Reload,
        FOV_zoom,
        Jump
    }

    private ParticleSystem Heal_PS;
    private ParticleSystem Halo_PS;
    private ParticleSystem AmmoFlickering_PS;
    private ParticleSystem BloodSplater_PS; 

    public Color HealColor;

    [Header("Parameters linked to Health")]
    [Range(0f,100f)]
    public float LevelBeforeHaloAppearance = 75f;
    [Range(0f,100f)]
    public float LevelBeforeDrops = 35f;
    [Range(0f,1f)]
    public float BaseIntensity = 0.8f;
    private PostProcessVolume PP_volume;
    private PlayerLife _LifeManager;
    private Vignette _vignette;
    
    private bool BloodSplater_IsPlaying = false;

    [Header("FOV zoom in while shoot")]
    public float FOV_ModificationTime = 0.1f;
    public float FOV_Delta = 5f;
    public AnimationCurve FOV_DeltaValueOverTime;
    private float Base_FOV = 60f;

    [Header("Camera Vertical Offset while jump")]
    public float OffsetTime = 0.2f;
    public Vector3 TargetPosition = new Vector3(0, -1, 0);
    public AnimationCurve CameraPositionOverTime;

    #region Unity functions
    #region Init
    private void Awake() {
        #region Singleton Instance
        if(Instance == null){
            Instance = this;
        }
        else{
            Destroy(this.gameObject);
        }
        #endregion
    
        Heal_PS = ObjectReferencer.Instance.UI_particle_container.GetChild(3).GetComponent<ParticleSystem>();
        Halo_PS = ObjectReferencer.Instance.UI_particle_container.GetChild(1).GetComponent<ParticleSystem>();
        BloodSplater_PS = ObjectReferencer.Instance.UI_particle_container.GetChild(2).GetComponent<ParticleSystem>();
        AmmoFlickering_PS = ObjectReferencer.Instance.UI_particle_container.GetChild(0).GetComponent<ParticleSystem>();
    }

    private void Start() {
        PP_volume = Camera.main.GetComponent<PostProcessVolume>();
        PP_volume.profile.TryGetSettings<Vignette>(out _vignette);
        _vignette.active = true;
        _vignette.intensity.value = 0f;
        _LifeManager = ObjectReferencer.Instance.Avatar_Object.GetComponent<PlayerLife>();
    }
    #endregion

    private void Update() {
        float _HealthPercent = _LifeManager.getLifePoint() / _LifeManager.startingLifePoint * 100f;
        Debug.Log("HP% -> " + _HealthPercent);
        if(_HealthPercent <= LevelBeforeHaloAppearance){
            float _intensity = Mathf.Lerp(0f, BaseIntensity, 1 - (_HealthPercent / 100f) );
            _vignette.intensity.value = _intensity;
        }

        #region Apparition/Disparition du PS de sang
        if(_HealthPercent <= LevelBeforeDrops){
            if(BloodSplater_IsPlaying == false){
                BloodSplater_PS.Play();
                BloodSplater_IsPlaying = true;
            }
        }
        else{
            if(BloodSplater_IsPlaying == true){
                BloodSplater_PS.Stop();
                BloodSplater_IsPlaying = false;
            }
        }
        #endregion
    }
    #endregion

    public void CallFeedback(FeedbackType _type){
        switch (_type)
        {
            case FeedbackType.Healing:
            //Halo_PS.main.startColor.gradientMin = new Color(HealColor.r, HealColor.g, HealColor.b);
 
            Halo_PS.Play();
            Heal_PS.Play();
            break;

            case FeedbackType.Reload:
            AmmoFlickering_PS.Play();
            break;

            case FeedbackType.FOV_zoom:
            StartCoroutine(FOV_ZoomIn_Procedure());
            break;

            case FeedbackType.Jump:
            StartCoroutine(CameraVerticalOffset());
            break;

            default:
            Debug.Log("CallFeedback(): Something fucked up");
            break;
        }
    }

    private IEnumerator FOV_ZoomIn_Procedure(){
        float _elapsedTime = 0f;
        float _base_FOV = Camera.main.fieldOfView;

        while(_elapsedTime <= FOV_ModificationTime){
            _elapsedTime += Time.deltaTime;
            
            Camera.main.fieldOfView = Base_FOV + (FOV_DeltaValueOverTime.Evaluate(_elapsedTime/FOV_ModificationTime) * FOV_Delta);
            yield return null;
        }
        yield return null;
    }

    private IEnumerator CameraVerticalOffset(){
        float _elapsedTime = 0f;
        Vector3 _offset_position;
        while(_elapsedTime <= OffsetTime){
            _elapsedTime += Time.deltaTime;
            _offset_position = CameraPositionOverTime.Evaluate(_elapsedTime / OffsetTime) * TargetPosition;

            Camera.main.transform.localPosition = _offset_position;
            yield return null;
        }
        yield return null;
    }
}
