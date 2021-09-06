using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UpdateThing {LobbyNum, CurLobbyNum, ManNumInRoom, NickNameInRoom,ReadyText,RoomState, ActorNum}

interface UpdateSomeFromNet 
{
    void Updating();
}

public class UpdatingSomeFromNet : UpdateSomeFromNet
{
    protected Text text;
    public UpdatingSomeFromNet(Text _text) {text = _text; }

    public virtual void Updating() { }

    public static UpdatingSomeFromNet MakeUpdatingSomeFromNet(UpdateThing UpdateThing, Text _text)
    {

        switch (UpdateThing)
        {
            case UpdateThing.LobbyNum:
                return  new UpdateLobbyNum(_text);
            case UpdateThing.CurLobbyNum:
                return new UpdateCurLobbyNum(_text);
            case UpdateThing.NickNameInRoom:
                return new UpdateNickNamesInRoom(_text);
            case UpdateThing.ManNumInRoom:
                return new UpdateManNumInRoom(_text);
            case UpdateThing.ReadyText:
                return new UpdateReadyOrStart(_text);
            case UpdateThing.RoomState:
                return new UpdateRoomState(_text);
            case UpdateThing.ActorNum:
                return new UpdateActNum(_text);

        }

        return null;
    }
}
public class UpdateActNum : UpdatingSomeFromNet
{
    public UpdateActNum(Text _text) : base(_text) { }
    public override void Updating()
    {
        if (text == null) Debug.LogError("text==null");
        else
        {
            text.text = NetworkManager.networkManager.ordinalInRoom.ToString();
        }
    }
}
public class UpdateLobbyNum : UpdatingSomeFromNet
{
    public UpdateLobbyNum(Text _text) : base(_text) {  }
    public override void Updating()
    {
        if (text == null) Debug.LogError("text==null");
        else
            text.text = NetworkManager.networkManager.CurLobbyMansNum.ToString();
    }
}

public class UpdateCurLobbyNum : UpdatingSomeFromNet
{
    public UpdateCurLobbyNum(Text _text): base(_text) { }
    public override void Updating()
    {
        if(text==null) Debug.LogError("text==null");
        else
        text.text = NetworkManager.networkManager.CurLobbyMansNum.ToString()+" / "+ NetworkManager.networkManager.AllMans;
    }
}
public class UpdateRoomName : UpdatingSomeFromNet
{
    public UpdateRoomName(Text _text) : base(_text) {}
    public override void Updating()
    {
        if (text == null) Debug.LogError("text==null");
        else
            text.text = NetworkManager.networkManager.CurLobbyMansNum.ToString() ;
    }
}
public class UpdateRoomsMaxMans : UpdatingSomeFromNet
{
    public UpdateRoomsMaxMans(Text _text) : base(_text) {  }
    public override void Updating()
    {
        if (text == null) Debug.LogError("text==null");
        else
            text.text = NetworkManager.networkManager.CurRoomMaxMans.ToString();
    }
}
public class UpdateManNumInRoom : UpdatingSomeFromNet
{
    public UpdateManNumInRoom(Text _text) : base(_text) { }
    public override void Updating()
    {
        if (text == null) Debug.LogError("text==null");
        else
            text.text = NetworkManager.networkManager.MansNumInRoom.ToString()+" / "+ NetworkManager.networkManager.CurRoomMaxMans;
    }
}
public class UpdateNickNamesInRoom : UpdatingSomeFromNet
{
    string[] NickNams;
    public UpdateNickNamesInRoom(Text _text) : base( _text) {  }
    public override void Updating()
    {
        if (text == null) Debug.LogError("text==null");
        else
        {
            NickNams = NetworkManager.networkManager.GetNickNamesInRoom();
            text.text = "";
            int curLenght = NickNams.Length;
            for (int i = 0; i < curLenght; i++)
            {
                text.text += NickNams[i]+ (i + 1 == curLenght ? "" : ", ");
            }

        }
    }
}
public class UpdateReadyOrStart : UpdatingSomeFromNet
{
    public UpdateReadyOrStart(Text _text) : base(_text) { }
    public override void Updating()
    {
        if (text == null) Debug.LogError("text==null");
        else
            text.text = NetworkManager.networkManager.IsHost ? "Start" : NetworkManager.networkManager.IsReady ? "I_am_Ready" : "Do Ready";
    }
}
public class UpdateRoomState : UpdatingSomeFromNet
{
    public UpdateRoomState(Text _text) : base(_text) { }
    public override void Updating()
    {
        if (text == null) Debug.LogError("text==null");
        else
            text.text = NetworkManager.networkManager.IsGaming ? "Start" : "Wait";
    }
}

public class UpdatedFromNet : MonoBehaviour
{
    public UpdateThing updateThing;
    public Text text;
    UpdateSomeFromNet updateSomeFromNet;

    private void Awake()
    {
        updateSomeFromNet = UpdatingSomeFromNet.MakeUpdatingSomeFromNet(updateThing, text);
        if (updateSomeFromNet == null)
            Debug.LogError("updateSomeFromNet Is Not Set : updateThing_:" + updateThing);
    }

    // Update is called once per frame
    void Update()
    {
        //if (updateSomeFromNet == null)
        //    Debug.LogError("updateSomeFromNet Is Not Set : updateThing_:" + updateThing);
        //else
        updateSomeFromNet.Updating();
    }
}
