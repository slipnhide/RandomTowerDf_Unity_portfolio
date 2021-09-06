using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public interface Ingame_Update_PlayerInfo
{
    void ReadyUpdating(bool value);
    void LifeUpdate(int value);

    void GoldUpdate(int value);
    void MedalUpdate(int[] job,int[] Sp);
}

public class Ingame_CameraChanger : MonoBehaviour, Ingame_Update_PlayerInfo
{
    public GameObject DevelButton;
    Image DevilIMage;
    public Text Gold;
    public Text Player_name;
    public Text Ready;
    public Text Life;
    [SerializeField]
    PlayerInfo_Photon playerInfo;
    Vector3 StrCameraPos;
    int ActorNumber;
    PhotonView Pv;
    bool DevilOff = false;
   // public void Set(string name, PlayerInfo_Photon _playerInfo, Vector3 _StrCameraPos) { playerInfo = _playerInfo; StrCameraPos = _StrCameraPos; Player_name.text = name; }

    private void Awake()
    {
        Pv = GetComponent<PhotonView>();
        
        if (Pv.IsMine)
        {

            Pv.RPC(nameof(Set), RpcTarget.All);
            transform.SetAsFirstSibling();
        }
        DevilIMage = DevelButton.GetComponent<Image>();
    }

    public void ReadyUpdating(bool value)
    {
        Pv.RPC(nameof(updating), RpcTarget.All, value);
    }
    [PunRPC]
    void updating(bool value)
    {
        if (value)
            Ready.text = "Ready";
        else
            Ready.text = "";
    }
    [PunRPC]
    void Set()
    {
        PlayerInfo_Photon[] playerInfo_Photons = FindObjectsOfType<PlayerInfo_Photon>();
        Debug.Log(playerInfo_Photons.Length);

        transform.parent = Ingame_View_Controler.ingame_View_Controler.eachPlayerInfoPanel;
        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
        Player_name.text = Pv.Owner.NickName;
        ActorNumber = Pv.Owner.ActorNumber;
        foreach (PlayerInfo_Photon playerInfo_Photon in playerInfo_Photons)
        {
            if (playerInfo_Photon.Pv.Owner==Pv.Owner)
            {
                Debug.Log(Pv.Owner.NickName);
                playerInfo = playerInfo_Photon;
                playerInfo_Photon.InfoUpdating = this;
                LifeUpdate(Common_Static_Field.StartLife);
                break;
            }
        }
    }

    public void LifeUpdate(int value)
    {
        if (value <= 0)
            Life.text = "GameEnd";
        else
            Life.text = value.ToString();
    }
    //[PunRPC]
    //void lifeupdating(int value)
    //{
    //    Life.text = value.ToString();
    //}

    public void CameraSwitch()
    {
       Camera.main.transform.position = playerInfo.Pos+new Vector3(0,0,-10);
       // Debug.Log("Before : " + PlayerInfo_Master.master.MyInfo.LookActorNum);
        PlayerInfo_Master.master.MyInfo.LookActorNum = ActorNumber;
       // Debug.Log("Before : " + PlayerInfo_Master.master.MyInfo.LookActorNum);
        int[][] Ranks = playerInfo.CurRanks();
        Ingame_View_Controler.ingame_View_Controler.MedalUpdate(Ranks[0], Ranks[1]);
    }
    public void MedalUpdate(int[] job, int[] Sp)
    {
      //  Debug.Log("MedalUpdate요청");
        Ingame_View_Controler.ingame_View_Controler.MedalUpdate(job, Sp);
    }
    public void GoldUpdate(int value)
    {
        Gold.text = "Gold : "+value.ToString();
    }
    public void DevilPanelOpen()
    {
        if (Pv.IsMine) return;
        else if (PlayerInfo_Master.master.MyInfo.CurRank(Species.Demon) > 0)
            DevelButton.SetActive(true);
    }
    public void DevilPanelClose()
    {
        DevelButton.SetActive(false);
    }
    private void Update()
    {
        if (!DevilOff) return;
        if (Pv.IsMine) return;
        else
        {
            DevilIMage.fillAmount = PlayerInfo_Master.master.MyInfo.CoolTimePerDevil;
            if (DevilIMage.fillAmount >= 1)
                DevilOff = false;
        }
    }
    public void DevilDo()
    {
        Debug.Log("DevilDo()");
        Debug.Log(DevilOff);
        Debug.Log(playerInfo.CurRank(Species.Demon));
        Debug.Log(PlayerInfo_Master.master.MyInfo.DevilDo(Pv.OwnerActorNr));
        if (!DevilOff)
        if(playerInfo.CurRank(Species.Demon)>0)
        if (PlayerInfo_Master.master.MyInfo.DevilDo(Pv.OwnerActorNr))
        {
            DevilOff = true;
        }
    }

}
