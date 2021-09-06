using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[RequireComponent(typeof(CircleCollider2D))]
//public abstract class Champion : MonoBehaviour
//{
//    public Job job;
//    public Species species;

//    [HideInInspector]
//    public int Damage;
//    [HideInInspector]
//    public float CoolTime;
//    [HideInInspector]
//    protected float CoolTimedelta =0;
//    [HideInInspector]
//    public float Range;

//    public GameObject target;
//    [HideInInspector]
//    public bool IsAttacking=false;

//   // public abstract void Atk();
//    public abstract void FindEnemy();
//    public void OnDrawGizmos()
//    {
//        Gizmos.color = Color.red;
//        Gizmos.DrawWireSphere(transform.position, Range);
//    }


//}



/*
public class Novel : RangeChampion
{
    private void Awake()
    {
        Set((RangeJob)job, this);
    }

    private void Update()
    {
        if (CheckEnemyState())
        {
            Atk();
        }
        else
            FindEnemy();
    }

    //public override void Atk()
    //{
    //    GameObject go = Instantiate(Bullet, transform.position, Quaternion.identity);
    //    go.AddComponent<Bullet>().SetAll(Damage, job, species, BulletSpeed, target);
    //}

    public override void FindEnemy()
    {
        Collider2D collider2D=Physics2D.OverlapCircle(transform.position, Range,LayerMask.NameToLayer("Enemy"));
        if (collider2D != null)
            target = collider2D.gameObject;
        else
        {
            IsAttacking = false;
            target = null;
        }
    }
    bool CheckEnemyState()
    {
        //적 공격 가능한 상태인지 전체 판단   적 무적상태,이미 죽은 몬스터 등
        if (target != null)
        {
            if (target.GetComponent<EnemyHealth>().hp < 0)
                return false;
            if (Vector2.Distance(transform.position, target.transform.position) > Range)
                return false;
        }

        return true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsAttacking)
        {
            target = collision.gameObject;
            IsAttacking = true;
        }
    }
}

*/
