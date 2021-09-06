using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAtk : MonoBehaviour
{
    GameObject atkPos;
    public GameObject bullet;

    void Start()
    {
        atkPos = transform.GetChild(0).gameObject;
        InvokeRepeating("Attack", 0.1f, 1.5f);
    }

    void Attack()
    {
        Instantiate(bullet, atkPos.transform.position, Quaternion.identity);
    }

    void Update()
    {
        
    }
}


/*
 * 적 스폰시 타워가 총알 쏨
 * 충돌, 데미지, 체력, 파괴 구현하기
*/

