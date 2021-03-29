using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackManager : MonoBehaviour
{
    public static FeedbackManager Instance;
    
    public enum CameraDirection
    {
        Left,
        Right
    };

    [Header("Camera Rotion when wall run")]
    [Range(10f, 70f)]
    public float RotationDegree = 30f;
    [Range(0.5f, 3f)]
    public float TimeToRotate = 1f;

    private Transform Camera_Transform;
    private GameObject Avatar_Object;

    [Header("Dash Feedbacks & related")]
    private ParticleSystem PS_Dash;
    private Transform PS_Pivot;
    [Range(1f, 10f)]
    public float ParticuleSystemeDistanceFromPlayer = 5f;
    //public float FOV_MaxDeformation =

    private void Awake() {
        #region Singleton Instance
            if(Instance == null){
                Instance = this;
            }
            else{
                Destroy(this.gameObject);
            }
        #endregion

        Camera_Transform = GameObject.Find("Main Camera").transform;
        if(Camera_Transform == null){
            Debug.Log("Main camera not found");
        }

       /* PS_Pivot = GameObject.Find("DashParticule_Pivot").transform;
        PS_Dash = GameObject.Find("PS_Dash").GetComponent<ParticleSystem>();*/
        if(PS_Dash == null || PS_Pivot == null){
            Debug.Log("Particule element not defined");
        }
    }

    public IEnumerator AngularCameraRotation(CameraDirection _Dir){
        int DirMultiplier = 0;
        float _elapsedTime = 0f, RotateZ_offset = 0f;
        switch (_Dir)
        {
            case CameraDirection.Right:
            DirMultiplier = 1;
            break;

            case CameraDirection.Left:
            DirMultiplier = -1;
            break;

            default:
                Debug.Log("Direction Invalid");
            break;
        }

        while(_elapsedTime < TimeToRotate){
            _elapsedTime += Time.deltaTime;
            
            RotateZ_offset = Mathf.Lerp(ObjectReferencer.Instance.Avatar_Object.GetComponent<MouseLook>().Z_Rotation, RotationDegree * DirMultiplier, _elapsedTime/TimeToRotate);
            ObjectReferencer.Instance.Avatar_Object.GetComponent<MouseLook>().Z_Rotation = RotateZ_offset;
            Camera_Transform.Rotate(Camera_Transform.rotation.x, Camera_Transform.rotation.y, Camera_Transform.rotation.z + (RotateZ_offset * DirMultiplier ));
            var avatar = ObjectReferencer.Instance.Avatar_Object;
            float Z = Mathf.Lerp(0, RotationDegree* DirMultiplier,
                _elapsedTime / TimeToRotate);
            print(_elapsedTime / TimeToRotate);
            avatar.transform.eulerAngles =
                new Vector3(avatar.transform.eulerAngles.x, avatar.transform.eulerAngles.y, Z);
            yield return null;
        }

        yield return null;
    }

    public IEnumerator ResetCameraAngle(){
        float _elapsedTime = 0f, RotateZ_offset = 0f;

        while(_elapsedTime < TimeToRotate){
            _elapsedTime += Time.deltaTime;

            RotateZ_offset = Mathf.Lerp(this.GetComponent<MouseLook>().Z_Rotation, 0f, _elapsedTime/TimeToRotate);
            this.GetComponent<MouseLook>().Z_Rotation = RotateZ_offset;
            yield return null;
        }
        yield return null;
    }

    public IEnumerator ChangeCameraFOVDuringDash(float _DashTime, Vector3 _DashDir){
        float _elapsedTime = 0f;
        //PS_Dash.time = _DashTime;
        //
        PS_Dash.transform.position = (ObjectReferencer.Instance.Avatar_Object.transform.position + _DashDir) * ParticuleSystemeDistanceFromPlayer;
        PS_Dash.transform.LookAt(Avatar_Object.transform);

        PS_Dash.Play();

        while(_elapsedTime < _DashTime){
            _elapsedTime += Time.deltaTime;


        }
        yield return null;
    }
}
