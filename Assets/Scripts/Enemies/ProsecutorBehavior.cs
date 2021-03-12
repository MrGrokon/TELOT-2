using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProsecutorBehavior : MonsterBehavior
{
    public float DistanceMarginToTarget = 2f;

    public float HeightAmplitudeMul = 3f;

    [Header("Shoot Parameters")]
    [Range(1f, 15f)]
    public float TimeBetweenShoots = 5f;
    private float _elapsedAttackTime = 0f;
    public float AttackRange = 10f;

    public GameObject projectilePrefab;
    public float projectileSpeed = 30f;
    public int NumberOfBulletGenerated = 5;
    public float AmountOfSpread = 30f;

    #region Unity Functions
    override public void Update() {
        Vector3 _translateVector;
        #region Motion
        if (Vector3.Distance(this.transform.position, ObjectReferencer.Instance.Avatar_Object.transform.position) > DistanceMarginToTarget) 
           {
               Debug.Log("not close enought to player");
               
                //get the direction to the player
                _translateVector = ObjectReferencer.Instance.Avatar_Object.transform.position - this.transform.position;
           }
           else
           {
               Debug.Log("close to player");
            _translateVector = Vector3.zero;
           }

        //nullify the vertical vector
        _translateVector.y = Mathf.Sin(Time.timeSinceLevelLoad) * HeightAmplitudeMul;
        this.transform.Translate((_translateVector * MotionSpeed) * Time.deltaTime, Space.World);
        #endregion

        this.transform.LookAt(ObjectReferencer.Instance.Avatar_Object.transform);
        _elapsedAttackTime += Time.deltaTime;
        if (_elapsedAttackTime >= TimeBetweenShoots && Vector3.Distance(this.transform.position, ObjectReferencer.Instance.Avatar_Object.transform.position) <= AttackRange)
        {
            _elapsedAttackTime = 0f;
            Attack();
        }
    }
    #endregion

    void Attack()
    {
        Debug.Log("prosecutor attack");
        GameObject _proj = Instantiate(projectilePrefab, this.transform.position, Quaternion.identity);
        _proj.GetComponent<EnemieProjectileBehavior>().SetSpeed(projectileSpeed);
    }
}
