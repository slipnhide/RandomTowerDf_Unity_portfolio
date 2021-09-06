using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct TowerListPerRank
{
    public TowerListPerRank(GameObject[] gameObjects) { Towers = gameObjects; }
    public GameObject[] Towers;
}
public  class TowerList :MonoBehaviour
{
    static TowerList towerlist;
    public static TowerList towerList
    {
        get
        {
            #region getter
            if (towerlist==null)
            {
                TowerList find = FindObjectOfType<TowerList>();
                if (find != null)
                    towerlist = find;
                else
                    towerlist = new GameObject().AddComponent<TowerList>();
            }
            return towerlist;
            #endregion
        }
    }
    public TowerListPerRank[] TowerPerRank;
    private void Awake()
    {

        #region SingleToneWay2
        /*if (FindObjectsOfType<TowerList>().Length != 1)
        {
            Debug.LogError("TowerList is not One: TryObjectName : " + gameObject.name);
            
        (gameObject);
            return;
        }*/
        #endregion

        #region SingleToneWay1
        if (towerlist == null)
        {
            towerlist = this;
        }
        else if (towerlist != this)
        {
            Debug.LogError("towerList is not One: TryObjectNamee : " + gameObject.name);
            Destroy(gameObject);
            return;
        }
       // DontDestroyOnLoad(gameObject);
        #endregion
    }
    private void Start()
    {
        //타워 목록이 제대로 셋팅 되잇는지 확인 
        //for (int i = 0; i < TowerPerRank.Length; i++)
        //{
        //    TowerListPerRank TL = TowerPerRank[i];
        //    for (int k = 0; k < TL.Towers.Length; i++)
        //    {
        //        Tower tower = TL.Towers[k].GetComponent<Tower>();
        //    }
        //}
        #region 인스펙터가아닌코드로목록불러오기
        /*
        TowerPerRank = new TowerListPerRank[]
        {
            new TowerListPerRank(
                new GameObject[]
                {
                    Resources.Load<GameObject>(""),
                    Resources.Load<GameObject>(""),
                }),
                        new TowerListPerRank(
                new GameObject[]
                {
                    Resources.Load<GameObject>(""),
                    Resources.Load<GameObject>(""),
                })
        };*/
        #endregion
    }
    public GameObject FindTower(Champion _Tower)
    {
        TowerListPerRank TowerList = TowerPerRank[_Tower.RequiredGold];
        //Debug.Log(TowerList.Towers.Length);
        for (int i = 0; i < TowerList.Towers.Length; i++)
        {
          //  Debug.Log("i_:" + i + "  리스트 이름:" + TowerList.Towers[i].GetComponent<Tower>().Name + "  전달받은 타워 이름: " + _Tower.name);
          //  if (TowerList.Towers[i].GetComponent<Tower>().Name.Equals(_Tower.name))
          if (TowerList.Towers[i].name.Equals(_Tower.name))
                    return TowerList.Towers[i];
        }
        return null;
    }
}
