using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public enum ToseverButtonDo {Exit,Login,Logout, SelLobby,LeaveLobby,  
    CreateRoom, JoinRandomRoom, JoinRoom, LeaveRoom , Room, ReadyOrStart, GameEnd}
public enum PanelType { Login, SelLobby , Selroom , Room , Chat, Gaming}
interface Notify{ void ModeChage(PanelType serverDeep); }

public class MakeRoomInfo
{
    public string name;
    public string Password;
    public int MaxNumber;
    public  MakeRoomInfo(string _Roomname = null, string _RoomPw = null, int _MaxNumber=-1)
    {
        name = _Roomname;
        Password = _RoomPw;
        MaxNumber = _MaxNumber;
    }
}

public class NetworkPanelControl : MonoBehaviour
{
    static NetworkPanelControl _networkPanelControl;
    public GameObject SuverCanvas;
    public static NetworkPanelControl networkPanelControl
    {
        get
        {
            if (_networkPanelControl == null)
            {
                _networkPanelControl = FindObjectOfType<NetworkPanelControl>();
                if (_networkPanelControl == null)
                {
                    Debug.LogError("NetworkPanelControl Is Notthing");
                }
            }
            return _networkPanelControl;
        }
    }
   // public NetworkManager networkManager;
    public PanelType CurPanelType = PanelType.Login;
    [Header("OnOffPanels")]
    public SurverPanel[] surverPanels;
   // public GameObject ChatObject;
    public GameObject WaitPanel;
    [Space]
    [Header("MakeRoomInfo")]
    public InputField makeRoomnameInput;
    public InputField makeRoomPw;
    public Dropdown MakeMaxMansInRoom;

   // public InputField JoinRoomName;
    //public InputField JoinRoomPw;
    [Space]

    [Header("CurRoomInfo")]
    public Text NickName;
    public Text RoomName;
    public Text Pw;
    public Text MansNumInRoom;
    public Transform playerPanelParent;
    public GameObject PlayerPanelPreFab;
    [Space]
    public OutButton outButton = new OutButton();
    Queue<ToSeverCommand> toSeverCommands = new Queue<ToSeverCommand>();

