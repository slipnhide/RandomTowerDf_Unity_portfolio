using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaSlot : DeckSlot
{

    public Text Gold;
   // int[] Job_Spiceis;

   // protected GachaSystem gachaSystem;
    //protected Player_Info player_Info;

    protected override void Awake()
    {
        base.Awake();
    }
    public override void SetSlot(int[] _Job_Spiceis, int Rank=1)
    {
        ColorOnOff(true);
        Job_Spiceis = _Job_Spiceis;
        //Job tempJob=global::Job.LastNumber;
        //if (_Tower.GetComponent<RangeChampion>())
        //    tempJob = (Job)_Tower.GetComponent<RangeChampion>().Rjob;
        //else if (_Tower.GetComponent<MeleeChampion>())
        //    tempJob = (Job)_Tower.GetComponent<MeleeChampion>().Mjob;
        // Job.text = tempJob.ToString();

        //Champion champion = _Tower.GetComponent<Champion>();
        // Sprite sprite= 
        Job.sprite = Ingame_View_Controler.ingame_View_Controler.FindMedal((Job)_Job_Spiceis[0], 0);
        //Champion tower = champion;
        Species.sprite = Ingame_View_Controler.ingame_View_Controler.FindMedal((Species)_Job_Spiceis[1], 0);

        //CurSlotTower = _Tower;
        //if (tower.GetComponent<SpriteRenderer>())
        //    SlotImg.sprite = tower.GetComponent<SpriteRenderer>().sprite;
        //else
        //    SlotImg.sprite = null;
        // Debug.Log(tower.RequiredGold);
        SlotImg.sprite = Ingame_View_Controler.ingame_View_Controler.FindShopImage(_Job_Spiceis[0], _Job_Spiceis[1]);
        Gold.text = Common_Static_Field.ChapmGold[_Job_Spiceis[0], _Job_Spiceis[1]].ToString();
       // Gold.text = champion.RequiredGold.ToString();
    }
    public void Buy()
    {
        if(!IsEmpty())
        if (Ingame_View_Controler.ingame_View_Controler.Buy(Job_Spiceis))
            ToEmpty();
    }
    public override void ToEmpty()
    {
       // GachaSlot BaseGachaSlot = Ingame_View_Controler.ingame_View_Controler.BaseGachaSlot.GetComponent<GachaSlot>();
        Job_Spiceis = null;
        ColorOnOff(false);
    }
    protected override void ColorOnOff(bool IsOn)
    {
        if (IsOn)
        {
            Species.color = Color.white;
            Job.color = Color.white;
            SlotImg.color = Color.white;
        }
        else
        {
            Species.color = Color.clear;
            Job.color = Color.clear;
            SlotImg.color = Color.clear;
            Gold.text = "";
        }
    }
    public void OnMouseEnter()
    {
        //Debug.Log("OnMouseEnter"+gameObject.name);
        //  if (!IsEmpty())
        //   explain.Set(Job_Spiceis);
        Vector3 pos = transform.position + new Vector3(GetComponent<RectTransform>().rect.width/2-60, 0, 0);
        if (Job_Spiceis!=null)
        Ingame_View_Controler.ingame_View_Controler.OnChampion(Job_Spiceis, 1, pos);
    } 
    public void OnMouseExit()
    {
        //if (Job_Spiceis != null)
            Ingame_View_Controler.ingame_View_Controler.OutChampion();
         //Debug.Log("OnMouseExit" + gameObject.name);
      //  explain.Off();
    }
    //public virtual void Set(Player_Info _player_Info, GachaSystem _gachaSystem) 
    //{ player_Info = _player_Info; gachaSystem = _gachaSystem; explain = player_Info.gameObject.GetComponent<Explain>(); }

}
