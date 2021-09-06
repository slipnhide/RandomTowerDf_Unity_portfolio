using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class NetworkManager : MonoBehaviourPunCallbacks
{
    static NetworkManager _NetworkManager;
    public static NetworkManager networkManager
    {
        get 
        {
            if (_NetworkManager == null)
            {
                _NetworkManager = FindObjectOfType<NetworkManager>();
                if (_NetworkManager == null)
                {
                    Debug.LogError("NetworkManager Is Notthing");
                }
              //  Debug.Log("networkManager getter: _"+_NetworkManager.name);
            }
            return _NetworkManager;
        }
    }

    public PhotonView Pv;
    public RoomUpdate roomUpdate;
    public static int MaxManInRoom { get { return 8; } }
    public static string BasePassWord = "666";

    public Text StatusText;
    public InputField NickNameInput;

    //public bool IsInRoom { get { return PhotonNetwork.InRoom; } }
    // public bool InLobby { get { return PhotonNetwork.InLobby; } }
    // public bool IsConnect { get { return PhotonNetwork.IsConnected; } }
    public bool IsConnect;
    public bool InLobby;
   // public bool IsInRoom;
    public bool RoomCreate;
    public int CurLobbyMansNum {get {return (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms);}}
    public int AllMans{get{return (PhotonNetwork.CountOfPlayers);}}

    public string NickName { get { if (PhotonNetwork.IsConnected) return PhotonNetwork.NickName; else return "Non"; } }
    public string RoomName { get { if (PhotonNetwork.InRoom) return PhotonNetwork.CurrentRoom.Name; else return "Non"; } }
    public int CurRoomMaxMans { get { if (PhotonNetwork.InRoom) return PhotonNetwork.CurrentRoom.MaxPlayers; else return 666; } }
    public int MansNumInRoom { get { if (PhotonNetwork.InRoom) return PhotonNetwork.CurrentRoom.PlayerCount; else return 666; } }
    // public int CurRoomPw { get { if (IsInRoom) return PhotonNetwork.CurrentRoom; else return 666; } }
    // public string CurRoomMaxMans { get { if (IsInRoom) return PhotonNetwork.CurrentRoom.MaxPlayers.ToString(); else return "?"; } }

    //이하 사용자 프로퍼티 
    public bool IsReady 
    { get 
        { return PhotonNetwork.IsMasterClient? true :
                (bool)PhotonNetwork.LocalPlayer.CustomProperties["Isready"]; }
        set {Pv.RPC(nameof(SetReady), RpcTarget.All, value, PhotonNetwork.NickName);}
    }
    [PunRPC]
    void SetReady(bool value,string name)
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if(player.NickName== name)
                player.CustomProperties["Isready"] = value;
        }
    }
    [SerializeField]
    public int ordinalInRoom { get{ return PhotonNetwork.InRoom ? (int)PhotonNetwork.LocalPlayer.CustomProperties[ordinalNumPropertyName] : -1;  }
        set { Pv.RPC(nameof(ordinalNum), RpcTarget.All, value, PhotonNetwork.NickName); }
    }
    [PunRPC]
    void ordinalNum(int value, string name)
    {
        foreach (Player player in PhotonNetwork.PlayerList)
            if (player.NickName == name)
                player.CustomProperties[ordinalNumPropertyName] = value;
    }
    [PunRPC]
    void ChgordinalNumInRoom(int NumThatPlayerHad)
    {
        if (ordinalInRoom > NumThatPlayerHad)
        {
            ordinalInRoom -= 1;
        }
    }



    public bool IsHost { get { return PhotonNetwork.IsMasterClient; } }
    public bool IsGaming { get { return (PhotonNetwork.InRoom) ? (bool)PhotonNetwork.CurrentRoom.CustomProperties["IsStart"] : false; } }

    public static string ordinalNumPropertyName = "ordinalNum";
    public static string ReadyPropertyName = "Isready";
    //Hashtable PlayerHashInRoom = new Hashtable() { { ReadyPropertyName, false } };
   // Hashtable RoomHash = new Hashtable() { { "IsStart", false } };


    private void Awake()
    {
       // Debug.LogError("NetworkManager is awake : " + gameObject.name);
        if (networkManager!=this)
        {
            {
               // Debug.LogError("NetworkManager is not One: TryObjectNamee : " + gameObject.name);
                 Destroy(gameObject);
               // gameObject.name = "D_NetworkManager";
                return;
            }
        }
       // Debug.LogError("NetworkManager is Passed : " + gameObject.name);
        DontDestroyOnLoad(gameObject);

        Screen.SetResolution(Screen.width, Screen.width * 9 / 16,true);
    }
    private void Update()
    {
      //  StatusText.text = PhotonNetwork.NetworkClientState.ToString();
    }

    public bool Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
        return PhotonNetwork.IsConnected;
    }
    public override void OnConnected()
    {
        Debug.Log("OnConnected");
        IsConnect = true;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { NetworkManager.ReadyPropertyName, false } ,{ ordinalNumPropertyName, -3 } });
        Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties[ordinalNumPropertyName]);
        NetworkPanelControl.networkPanelControl.ButtonDoDequeue(true);
        NetworkPanelControl.networkPanelControl.NickNameUpdate(PhotonNetwork.LocalPlayer.NickName);
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
    }
    public void DisConnect()
    {
        Debug.Log("DisConnect");
        PhotonNetwork.Disconnect();
        NetworkPanelControl.networkPanelControl.NickNameUpdate("Non");
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("cause");
        IsConnect = false;
    }
    public bool JoinLobby() { Debug.Log("JoinLobby"); return PhotonNetwork.JoinLobby(); }
    public override void OnJoinedLobby() { Debug.Log("LobbyIn"); InLobby = true; NetworkPanelControl.networkPanelControl.ButtonDoDequeue(true); }
    public bool LeaveLobby() { InLobby = false; return PhotonNetwork.LeaveLobby(); }
    public override void OnLeftLobby()
    {
        NetworkPanelControl.networkPanelControl.ButtonDoDequeue(true);
    }

    public bool JoinRandomRoom() { return PhotonNetwork.JoinRandomRoom(); }

    public bool CreateRoom(string Name=null, int Max=-1) 
    {if (Name == null)
            Name = Random.Range(0, 100).ToString();
        if (Max == -1)
            Max = MaxManInRoom;
        else
        Max =Mathf.Clamp(Max, 0, MaxManInRoom);

        Hashtable RoomHash = new Hashtable() { { "IsStart", false } };
        string[] RoomHashForLobby = new string[]{ "IsStart" };

        RoomOptions roomOptions=new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.MaxPlayers = (byte)Max;
        roomOptions.CustomRoomPropertiesForLobby = RoomHashForLobby;
        roomOptions.CustomRoomProperties = RoomHash;

     return PhotonNetwork.CreateRoom(Name, roomOptions, null); 
    }
    //public bool JoinOrCreateRoom() { return PhotonNetwork.JoinOrCreateRoom(roomInput.text, new RoomOptions { MaxPlayers = (byte)MaxManInRoom }, null); }
    public bool JoinRoom(string name=null) 
    {
        //if (name != null)
        //{
        //    if (roomUpdate.RoomIsStart(NetworkPanelControl.DecordingRoomName(name)[0]))
        //    {
        //        Debug.LogError("시작중이라 진입 불가");
        //        return false;
        //    }
        //}

        return PhotonNetwork.JoinRoom(name); 
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LocalPlayer.CustomProperties[ReadyPropertyName] = false;
        ordinalInRoom = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log("OnJoinedRoom");
        NetworkPanelControl.networkPanelControl.ButtonDoDequeue(true);
        //networkPanelControl에서 RoomRenewal콜백
        RoomUpdate();
    }
    [PunRPC]
    void RoomUpdate()
    {
        NetworkPanelControl.networkPanelControl.EnterRoom();
    }
    public void ReadyToGame() 
    {
        if (PhotonNetwork.IsMasterClient)
        {
           // bool IsStart = true;
            Debug.Log("IsMasterClient");
            string MasterName = PhotonNetwork.LocalPlayer.NickName;
            Hashtable hashtable;
            foreach (Player player in PhotonNetwork.PlayerListOthers)
            {
                //if (player.NickName == MasterName) continue;
                //Debug.Log(player.NickName + player.CustomProperties[ReadyPropertyName]);
                hashtable = player.CustomProperties;
                if (!(bool)hashtable[ReadyPropertyName])
                {
                    return;
                }
            }
            Debug.Log("시작");
            Pv.RPC(nameof(StartGame), RpcTarget.All);
            PhotonNetwork.LoadLevel(1);
        }
        else
        {
            //value = (bool)PhotonNetwork.LocalPlayer.CustomProperties[ReadyPropertyName];
            //Debug.Log(value);
            //value = !value;
            //PhotonNetwork.LocalPlayer.CustomProperties[ReadyPropertyName] = value;
            //IsReady = value;
            //Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties[ReadyPropertyName]);
            IsReady= !(bool)PhotonNetwork.LocalPlayer.CustomProperties[ReadyPropertyName];
            Pv.RPC("RoomUpdate", RpcTarget.All);
        }
    }
    [PunRPC]
    public void StartGame()
    {
        //NetworkPanelControl.networkPanelControl.CurPanelType = PanelType.Gaming;
        //NetworkPanelControl.networkPanelControl.mediate();
        Debug.Log("StartGame");
        NetworkPanelControl.networkPanelControl.SuverCanvas.SetActive(false);
        PhotonNetwork.CurrentRoom.CustomProperties["IsStart"] = true;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        Debug.Log("StartGame()");
        // Pv.RPC(nameof(GameSet), RpcTarget.All);
        GameSet();
    }
    void GameSet()
    {
       GetComponent<PlayerInfo_Master>().enabled = true;
        Invoke("Insert", 1);
    }
    void Insert()
    {
      GameObject temp=PhotonNetwork.Instantiate("UiPreFab/InGame/IngameManager", Vector3.zero, Quaternion.identity);
       //temp.GetComponent<IngameNetworManager>().
       // Set(new PlayerInfo_Net(PhotonNetwork.NickName, (int)PhotonNetwork.LocalPlayer.CustomProperties[ordinalNumPropertyName]),PhotonNetwork.CurrentRoom.PlayerCount);
    }


    public void EndGame()
    {
        Hashtable hashtable = PhotonNetwork.LocalPlayer.CustomProperties;
        hashtable[ReadyPropertyName] = false;
        //NetworkPanelControl.networkPanelControl.CurPanelType = PanelType.Room;
        //NetworkPanelControl.networkPanelControl.mediate();
        NetworkPanelControl.networkPanelControl.SuverCanvas.SetActive(true);
        // RoomInfo roomInfo = PhotonNetwork.CurrentRoom;
        //현제 photonView 가 붙어있는상태로 씬 전환시 오브젝트 삭제되는 문제 발견됨 임시 방책
        GetComponent<PlayerInfo_Master>().enabled = false;
        Destroy(Pv);
      //  Pv.enabled = false;
        // SceneManager.LoadScene("Login");
        PhotonNetwork.LoadLevel(0);
        //Pv.enabled = true;
         Pv=gameObject.AddComponent<PhotonView>();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        NetworkPanelControl.networkPanelControl.UpdateRoom();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Pv.RPC(nameof(ChgordinalNumInRoom), RpcTarget.All, otherPlayer.CustomProperties[ordinalNumPropertyName]);
        NetworkPanelControl.networkPanelControl.UpdateRoom();
    }
    public string[] GetNickNamesInRoom()
    {
        int curLenght = PhotonNetwork.PlayerList.Length;
        string[] strings = new string[curLenght];
        for (int i = 0; i < curLenght; i++)
        {
            strings[i] = PhotonNetwork.PlayerList[i].NickName;
        }
        return strings;
    }
    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom()");
        RoomCreate = false;
        PhotonNetwork.LocalPlayer.CustomProperties[ReadyPropertyName] = false;
        ordinalInRoom = -1;
        NetworkPanelControl.networkPanelControl.ButtonDoDequeue(true);
        NetworkPanelControl.networkPanelControl.LeaveRoom();
        //networkPanelControl.ButtonDo(ToSeverCommand.MakeCommand(this, ToseverButtonDo.SelLobby));
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        RoomCreate = false;
        NetworkPanelControl.networkPanelControl.ButtonDoDequeue(false);
        Debug.Log(message);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        NetworkPanelControl.networkPanelControl.ButtonDoDequeue(false);
        Debug.Log(message);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        NetworkPanelControl.networkPanelControl.ButtonDoDequeue(false);
        Debug.Log(message);
    }

    public bool LeaveRoom() { return PhotonNetwork.LeaveRoom(); }

    [ContextMenu("정보")]
    void Info()
    {
        Debug.Log("Cur Name" + PhotonNetwork.NickName);
        Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties[ReadyPropertyName]);
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Cur Room Name" + PhotonNetwork.CurrentRoom.Name);
            Debug.Log("Cur Room PersonNum" + PhotonNetwork.CurrentRoom.PlayerCount);
            Debug.Log("Cur Rroom MaxPerson" + PhotonNetwork.CurrentRoom.MaxPlayers);

            string PlayerStr = "Cur Player Name: ";
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) PlayerStr += PhotonNetwork.PlayerList[i].NickName;
            Debug.Log(PlayerStr);
        }
        else
        {
            Debug.Log("Cur Server Man : " + PhotonNetwork.CountOfPlayers);
            Debug.Log("Cur Room Num : " + PhotonNetwork.CountOfRooms);
            Debug.Log("Every Man In All Rooms : " + PhotonNetwork.CountOfPlayersInRooms);
            Debug.Log("Are u In Lobby : " + PhotonNetwork.InLobby);
            Debug.Log("Are u InConnect : " + PhotonNetwork.IsConnected);
        }
    }
    //public override void OnRoomListUpdate(List<RoomInfo> roomList)
    //{
    //    foreach (RoomInfo roomInfo in roomList)
    //        Debug.Log(roomInfo.Name);
    //}
}
