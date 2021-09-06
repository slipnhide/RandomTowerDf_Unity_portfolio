using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Player_Info : MonoBehaviour
{
    public static Player_Info player_Info;
    [Range(0, 5)]
    public int PlayerLv=0;
    public Image Xpimage;
    int RequiredXp;
    int CurXp;
    [HideInInspector]
    public float XpPercentate { get { return (float)CurXp / RequiredXp; } }
    [SerializeField]
    [Range(1, 99999)]
    int gold;
    public Text CurGoldText;
    public int Gold { get { return gold; } set { if (photon != null)  gold =value; } }//photon.SetGold(value);
    //if (photon != null) photon.SetGold(value);
    public PlayerInfo_Photon photon;


    private void Awake()
    {
        if (player_Info == null)
            player_Info = this;
        XpSet();
        Gold = 100;
    }


    public bool GoldChg(int ChgGold=0) 
    { 
        if (Gold + ChgGold < 0) 
            return false; 
        else 
        { 
            Gold += ChgGold;
            CurGoldText.text = Gold.ToString();
            return true; 
        } 
    }
    [ContextMenu("GoldSet")]
    public void GoldText()
    {
     CurGoldText.text = Gold.ToString();
    }
    void XpSet()
    {
        RequiredXp = (PlayerLv+1) * 10;
        CurXp = 0;
        Xpimage.transform.GetChild(0).GetComponent<Text>().text ="CurLv: "+ PlayerLv.ToString();
    }
    public void AddXp()
    {
        CurXp++;
        if (CurXp>= RequiredXp)
        {
            PlayerLv++;
            XpSet();
        }
        Xpimage.fillAmount = (float)CurXp / RequiredXp;
    }
}
