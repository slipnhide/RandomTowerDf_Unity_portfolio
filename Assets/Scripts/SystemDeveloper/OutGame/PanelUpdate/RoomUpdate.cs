using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class RoomUpdate : MonoBehaviourPunCallbacks
{
    public Transform RoomPanel_Parent;
    public GameObject RoomPanel;
    List<RoomInfo> roomInfos=new List<RoomInfo>();
    List<RoomInfoPanelUpdating> roomInfoPanels;
    public Transform JoinPanel;
    public Text JoinName;
   // public Text JoinPW;
    public override void OnJoinedLobby()
    {
        roomInfos.Clear();
    }

    public bool RoomIsStart(string RealRoomName)
    {
        foreach (RoomInfo roomInfo in roomInfos)
        {
            if (NetworkPanelControl.DecordingRoomName(roomInfo.Name)[0].Equals(RealRoomName))
            {
                //if ((bool)roomInfo.CustomProperties["IsStart"])
                if(!roomInfo.IsOpen)
                    return true;
                else
                    return false;
            }
        }
        return true;
    }
    public bool RoomIsStart(RoomInfo roomInfo)
    {
        if ((bool)roomInfo.CustomProperties["IsStart"])
            return true;
        else
            return false;
    }

    void UpdatingRoomInfo()
    {
        GameObject temp;
        string[] namePw;
        string curMans;
        bool Roomstart;
        for (int i = 0; i < roomInfos.Count; i++)
        {
            //현제 가진거 보다 적다면 생성, 아니라면 목록순으로 지정
            if (RoomPanel_Parent.childCount <= i)
                temp = Instantiate(RoomPanel, RoomPanel_Parent);
            else
            {
                RoomPanel_Parent.GetChild(i).gameObject.SetActive(true);
                temp = RoomPanel_Parent.GetChild(i).gameObject;
            }
            namePw = NetworkPanelControl.DecordingRoomName(roomInfos[i].Name);
            curMans = roomInfos[i].PlayerCount.ToString() + " / " + ((int)(roomInfos[i].MaxPlayers)).ToString();
            Roomstart = RoomIsStart(namePw[0]);
            if (Roomstart)
            {
                Debug.Log(namePw[0] + " 게임중");
                Text[] texts = temp.GetComponentsInChildren<Text>();
                foreach (Text text in texts)
                    text.color = Color.red;
            }
            else
            {
                Debug.Log(namePw[0] + " 대기중");
                Text[] texts = temp.GetComponentsInChildren<Text>();
                foreach (Text text in texts)
                    text.color = Color.black;
            }

            temp.GetComponent<RoomInfoPanelUpdating>().Set(namePw[0], curMans, Roomstart);
        }
    }
    public override void OnLeftRoom()
    {
        UpdatingRoomInfo();
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("OnRoomListUpdate");
        //고라니Tv 포톤 참조
        int roomCount = roomList.Count;
        for (int i = 0; i < roomCount; i++)
        {
            //사라진 방이 아니라면
            if (!roomList[i].RemovedFromList)
            {
                //현제 방목록을 가지고 있던게 아니면 추가
                if (!roomInfos.Contains(roomList[i])) roomInfos.Add(roomList[i]);
                //가지고 있던것이라면 바뀐 인원수등 떄문에 초기화
                else
                {
                    roomInfos[roomInfos.IndexOf(roomList[i])] = roomList[i];
                }
            }
            //사라지는 방인데 내가 가지고 있던게 맞으면
            else if (roomInfos.IndexOf(roomList[i]) != -1)
            {
                Unactive(roomList[i].Name);
                roomInfos.RemoveAt(roomInfos.IndexOf(roomList[i]));
            }
        }
        UpdatingRoomInfo();
    }
    void Unactive(string RoomnameNPw)
    {
        string RealName = NetworkPanelControl.DecordingRoomName(RoomnameNPw)[0];
        for (int i = 0; i < RoomPanel_Parent.childCount; i++)
        {
            RoomPanel_Parent.GetChild(i).GetComponent<RoomInfoPanelUpdating>().IsSameName(RealName);
            RoomPanel_Parent.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void ToJoin(string RealRoomName)
    {
        string EndCode = NetworkPanelControl.EncordingRoomName(RealRoomName);
        Debug.Log("Call RealRoomName  "+ RealRoomName);
        //비밀번호 없는경우 찾음
        foreach (RoomInfo roomInfo in roomInfos)
        {
            if (roomInfo.Name.Equals(EndCode))
            {
                Debug.Log("Join_Room");
                NetworkPanelControl.networkPanelControl.ButtonDo(new Join_Room(PanelType.Room,true,true,new string[] { RealRoomName }));
                return;
            }
        }
        Debug.Log("Call Panel_To_Join_Room");
        JoinPanel.gameObject.SetActive(true);
        JoinName.text = RealRoomName;
    }
}
