using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaSystem:MonoBehaviour
{
    List<GachaSlot> gachaSlot;
    List<DeckSlot> deckSlots;
    public GameObject gachaSlotPanel;
    public GameObject deckSlotsPanel;
    PlayerInfo_Photon PlyerInfo ;
    public GameObject BaseGachaSlot;
    public GameObject BaseDeckSlot;
    bool InfinitySlot = false;
    private void Awake()
    {
        //PlyerInfo = GetComponent<Player_Info>();
        //gachaSlot = new List<GachaSlot>();
        //for (int index = 0; index < gachaSlotPanel.transform.childCount; index++)
        //{
        //    gachaSlot.Add(gachaSlotPanel.transform.GetChild(index).GetComponent<GachaSlot>());
        //    gachaSlot[index].Set(PlyerInfo, this);
        //}
        //deckSlots= new List<DeckSlot>();
        //for (int index = 0; index < deckSlotsPanel.transform.childCount; index++)
        //{
        //    deckSlots.Add(deckSlotsPanel.transform.GetChild(index).GetComponent<DeckSlot>());
        //    deckSlots[index].Set(PlyerInfo, this);
        //}
    }
    private void Start()
    {
        PlyerInfo = PlayerInfo_Master.master.MyInfo;
    }

    public void Roll()
    {
        if (PlyerInfo.Gold < 
            Common_Static_Field.RollPay) 
            return;

        PlyerInfo.Gold -= Common_Static_Field.RollPay;
        foreach (GachaSlot slot in gachaSlot)
        {
            int SelectedRank = 0;
            RankPerLv CurPercentage = Common_Static_Field.rankPerLvs[PlyerInfo.lv];
            int MaxPercentage = 0;
            int length = CurPercentage.Rank_Percentage.Length;
            for (int i = 0; i < length; i++)
            {
                MaxPercentage += CurPercentage.Rank_Percentage[i];
            }
            int random = Random.Range(0, MaxPercentage);
            for (int i = length-1; i>-1; i--)
            {
                if (random <= CurPercentage.Rank_Percentage[i])
                    SelectedRank = i;
            }
            #region 이전방법
            /*
            RankPerLv RankPercent = Common_Static_Field.rankPerLvs[PlyerInfo.PlayerLv];
            int Langht = RankPercent.Rank_Percentage.Length;
            if (RankPercent.Rank_Percentage == null || Langht==0)
                Debug.LogError("RollFuctionError_ RankPercent Is Not Setted curPlayerLv:" + PlyerInfo.PlayerLv);
            int SelectedRank=0;
            int CurPercent = 0;
            int Randomize = Random.Range(0, 100);
            for (int i = Langht - 1; i > -1; i--)
            {
                CurPercent += RankPercent.Rank_Percentage[i];
               // Debug.Log("i:" + i + "  CurPercent" + CurPercent);
                if (Randomize < CurPercent)
                {
                    SelectedRank = i;
                    if (TowerList.towerList.TowerPerRank[SelectedRank].Towers.Length == 0)
                    {
                        Debug.LogWarning("RollFuctionError: TowerList Is Empty _ CurRank:" + (SelectedRank));
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            */
            #endregion
            // Debug.Log("SelectedRank" + SelectedRank+ "  PlayerLv"+PlyerInfo.PlayerLv);
            //int RandomTower = Random.Range(0,Ingame_View_Model.ingame_View_Model.towerListPerRanks[SelectedRank].Towers.Length);
            //Champion temp = Ingame_View_Model.ingame_View_Model.towerListPerRanks[SelectedRank].Towers[RandomTower].GetComponent<Champion>();
            //slot.SetSlot(temp);
        }
    }

    //public bool Buy(Champion _tower)
    //{
    //    if (PlyerInfo.Gold >= _tower.RequiredGold)
    //    {
           
    //        //가챠 시스템에서 n개 이상인지 확인하고 아니라면 진행 맞다면 true반환하고 종료

    //        //현제가진 슬롯중 빈곳을 찾아서 Set하고 종료
    //        foreach (DeckSlot slot in deckSlots)
    //        {
    //            if (slot.IsEmpty())
    //            {
    //                slot.SetSlot(_tower);
    //                PlyerInfo.TryGoldChg(_tower.RequiredGold * -1);
    //                return true;
    //            }
    //        }
    //        if(InfinitySlot)
    //        {
    //            //빈곳을 못찾았다면 슬롯을 추가한뒤 Set
    //            //AddNewDeckSlot(_tower);
    //            PlyerInfo.TryGoldChg(_tower.RequiredGold * -1);
    //            return true;
    //        }
    //    }
    //    return false;
    //}
    //public void AddNewDeckSlot(Tower _tower=null)
    //{
    //    if (!InfinitySlot) return;
    //    GameObject NewSlot = Instantiate(BaseDeckSlot);
    //    NewSlot.transform.SetParent(deckSlotsPanel.transform);

    //    DeckSlot NewDeck= NewSlot.GetComponent<DeckSlot>();
    //    NewDeck.Set(PlyerInfo, this);
    //    deckSlots.Add(NewDeck);
    //    if (_tower != null)
    //        NewDeck.SetSlot(_tower);
    //}
    //public void AddTower(Champion _tower)
    //{
    //    //현제가진 슬롯중 빈곳을 찾아서 Set하고 종료
    //    foreach (DeckSlot slot in deckSlots)
    //    {
    //        if (slot.IsEmpty())
    //        {
    //            slot.SetSlot(_tower);
    //        }
    //    }
    //    {
    //        //빈곳을 못찾았다면 슬롯을 추가한뒤 Set
    //       // if (InfinitySlot)
    //            //AddNewDeckSlot(_tower);
    //    }
    //}
}
