using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[System.Serializable]
public class PlayerInfo_Net
{
    public PlayerInfo_Net(string _name, int _number)
    {
        name = _name;
        Number = _number;
    }

    public string name;
    public int Number;
    public int Gold=400;
    public int Lv;
}

public class IngameNetworManager : MonoBehaviour
{
    PhotonView Pv;
   public PlayerInfo_Net playerInfo_Net;
    public int i = 0;
    [Header("GameStart")]
    public GameObject Grid;
    private void Awake()
    {
        Pv = GetComponent<PhotonView>();
            Init();
    }
    void Init()
    {
        Debug.Log("Init");
        if (Pv.IsMine)
        {
            playerInfo_Net.name = PhotonNetwork.LocalPlayer.NickName;
            playerInfo_Net.Number = NetworkManager.networkManager.ordinalInRoom;
            gameObject.name = playerInfo_Net.name;
            Vector3 instancePos = new Vector3(playerInfo_Net.Number * 30, 0, 0);
            Camera.main.transform.position = instancePos + new Vector3(0, 0, -10);
            Transform tiles=PhotonNetwork.Instantiate("Map", instancePos, Quaternion.identity).transform.GetChild(0);
            Transform[] transforms = tiles.GetComponentInParent<WayPoints>().wayPoints;
            int ChildNum = tiles.childCount;
            EnemySpawner enemySpawner = GameObject.Find("PlayerManager").GetComponent<EnemySpawner>();
          //  enemySpawner.WayPoints = 
                //transforms;
            for (int index = 0; index < ChildNum; index++)
            {
                tiles.GetChild(index).tag = "MyTile";
            }

        }
        else
        {
            playerInfo_Net.name = Pv.Owner.NickName;
            playerInfo_Net.Number = (int)Pv.Owner.CustomProperties[NetworkManager.ordinalNumPropertyName];
            gameObject.name = playerInfo_Net.name;
        }
    }



    public void Set(PlayerInfo_Net _playerInfo_Net, int PlayerNum)
    {
        Pv.RPC(nameof(SetNameRpc), RpcTarget.AllBuffered, _playerInfo_Net.name);
        Pv.RPC(nameof(SetNumRpc), RpcTarget.AllBuffered, _playerInfo_Net.Number);
        Debug.Log(PlayerNum);
    }
    [PunRPC]
    void SetNameRpc(string name)
    {
        playerInfo_Net.name = name;
        gameObject.name = name;
    }
    [PunRPC]
    void SetNumRpc(int _playerInfo_Net)
    {
        playerInfo_Net.Number = _playerInfo_Net;
    }
}