    [HideInInspector]
    public MakeRoomInfo makeRoomInfo
    {
        get
        {
            if(makeRoomPw == null)
            Debug.Log(makeRoomPw.text);
            return new MakeRoomInfo((makeRoomnameInput == null) ? null : (makeRoomnameInput.text == "" ? null : makeRoomnameInput.text),
                   (makeRoomPw == null) ? null : (makeRoomPw.text == "" ? null : makeRoomPw.text),
                  MakeMaxMansInRoom.value);
        }
    }
    private void Awake()
    {
        if (networkPanelControl!=this)
        {
            Destroy(gameObject);
            return;
        }
       DontDestroyOnLoad(gameObject);
        notify();
        

    }
    private void Start()
    {

    }
    public void Pop() 
    {
        ToSeverCommand Out_toSeverCommand;
        if (outButton.IsNonNull)
        {
            Out_toSeverCommand = outButton.Pop();
            ButtonDo(Out_toSeverCommand);
        }

    }
    public void notify() 
    {
        foreach (Notify notify in surverPanels) notify.ModeChage(CurPanelType); 
    }
    public bool ButtonDo(ToSeverCommand _toSeverCommand)
    {
        if (_toSeverCommand == null) Debug.LogError("버튼역활 미정:" + _toSeverCommand);
        //중복 방지를 위한 부분
        //추가 작업 필요 한부분
        //현제는 이전 함수와 현제 함수 이름이 같은지만 확인한다
        //예상되는 문제 대기중에 여러 버튼을 누른다면 콜백이 불가능한 함수들이 존재
        //ex) 방을 만드는 버튼을 누르고 대기중 -> 랜덤 조인 버튼 -> 방 만드는 버튼 누름 -> 랜덤 조인 버튼
        //작업 큐에는 다음과 같이 저장됨 방 만들기- 랜덤조인- 방만들기 -랜덤조인
        //하지만 네트워크매니저에서는 방이 만들어 지는게 한번일 뿐이므로 OnjoinRoom 이 한번 호출되므로 큐에는 쓸대 없는 작업이 존재하게되고
        //다른작업으로 인해서 디큐가 되고 해당 다른 작업은 또 다시 다른 버튼을 누른뒤에야 디큐가 되는 현상이 발생함
        //해당 문제는 게임 내내 발생할것이고 백 버튼에도 영향을 주게됨

        //예상 해결방안 큐 받을때 같은 종류의 혹은 같이 대기하면 안되는 역활은 큐에 삽입 하지 않음
        //종류의 정함은 콜백되는 종류에 따라 구분 위의 예시와 같이 OnJoinRoom 으로 콜백되는 함수끼리는 겹치지 않게 함
        //단, 콜백이 되지 않아서 자체디큐 하게 되는 함수는 제외
        //내부 검사하는데 큐가 방해라면 리스트로 바꾸고 큐형태로 사용
        if (toSeverCommands.Count > 0 && toSeverCommands.Peek().ToString() == _toSeverCommand.ToString())
            return false;


        if (_toSeverCommand.Excute())
        {
            toSeverCommands.Enqueue(_toSeverCommand);
            //만약 대기 큐가 방금 들어온 작업 뿐이고 해당 큐가 콜백 될 함수가 아니라면
            //자체적으로 디큐하게만듬
            //해당 사항이 없으면 콜백 될 일이 없기 때문에 무기한 보류됨
            if (!_toSeverCommand.IsCallBack && toSeverCommands.Count == 1)
            {
                ButtonDoDequeue(true);
                return true;
            }
            WaitPanel.SetActive(true);
            return true;
        }
        else
            //네트워크에서 무시됬다면 처리 안함
            return false;
    }
    public void ButtonDoDequeue(bool RequestReSult)
    {
        if (toSeverCommands.Count <= 0) return;
        WaitPanel.SetActive(false);
        ToSeverCommand toSeverCommand = toSeverCommands.Dequeue();
        //서버에 부탁한 일이 처리가 됬다면 백 버튼에 나가는 명령 푸쉬, 이후 화면 전환
        if (RequestReSult)
        {
            ToSeverCommand OuttoSeverCommand = toSeverCommand.Out();
            //다음 작업이 뒤로가기 역활이있는 버튼이라면 뒤로가기버튼에 Out 클래스
            if (OuttoSeverCommand != null)
                outButton.Push(OuttoSeverCommand);
            //판넬 교환작업이 있는 함수라면 
            if (toSeverCommand.Chg_Panel)
            {
                CurPanelType = toSeverCommand.nextPanel;
                notify();
            }
        }
        //다음 작업이 콜백될 작업이 아니라면 자동으로 재귀호출
        if (toSeverCommands.Count > 0)
            if (!toSeverCommands.Peek().IsCallBack)
            ButtonDoDequeue(true);
    }

    public void NickNameUpdate(string name) { NickName.text = name; }


