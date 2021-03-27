using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectReferencer : MonoBehaviour
{
    public static ObjectReferencer Instance;

    public GameObject Avatar_Object;

    public GameObject Crossair_Object;

    #region Enemies Prefabs
        [Header("Enemy prefabs init by hand")]
        public GameObject TurretEnemy_prefab;
        public GameObject DummyEnemy_prefab;
        public GameObject ProsecutorEnemy_prefab;
        public GameObject SniperEnemy_prefab;
    #endregion

    private void Awake() {
        #region Singleton Instance
            if(Instance == null){
                Instance = this;
            }
            else{
                Destroy(this.gameObject);
            }
        #endregion

        Avatar_Object = GameObject.FindWithTag("Player");

        Crossair_Object = GameObject.Find("Crossair_container");
    }
}
