using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class TowerSpawner : MonoBehaviour
{
    //public static TowerSpawner towerSpawner;
    SynergySystem synergySystem;
    GameObject Tower;
    private void Awake()
    {
        synergySystem = GetComponent<SynergySystem>();
        //if (towerSpawner == null)
        //{
        //    towerSpawner = this;
        //}
        //else if (towerSpawner != this)
        //{
        //    Debug.LogError("towerSpawner is not One" + gameObject.name);
        //}

        //  Mcamera = Camera.main;
    }
    // Update is called once per frame
    void Update()
    {
    }

  // public bool TowerSpawn(Tower _tower,RaycastHit _hit)
    public bool TowerSpawn(Champion _tower, RaycastHit2D _hit)
    {
        //생성검사,기타처리와 호출되는부분
        Tower = TowerList.towerList.FindTower(_tower);
        if (Tower != null)
        {
            {
                // Debug.Log("충돌");
                if (_hit.transform.CompareTag("MyTile"))
                {
                    //Debug.Log("타일");
                    if (!_hit.transform.GetComponent<Tile>().IsBuildTower)
                    {
                        _hit.transform.GetComponent<Tile>().IsBuildTower = true;
                        Create(Tower, _hit.transform.position);
                       // synergySystem.AddTower(Tower.GetComponent<Champion>());
                        Tower = null;
                        return true;
                    }
                }
            }
            
        }
        else
            Debug.LogError("방금 설치하려한 타워를 타워리스트에서 찾을수 없습니다.");
        Tower = null;
        return false;
    }

    //실제 타워가 생성되는 부분
    void Create(GameObject _tower,Vector3 Pos)
    {
        
        // Instantiate(_tower, Pos, Quaternion.identity);
        PhotonNetwork.Instantiate("Towers/"+_tower.name, Pos, Quaternion.identity);
    }
}