    public void EnterRoom()
    {
        NickName.gameObject.SetActive(false);
        string[] nameNPw = NetworkPanelControl.DecordingRoomName(NetworkManager.networkManager.RoomName);
        RoomName.text = nameNPw[0];
        Pw.text = nameNPw[1];
        {
            /*
            string Name = networkManager.RoomName;
            char[] NameArray = Name.ToCharArray();
            int Where_ = 0;
            for (int i = NameArray.Length - 1; i > 0; i--)
            {
                if (NameArray[i] == '_')
                {
                    Where_ = i;
                    break;
                }
            }
            char[] name = new char[Where_];
            for (int i = 0; i < Where_; i++)
            {
                name[i] = NameArray[i];
            }
            char[] PwArray = new char[NameArray.Length - 1 - Where_];
            int index = 0;
            for (int i = Where_ + 1; i < NameArray.Length; i++)
            {
                PwArray[index] = NameArray[i];
                index++;
            }
            RoomName.text = new string(name);
            Pw.text = (new string(PwArray).Equals(NetworkManager.BasePassWord)) ? "Open" : new string(PwArray);
            */
        }
        //이름과 최대인원이 바뀌는 경우는 별도 함수 구현뒤 networkManager 에서 콜백구현?
        //RoomName.text = networkManager.RoomName;
        UpdateRoom();
        //Pw.text= networkManager.

    }
    public void UpdateRoom()
    {
        Player[] players = PhotonNetwork.PlayerList;
        //string[] strings = networkManager.GetNickNamesInRoom();
       // MansNumInRoom.text = strings.Length.ToString()+" / "+ networkManager.CurRoomMaxMans.ToString();
        MansNumInRoom.text = players.Length.ToString() + " / " + NetworkManager.networkManager.CurRoomMaxMans.ToString();
        GameObject temp;
        for (int i = 0; i < NetworkManager.networkManager.CurRoomMaxMans; i++)
        {
            //플레이어 정보 패널 지정 or 생성
            if (playerPanelParent.childCount <= i)
                temp = Instantiate(PlayerPanelPreFab, playerPanelParent);
            else
                temp = playerPanelParent.GetChild(i).gameObject;

            //if (i < strings.Length)
            //{
            //    temp.GetComponentInChildren<Text>().text = strings[i];
            //}
            if (i < players.Length)
            {
                temp.GetComponentInChildren<Text>().text = players[i].NickName;
                if ((bool)players[i].CustomProperties[NetworkManager.ReadyPropertyName])
                    temp.GetComponentInChildren<Text>().text += "_Ready";
            }
            else
                temp.GetComponentInChildren<Text>().text = "";

        }
    }
    public void LeaveRoom()
    {
        if(NickName!=null)
        NickName.gameObject.SetActive(true);
    }
    public static string EncordingRoomName(string RealRoomName, string RealPw=null)
    {
        string Name = RealRoomName;
        if (RealPw != null)
            Name += "_" + RealPw;
        else
            Name += "_" + NetworkManager.BasePassWord;
        return Name;
    }

    public static string[] DecordingRoomName(string _name)
    {
        string Name = _name;
        char[] NameArray = Name.ToCharArray();
        int Where_ = 0;
        for (int i = NameArray.Length - 1; i > 0; i--)
        {
            if (NameArray[i] == '_')
            {
                Where_ = i;
                break;
            }
        }
        char[] name = new char[Where_];
        for (int i = 0; i < Where_; i++)
        {
            name[i] = NameArray[i];
        }
        char[] PwArray = new char[NameArray.Length - 1 - Where_];
        int index = 0;
        for (int i = Where_ + 1; i < NameArray.Length; i++)
        {
            PwArray[index] = NameArray[i];
            index++;
        }
        string Password = new string(PwArray);
        return new string[] { new string(name), NetworkPanelControl.CheckUnLockRoom(Password) ? "Open" : Password };
    }
    public static bool CheckUnLockRoom(string Pw) { return Pw.Equals(NetworkManager.BasePassWord) ? true : false; }
}

public class OutButton
{
   // Stack<ServerButtonDo> serverInOuts = new Stack<ServerButtonDo>();
    Stack<ToSeverCommand> toSeverCommands = new Stack<ToSeverCommand>();
    public bool IsNonNull { get { if (toSeverCommands.Count > 0) return true; else return false; } }

    //public void Push(ServerButtonDo _serverButtonDo, ServerDeep serverDeep)
    //{
    //    serverInOuts.Push(_serverButtonDo);
    //    serverDeeps.Push(serverDeep);
    //}
    public void Push(ToSeverCommand _toSeverCommand)=>toSeverCommands.Push(_toSeverCommand);
    //public ServerDeep Pop2()
    //{
    //    serverInOuts.Pop().ServerOut();
    //    return serverDeeps.Pop();
    //}
    public ToSeverCommand Pop()
    {
        return toSeverCommands.Pop();
    }
    public void ErazerLeaveLobby()
    {
    }
}


