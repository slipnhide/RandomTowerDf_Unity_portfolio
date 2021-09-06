using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int Dmg;

    Job job;
    [SerializeField]
    int jobrank;
    [SerializeField]
    Species spiecies;
    [SerializeField]
    int spieciesrank;
    GameObject target;
    public float Speed = 1;
    public Champion owner;
    public void SetAll(int _Dmg,Job _job, Species _spiecies, float _Speed, GameObject _target)
    {
        Dmg = _Dmg;
        target = _target;
        job = _job;
        spiecies = _spiecies;
        Speed = _Speed;
    }
    public Bullet Set(GameObject _target) { target = _target; return this; }
    public Bullet JobSet(Job _job, int _jobrank) { job = _job; jobrank = _jobrank; return this; }
    public Bullet SpieciesSet(Species _spiecies, int _spieciesrank) { spiecies = _spiecies; spieciesrank = _spieciesrank; return this; }

    public Bullet SetSpeed(float _Speed) { Speed = _Speed; return this; }

    private void FixedUpdate()
    {
        if (target != null)
        {
            Vector2 dir = target.transform.position - transform.position;
            transform.Translate(dir.normalized * Speed * Time.deltaTime);
        }
        else
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (target == collision.gameObject)
        {
            EnemyHealth temp = collision.GetComponent<EnemyHealth>();
            if (temp == null) Debug.LogError("EnemyHealth null");
            else
            {
                temp.Synergy(job, spiecies, owner);
                temp.DamageIt(Dmg);
                Destroy(gameObject);
            }
        }
    }
}
//public class BoxBullet : MonoBehaviour
//{
//    public int Dmg;
//    Job job;
//    [SerializeField]
//    int jobrank;
//    [SerializeField]
//    Species spiecies;
//    [SerializeField]
//    int spieciesrank;

//    bool IsReUse = false;
//    public int MaxNum = 1;
//    int curnum = 0;
//    public float time;
//    public float _CurTime;
//    public void SetAll(int _Dmg, Job _job, Species _spiecies, float _Time, bool _IsReUse, int _MaxNum)
//    {
//        job = _job;
//        spiecies = _spiecies;
//        IsReUse = _IsReUse;

//        MaxNum = _MaxNum;
//        time = _Time;
//    }
//    public BoxBullet JobSet(Job _job, int _jobrank) { job = _job; jobrank = _jobrank; return this; }
//    public BoxBullet SpieciesSet(Species _spiecies, int _spieciesrank) { spiecies = _spiecies; spieciesrank = _spieciesrank; return this; }

//    private void Update()
//    {
//        _CurTime += Time.deltaTime;
//        if (_CurTime > time)
//        {
//            if (IsReUse) gameObject.SetActive(false);
//            else Destroy(gameObject);
//        }
//    }

//    private void FixedUpdate()
//    {
//    }

//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        Debug.Log("Enter");
//        EnemyHealth temp = collision.GetComponent<EnemyHealth>();
//        if (temp == null) Debug.LogError("EnemyHealth null");
//        else
//        {
//            temp.Synergy(job, spiecies);
//            temp.DamageIt();
//            curnum++;
//            if (curnum >= MaxNum)
//            {
//                if (IsReUse) gameObject.SetActive(false);
//                else Destroy(gameObject);
//            }
//        }

//    }
//    public void use()
//    {
//        gameObject.SetActive(true);
//        curnum = 0;
//        _CurTime = 0;
//    }
//    public void Off()
//    {
//        gameObject.SetActive(false);
//    }
//}