using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
enum WhatMakeCallBack{OnConnect, On }

public abstract class ToSeverCommand
{
    PanelType NextPanel;
    [HideInInspector]
    bool chg_Panel;
    public bool Chg_Panel { get { return chg_Panel; } }
    [HideInInspector]
    public bool IsCallBack { get { return isCallBack; } }
    bool isCallBack;

    //UseOut 은 Out으로 있는 함수를 쓰는지 안쓰는지 안쓴다면 일반적인 전략패턴
    //Use_NextDeep 화면전환이 필요한 함수인지 
    public PanelType nextPanel { get { return NextPanel; } }
    public ToSeverCommand( PanelType _NextPanel, bool _Chg_Panel , bool _IsCallBack)
    {NextPanel = _NextPanel; chg_Panel = _Chg_Panel; isCallBack = _IsCallBack; }
    public static ToSeverCommand MakeCommand(ToseverButtonDo CurButtonDo,Text[] texts=null)
    {
        switch (CurButtonDo)
        {
            case ToseverButtonDo.Login:
                    return new Login(PanelType.SelLobby, true,true);
            case ToseverButtonDo.Logout:
                return new Logout(PanelType.Login, true,false);
            case ToseverButtonDo.SelLobby:
                return new SelLobby(PanelType.Selroom, true,true);
            case ToseverButtonDo.LeaveLobby:
                return new LeaveLobby(PanelType.Login, true,true);
            case ToseverButtonDo.CreateRoom:
                return new MakeRoom(PanelType.Room, true,true);
            case ToseverButtonDo.JoinRandomRoom:
                return new RandomJoin_Room(PanelType.Room, true, true);
            case ToseverButtonDo.JoinRoom:
                return new Join_Room(PanelType.Room, true, true, texts);
            case ToseverButtonDo.LeaveRoom:
                return new LeaveRoom(PanelType.SelLobby, true, true);
            case ToseverButtonDo.ReadyOrStart:
                return new ReadyOrStart(PanelType.Room, false, false);
            case ToseverButtonDo.GameEnd:
                return new GameEnd(PanelType.Room, false, false);
        }
        return null;
    }

    public abstract bool Excute();
    public abstract ToSeverCommand Out();
}
public class GameEnd : ToSeverCommand
{
    public GameEnd(PanelType NextPanelType, bool ChgPanel, bool _IsCallBack) :
        base(NextPanelType, ChgPanel, _IsCallBack)
    { }

    public override bool Excute()
    {
        NetworkManager.networkManager.EndGame();
        return true;
    }

    public override ToSeverCommand Out()
    {
        return null;
    }
}
public class Login : ToSeverCommand
{
    public Login(PanelType NextPanelType, bool ChgPanel,bool _IsCallBack) : 
        base(NextPanelType, ChgPanel, _IsCallBack) { }

    public override bool Excute()
    {
        return NetworkManager.networkManager.Connect();
    }

    public override ToSeverCommand Out()
    {
        return ToSeverCommand.MakeCommand(ToseverButtonDo.Logout);
    }
}
public class Logout : ToSeverCommand
{
    public Logout(PanelType NextPanelType, bool ChgPanel,bool _IsCallBack) : 
        base(NextPanelType, ChgPanel, _IsCallBack) 
    { }

    public override bool Excute()
    {
        NetworkManager.networkManager.DisConnect();
        return true;
    }

    public override ToSeverCommand Out()
    {
        return null;
    }
}
public class SelLobby : ToSeverCommand
{
    public SelLobby(PanelType NextPanelType, bool ChgPanel, bool _IsCallBack) :
        base(NextPanelType, ChgPanel, _IsCallBack)
    { }

    public override bool Excute()
    {
        return NetworkManager.networkManager.JoinLobby();
    }

    public override ToSeverCommand Out()
    {
        //현제 문제: 플레이어가 방에서 나가기위해 LeaveRoom 을 시행하면 로비까지 자동으로 나가지는 일이 발생함
        //방에서 나갈때 큐에서 LeaveLobby를 없에거나 로비까지 자동으로 나가는 일을 없애야함
        // return ToSeverCommand.MakeCommand(networkManager, ToseverButtonDo.LeaveLobby);
        return null;
    }
}
public class LeaveLobby : ToSeverCommand
{
    public LeaveLobby(PanelType NextPanelType, bool ChgPanel, bool _IsCallBack) :
        base(NextPanelType, ChgPanel, _IsCallBack)
    { }


    public override bool Excute()
    {
        return NetworkManager.networkManager.LeaveLobby();
    }

    public override ToSeverCommand Out()
    {
        return null;
    }
}
public class MakeRoom : ToSeverCommand
{

