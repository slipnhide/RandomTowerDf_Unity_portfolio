using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCreater : MonoBehaviour
{
   public GameObject TestEnemy32;
    public GameObject TestEnemy64;
    Vector3[] WayPoints;
    public Transform[] WayPoint;
    private void Start()
    {
        int index = WayPoint.Length;
        WayPoints = new Vector3[index];
        for (int i = 0; i < index; i++)
        {
            WayPoints[i] = WayPoint[i].position;
        }
    }

    //public void Spwan32()
    //{
    //    Enemy enemy = Instantiate(TestEnemy32).GetComponent<Enemy>();
    //    enemy.Create(WayPoints);
    //    enemy.Test = true;
    //  //  Instantiate(TestEnemy32).GetComponent<Enemy>().Create(WayPoints);
    //}
    //public void Spwan64()
    //{
    //    Enemy enemy = Instantiate(TestEnemy64).GetComponent<Enemy>();
    //    enemy.Create(WayPoints);
    //    enemy.Test = true;
    //}


}
