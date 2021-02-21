using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectReferencer : MonoBehaviour
{
    public static ObjectReferencer Instance;

    public GameObject Avatar_Object;

    private void Awake() {
        #region Singleton Instance
            if(Instance == null){
                Instance = this;
            }
            else{
                Destroy(this.gameObject);
            }
        #endregion

        Avatar_Object = GameObject.Find("Avatar");
    }
}