    //현제 텍스트를 받아서 하는 형식을 안하고 있음- 팩토리 메소드에서도 text는 전달 안하는중
    public MakeRoom(PanelType NextPanelType, bool ChgPanel, bool _IsCallBack, Text[] texts=null) :
        base(NextPanelType, ChgPanel, _IsCallBack)
    { }

    public override bool Excute()
    {
        MakeRoomInfo makeRoomInfo = NetworkPanelControl.networkPanelControl.makeRoomInfo;
        int MaxNum = NetworkManager.MaxManInRoom - makeRoomInfo.MaxNumber;
        string Name=

        NetworkPanelControl.EncordingRoomName(makeRoomInfo.name, makeRoomInfo.Password);
        return NetworkManager.networkManager.CreateRoom(Name, MaxNum);
    }
    public override ToSeverCommand Out()
    {
        return ToSeverCommand.MakeCommand(ToseverButtonDo.LeaveRoom);
    }
}
public class Join_Room : ToSeverCommand
{
    Text[] Texts;
    string[] strings;
    bool UseTexts;
    public Join_Room(PanelType NextPanelType, bool ChgPanel, bool _IsCallBack, Text[] texts) :
        base(NextPanelType, ChgPanel, _IsCallBack)
    {
        Texts = texts;
        UseTexts = true;
    }
    public Join_Room(PanelType NextPanelType, bool ChgPanel, bool _IsCallBack, string[] texts) :
        base( NextPanelType, ChgPanel, _IsCallBack)
    {
        UseTexts = false;
        strings = texts;
    }
    public override bool Excute()
    {
        if (UseTexts)
            return UseText();
        else
           return UseString();
    }
    public bool UseText()
    {
       // MakeRoomInfo JoinRoomInfo = NetworkPanelControl.networkPanelControl.JoinRoomInfo;
        string Name = Texts[0].text;
        if (Texts[1] != null && Texts[1].text != "")
            Name += "_" + Texts[1].text;
        else
            Name += "_" + NetworkManager.BasePassWord;
        Debug.Log("접속하고자 하는 방이름: " + Name);
        return NetworkManager.networkManager.JoinRoom(Name);
    }
    public bool UseString()
    {
      //  MakeRoomInfo JoinRoomInfo = NetworkPanelControl.networkPanelControl.JoinRoomInfo;
        string Name = strings[0];
        if (strings.Length>1 && strings[1] != "")
            Name += "_" + strings[1];
        else
            Name += "_" + NetworkManager.BasePassWord;
        Debug.Log("접속하고자 하는 방이름: " + Name);
        return NetworkManager.networkManager.JoinRoom(Name);
    }

    public override ToSeverCommand Out()
    {
        return ToSeverCommand.MakeCommand(ToseverButtonDo.LeaveRoom);
    }
}
public class RandomJoin_Room : ToSeverCommand
{
    public RandomJoin_Room(PanelType NextPanelType, bool ChgPanel, bool _IsCallBack) :
        base(NextPanelType, ChgPanel, _IsCallBack)
    { }

    public override bool Excute()
    {
        return NetworkManager.networkManager.JoinRandomRoom();
    }

    public override ToSeverCommand Out()
    {
        return ToSeverCommand.MakeCommand(ToseverButtonDo.LeaveRoom);
    }
}
public class LeaveRoom : ToSeverCommand
{
    public LeaveRoom(PanelType NextPanelType, bool ChgPanel, bool _IsCallBack) :
        base(NextPanelType, ChgPanel, _IsCallBack)
    { }

    public override bool Excute()
    {
        return NetworkManager.networkManager.LeaveRoom();
    }

    public override ToSeverCommand Out()
    {
        return null;
    }
}
public class ReadyOrStart : ToSeverCommand
{
    public ReadyOrStart(PanelType NextPanelType, bool ChgPanel, bool _IsCallBack) :
        base( NextPanelType, ChgPanel, _IsCallBack)
    { }

    public override bool Excute()
    {
        NetworkManager.networkManager.ReadyToGame();
        return true;
    }

    public override ToSeverCommand Out()
    {
        return null;
    }
}

public class ToSeverButton : MonoBehaviour
{
    ToSeverCommand toSeverCommand;
    public ToseverButtonDo toseverButton;
    public Text[] Texts;
    void Awake()
    {
        toSeverCommand = ToSeverCommand.MakeCommand(toseverButton, Texts);
        if (toSeverCommand == null) 
            Debug.LogError("toSeverCommand is null_ name:" + gameObject.name+ " ToseverButtonDo: " + toseverButton.ToString());
    }

    public void Button()
    {
        NetworkPanelControl.networkPanelControl.ButtonDo(toSeverCommand);
    }

}
