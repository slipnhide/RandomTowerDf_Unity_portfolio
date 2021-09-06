using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public struct JobItem 
{
    public Job job; public Sprite[] image;
    public JobItem(Job _job, Sprite[] sprites)
    {
        job = _job;
        image = sprites;
    }
}
public struct SpeciesItem
{
    public Species species; public Sprite[] image;
    public SpeciesItem(Species _species, Sprite[] sprites)
    {
        species = _species;
        image = sprites;
    }
}

public class Ingame_View_Controler : MonoBehaviour
{
    public Text test;
    public static Ingame_View_Controler ingame_View_Controler;
    SynergyView synergyView=new SynergyView();
    public void MedalUpdate(int[] JobRank, int[] SpRank)
    {
        Debug.Log("Ingame_View_Controler MedalUpdate");
        synergyView.MedalUpdate(JobRank, SpRank);
    }
    public ParticleSystem[] RankParticle;
    public GameObject ChampionPanel;
    public Text[] ChampionText;
    RectTransform[] JobSpRect=new RectTransform[2];
    public Image[] ChampionImages;
    public Image[] MedalImages;
    public Button SellButton;
    public Image[] SkillCoolTimeImage;
    public AudioClip[] BaseAtackSound;
    public AudioClip AssasineCriticalSound;
    public AudioClip WindSound;
    public AudioClip[] SkillsSound;
    public AudioClip TicTocSound;
    public AudioClip BuySound;
    public AudioClip FailedSound;
    public AudioClip OnFieldSound;
    public AudioClip BoosSound;
    public AudioClip StunSound;
    public void BossSound() { audio.PlayOneShot(BoosSound); }
    public AudioClip CoinDrop;
    public void CoinDropSound() { audio.PlayOneShot(CoinDrop); }
    public AudioClip DiceRoll;
    AudioSource audio;
    float[] CoolTimeDelta=new float[3];
    public SpriteRenderer[] Tiles;
    public BoxCollider2D[] TielsColliders;

    public void TileOpen(bool OnOff)
    {
        int Num = Tiles.Length;
        if (OnOff)
            //   Tiles[0].transform.parent.gameObject.SetActive(true);
            for (int i = 0; i < Num; i++)
            {
                TielsColliders[i].enabled = true;
                Tiles[i].color = Color.magenta;
            }
        else
            //  Tiles[0].transform.parent.gameObject.SetActive(false);
            for (int i = 0; i < Num; i++)
            {
                TielsColliders[i].enabled = false;
                Tiles[i].color = Color.clear;
            }
    }
    //public Image 
    public Image LvPercentage;
    public Text Lv;
    public Text Gold;
    public Text RemainTime;
    public GameObject WaveComingObject;
    public Text WaveComingText;
    IEnumerator TicToc;
    public Transform eachPlayerInfoPanel;
    public GameObject SynergyPanel;
    public GameObject RollSlotPanel;
    public GameObject TowerInventoryPanel;
    Champion SelectedChampion;
    public List<DeckSlot> deckSlots;
   // public GameObject ExplainPanel;
   // public GameObject BaseGachaSlot;
    //public GameObject BaseDeckSlot;
    public Explain explain;
    public Transform GameOverPanel;

    public PlayerInfo_Photon Myinfo;
    // public Item[] UsedMedalImage;
    Dictionary<Job, JobItem> JobMedal = new Dictionary<Job, JobItem>();
    Dictionary<Species, SpeciesItem> SpeciesMedal = new Dictionary<Species, SpeciesItem>();
    Sprite[,] ShopImage = new Sprite[(int)Job.LastNumber, (int)Species.LastNumber];
    //public Image XpImage;

    bool InfinitySlot = false;
    private void Awake()
    {
        synergyView.SetImages(MedalImages);
        if (ingame_View_Controler == null)
        {
            ingame_View_Controler = this;
        }
        else if (ingame_View_Controler != this)
        {
            Destroy(gameObject);
        }

        if (TowerInventoryPanel == null) { Debug.LogWarning("TowerInventoryPanel Is Null"); return; }
        else
        {
            deckSlots = new List<DeckSlot>();
            for (int index = 0; index < TowerInventoryPanel.transform.childCount; index++)
                deckSlots.Add(TowerInventoryPanel.transform.GetChild(index).GetComponent<DeckSlot>());
        }
        #region 메달이미지 초기화
        Sprite[] sprites = Resources.LoadAll<Sprite>("Medal");
        for (int i = 0; i < (int)Job.LastNumber; i++)
        {
            JobMedal.Add((Job)i, new JobItem((Job)i, new Sprite[3]
                {
                    sprites[(i*3)],
                    sprites[(i*3)+1],
                    sprites[(i*3)+2]
                }));
        }
        for (int i =0 ; i < (int)(Species.LastNumber); i++)
        {
            SpeciesMedal.Add((Species)i, new SpeciesItem((Species)i, new Sprite[3]
            {
                    sprites[((i+(int)Job.LastNumber)*3)],
                    sprites[((i+(int)Job.LastNumber)*3)+1],
                    sprites[((i+(int)Job.LastNumber)*3)+2]
            }
            )
            );
        }
        sprites = null;
        #endregion
        #region 샵이미지 초기화
        sprites = Resources.LoadAll<Sprite>("ChampionSprite");
        int Index = 0;
        for (int i = 0; i <(int)Species.LastNumber; i++)
        {
            for (int t = 0; t <(int)Job.LastNumber; t++)
            {
                ShopImage[t,i] = sprites[Index];
              //  Debug.Log(sprites[Index].name);
                    Index++;
            }
        }
        #endregion
        audio = GetComponent<AudioSource>();
        JobSpRect[0] = ChampionText[4].transform.parent.GetComponent<RectTransform>();
        JobSpRect[1] = ChampionText[5].transform.parent.GetComponent<RectTransform>();
    }
    public Sprite FindMedal(Job _job, int Rank){return JobMedal[_job].image[Rank];}
    public Sprite FindMedal(Species _species, int Rank) { return SpeciesMedal[_species].image[Rank]; }

    public Sprite FindShopImage(int _job, int _species) { return ShopImage[_job, _species]; }
    public void SynergyPanelOn()
    {
        SynergyPanel.SetActive(true);
    }


    public void Roll()
    {
        if (Myinfo.Gold <
            (Common_Static_Field.RollPay))
        {
            audio.PlayOneShot(FailedSound);
            return;
        }
        Myinfo.Gold -= Common_Static_Field.RollPay;
        audio.PlayOneShot(DiceRoll);
        int[][] towers=Ingame_View_Model.ingame_View_Model.Roll();
            RollUpdate(towers);
    }
    void RollUpdate(int[][] towers)
    {
        for (int index = 0; index < RollSlotPanel.transform.childCount; index++)
        {
            RollSlotPanel.transform.GetChild(index).GetComponent<GachaSlot>().SetSlot(towers[index]);
        }
    }
    public bool Buy(int[] Job_Spicies)
    {


        //빈칸이 있는지 확장 가능한지 확인
        DeckSlot EmptySlot=null;
        foreach (DeckSlot slot in deckSlots)
        {
            if (slot.IsEmpty())
            {
                EmptySlot = slot;
                break;
            }
        }
        //if (EmptySlot==null && InfinitySlot)
        //{
        //    //빈곳을 못찾았다면 슬롯을 추가한뒤
        //    EmptySlot=AddNewDeckSlot();
        //}

        if (EmptySlot != null)
        {
            if (Ingame_View_Model.ingame_View_Model.Buy(Common_Static_Field.ChapmGold[Job_Spicies[0], Job_Spicies[1]]))
            {
                EmptySlot.SetSlot(Job_Spicies);
                return true;
            }
            else
            {
                audio.PlayOneShot(FailedSound);
                return false;
            }
        }
        else
        {
            audio.PlayOneShot(FailedSound);
            return false;
        }
    }
    public DeckSlot[] FindDeckSlot(int[] Job_Sp, int Rank)
    {
        DeckSlot[] temp = TowerInventoryPanel.transform.GetComponentsInChildren<DeckSlot>();
        int[] DJob_Sp = null;
        List<DeckSlot> deckSlots = new List<DeckSlot>();
        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i].IsEmpty())
                continue;
            DJob_Sp = temp[i].GetJob_Sp();
            if (Job_Sp[0] == DJob_Sp[0] && Job_Sp[1] == DJob_Sp[1] && temp[i].ChapionRank == Rank)
            {
                deckSlots.Add(temp[i]);
            }
        }
        return deckSlots.ToArray();
    }
    public int FindDeckSlotNum(int[] Job_Sp, int Rank)
    {
        DeckSlot[] temp = TowerInventoryPanel.transform.GetComponentsInChildren<DeckSlot>();
        int[] DJob_Sp = null;
        int value = 0;
       // Debug.Log(" Job: " + Job_Sp[0] + "  Sp : " + Job_Sp[1] + "  Rank : " + Rank);
        // List<DeckSlot> deckSlots = new List<DeckSlot>();
        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i].IsEmpty())
                continue;
            DJob_Sp = temp[i].GetJob_Sp();
            //Debug.Log("i : " + i + " Job: " + DJob_Sp[0] + "  Sp : " + DJob_Sp[1]+ "  Rank : "+ temp[i].ChapionRank);
            if (Job_Sp[0] == DJob_Sp[0] && Job_Sp[1] == DJob_Sp[1] && temp[i].ChapionRank == Rank)
            {
                value++;
            }
        }
        // _deckSlot = deckSlots.ToArray();
        return value;
    }

    public void TowerMerge(GameObject _tower, int _Rank)
    {
        
    }

    public void DeckSlotReTake()
    {
        bool IsEmpty = false;
        for (int i = 0; i < deckSlots.Count; i++)
        {
            if (!IsEmpty)
            {
                if (deckSlots[i].IsEmpty())
                {
                    IsEmpty = true;
                }
                else
                    continue;
            }
            else
            {
                if (deckSlots[i].IsEmpty())
                {
                    break;
                }
                else
                {
                    int[] temp = deckSlots[i].GetJob_Sp();
                    int Rank = deckSlots[i].ChapionRank;
                    deckSlots[i].ToEmpty();
                    deckSlots[i - 1].SetSlot(temp, Rank);
                }
            }

        }
    }

    //public DeckSlot AddNewDeckSlot()
    //{
    //    GameObject NewSlot = Instantiate(BaseDeckSlot);
    //    NewSlot.transform.SetParent(TowerInventoryPanel.transform);
    //    DeckSlot New = NewSlot.GetComponent<DeckSlot>();
    //    deckSlots.Add(New);
    //    return New;
    //}

    public void BuyXp()
    {
        if (LvPercentage == null) Debug.LogError("LvPercentageImage Is Null");
        if (Myinfo.lv >= Common_Static_Field.MaxLv) return;
        if (Ingame_View_Model.ingame_View_Model.BuyXp())
            LevelUpdate();
    }


    public bool TowerSpawn(int[] Job_Sp, Tile _Tile, Vector3 Pos,int _Rank=1)
    {
        //모델에서 해당 타워가 프리팹으로 존재하는지 확인하거나 오브젝트로 변환하여 생성
        //기타 모든검사는 덱슬롯 쪽으로 처리위임
        bool Value = false;
        Value= Ingame_View_Model.ingame_View_Model.SpawnTower(Job_Sp, _Tile, Pos, _Rank);
        if (Value)
        {
            audio.PlayOneShot(OnFieldSound);
            //생성시 일어나야할 이미지등 처리있다면 처리, 데이터 처리는 모델에서 처리
        }

        return Value;
    }

    public void LevelUpdate()
    {
        LvPercentage.fillAmount = Myinfo.XpPercentate;
        Lv.text =" Lv : "+ Myinfo.lv.ToString();
    }
    public void GoldUpdate()
    {
        if (Gold == null)
            return;
        Gold.text =
         Myinfo.
         Gold.ToString();
    }

    public void WaveStart()
    {
        TicToc = Tic();
        StartCoroutine(TicToc);

    }
    public void NomoreWait()
    {
        Debug.Log("NomoreWait");
        RemainTime.text = " Waveing ";
        WaveComingObject.SetActive(false);
        StopCoroutine(TicToc);
    }
    IEnumerator Tic()
    {
        RemainTime.color = Color.magenta;
        float Maxtime = Common_Static_Field.WaitTime + Common_Static_Field.FreeTime;
        float CurTime=0;
        while (CurTime< Common_Static_Field.WaitTime)
        {
            CurTime += 0.1f;
            RemainTime.text = ((int)CurTime).ToString("F1");
            yield return new WaitForSeconds(0.1f);
        }
        RemainTime.color = Color.red;
        int WarningStart = 5;
        IEnumerator WaveCount=null;
        
        while (CurTime < Maxtime)
        {

            CurTime += 0.1f;
            RemainTime.text = ((int)CurTime).ToString("F1");
            if (Mathf.Abs(Maxtime - CurTime) <= WarningStart)
            {
               // Debug.Log(WarningStart);
                if(WaveCount!=null)
                StopCoroutine(WaveCount);
                WaveCount = WaveCountWarning(WarningStart.ToString());
                StartCoroutine(WaveCount);
                WarningStart--;
            }
            yield return new WaitForSeconds(0.1f);
        }
        NomoreWait();
       // RemainTime.text = " Waveing ";
       // WaveComingObject.SetActive(false);
    }
    IEnumerator WaveCountWarning(string i)
    {
        Transform parent = WaveComingText.gameObject.transform.parent;
        WaveComingObject.SetActive(true);
        audio.PlayOneShot(TicTocSound); 
        WaveComingText.text = i;
        for (float t=0; t<=1; t+=0.05f)
        {
          //  Debug.Log(Mathf.Lerp(90, -90, t));
           // parent.Rotate(new Vector3(Mathf.Lerp(90, -90, t), 0, 0));
            parent.localEulerAngles = new Vector3(Mathf.Lerp(90, -90, t), 0, 0);
            yield return new WaitForSeconds(0.05f);
        }
    }
    public void Ready()
    {
        if (Myinfo.ownEnemy > 0) return;
        else
            Myinfo.IsReady = !Myinfo.IsReady;
    }

    public void LifeOver()
    {
        GameOverPanel.gameObject.SetActive(true);
    }

    /*
     * public enum Job { Warrior, assassin,scout, Mage, Novel, LastNumber }
public enum RangeJob { scout = 2, Mage = 3, Novel = 4, LastNumber }
public enum MeleeJob { Warrior = 0, assassin = 1, LastNumber }
public enum Species { Angel, Demon, Human, Machine,MerMaid, Orc,Undead,LastNumber }
     */
    public static string[] JobText = new string[(int)Job.LastNumber]
        {
            "10회 공격마다 한 대상에게 추가공격을 합니다." +
            "침피언랭크와 시너지 단계가 높을수록 추가로 공격합니다.",

            " 일정확률로 크리티컬을 입힙니다. " +
            "챔피언 랭크에 따라 배율이 올라갑니다. " +
            "시너지 단계가 높을수록 확률이 올라갑니다.",

            "챔피언 랭크에따라 공격력이 배율로 올라갑니다." +
            "시너지가 발생하면 스킬이 개방됩니다."+
            "시너지 단계에 따라 모든 적에게 일정 데미지를 줍니다",

            "일정 확률로 시너지 단계에 따라 적을 뒤로 밀어냅니다" +
            "챔피언 랭크에따라 확률이 올라갑니다.",

            "라운드 종료시 추가 골드를 얻습니다",
        };
    static int[] JobTextSize = new int[(int)Job.LastNumber]
        {
            70,80,80,50,30
        };
    static int[] SpTextSize = new int[(int)Species.LastNumber]
    {
            70,70,30,30,70,70,70
    };
    /*
 * public enum Job { Warrior, assassin,scout, Mage, Novel, LastNumber }
public enum RangeJob { scout = 2, Mage = 3, Novel = 4, LastNumber }
public enum MeleeJob { Warrior = 0, assassin = 1, LastNumber }
public enum Species { Angel, Demon, Human, Machine,MerMaid, Orc,Undead,LastNumber }
 */
    public static string[] SpText = new string[(int)Species.LastNumber]
    {
                    "시너지가 발생하면 스킬이 개방됩니다."
        +"일정 시간 마우스 위치에 적을 공격합니다."
        +"단계에 따라 공격력이 증가합니다.",

            "시너지가 발생하면 스킬이 개방됩니다."
        +"시너지 단계에 따라 적에게 몬스터를 소환합니다.",

            "미구현" +
            "시너지 단계에 따라 모든 챔피언의 공격속도가 올라갑니다.",

            "챔피언 생성시 스텟에 보너스를 주게됩니다",

            "공격시 적을 느리게 합니다."+
        "시너지 단계에 따라 더 느려집니다"+
        "챔피언 랭크에따라 지속시간이 길어집니다.",

            "시너지가 발생하면 스킬이 개방됩니다."+
        "시너지 단계에 따라 일정시간 모든 적을 멈춥니다.",

            "공격시 적을 중독시킵니다."+
        "시너지 단계에 따라 지속시간이 증가합니다"+
        "챔피언 랭크에 따라 더 자주 데미지를 줍니다."

            
    };

    public void OnChampion(Champion champion)
    {
        ChampionPanel.GetComponent<Image>().raycastTarget = true;
        
        SelectedChampion = champion;
        if (champion.ActorNum != Myinfo.ActorNumber)
            SellButton.transform.parent.gameObject.SetActive(false);
        else
            SellButton.transform.parent.gameObject.SetActive(true);
        ChampionPanel.transform.GetChild(0).localPosition = new Vector3(90, 30, 0);
        ChampionPanel.transform.position = Camera.main.WorldToScreenPoint(champion.transform.position);
        ChampionImages[0].sprite = FindMedal(champion.Job, champion.Rank-1);
        ChampionImages[1].sprite = FindMedal(champion.species, champion.Rank-1);
        ChampionText[0].text = champion.Damage.ToString();
        ChampionText[1].text = champion.MultiTargetNum.ToString();
        ChampionText[2].text = champion.CoolTime.ToString("F2");
        ChampionText[3].text = champion.Range.ToString("F2");
        ChampionText[4].text = JobText[(int)champion.Job];
        JobSpRect[0].sizeDelta = new Vector2(0, JobTextSize[(int)champion.Job]);
        ChampionText[5].text = SpText[(int)champion.species];
        JobSpRect[1].sizeDelta = new Vector2(0, SpTextSize[(int)champion.species]);
        ChampionPanel.SetActive(true);
        ChampionPanel.SetActive(false);
        ChampionPanel.SetActive(true);
    }
    public void OnChampion(int[] JosSp,int Rank,Vector2 ScreenPos)
    {
        SellButton.transform.parent.gameObject.SetActive(false);
        ChampionPanel.GetComponent<Image>().raycastTarget = false;
        ChampionPanel.SetActive(true);
        ChampionPanel.transform.GetChild(0).localPosition = new Vector3(160, 100, 0);
        ChampionPanel.transform.position = ScreenPos;
        ChampionImages[0].sprite = FindMedal((Job)JosSp[0], Rank - 1);
        ChampionImages[1].sprite = FindMedal((Species)JosSp[1], Rank - 1);
        float[] Stat = Common_Static_Field.GetStat(Myinfo.ActorNumber, (Job)JosSp[0], (Species)JosSp[1], Rank);
        ChampionText[0].text = Stat[0].ToString();
        ChampionText[1].text = ((int)Stat[1]).ToString();
        ChampionText[2].text = Stat[2].ToString("F2");
        ChampionText[3].text = Stat[3].ToString("F2");
        ChampionText[4].text = JobText[JosSp[0]];
        JobSpRect[0].sizeDelta = new Vector2(0, JobTextSize[JosSp[0]]);
        ChampionText[5].text = SpText[JosSp[1]];
        JobSpRect[1].sizeDelta = new Vector2(0, SpTextSize[JosSp[1]]);
    }
    public void OutChampion()
    {
        SelectedChampion = null;
        ChampionPanel.SetActive(false);
       // ChampionPanel.GetComponent<Image>().raycastTarget = true;
    }
    //string[] Jobinfotext = new string[(int)Job.LastNumber] { };
    public void SellChamp()
    {
        ChampionPanel.SetActive(false);
        ToSimpleJson<int[]> number = new ToSimpleJson<int[]>();
        number.value = new int[1] { SelectedChampion.TowerNumber };
        Myinfo.DisableChampion(JsonUtility.ToJson(number),false,Vector2.zero);
        audio.PlayOneShot(CoinDrop);
    }

    private void Update()
    {

        CoolTimeDelta[0] -= Time.deltaTime;
        CoolTimeDelta[1] -= Time.deltaTime;
        CoolTimeDelta[2] -= Time.deltaTime;

        SkillCoolTimeImage[0].fillAmount = CoolTimeDelta[0]/ Common_Static_Field.ScoutCoolTime;
        SkillCoolTimeImage[1].fillAmount = CoolTimeDelta[1] / Common_Static_Field.OrcCoolTime;
        SkillCoolTimeImage[2].fillAmount = CoolTimeDelta[2] / Common_Static_Field.AngelCoolTime;

    }
    public void SkiilOnOff(int index, bool OnOff)
    {
        if (OnOff)
            SkillCoolTimeImage[index].transform.parent.gameObject.SetActive(true);
        if (!OnOff)
            SkillCoolTimeImage[index].transform.parent.gameObject.SetActive(false);
    }
    public void ArrowSkill()
    {
        if (CoolTimeDelta[0] < 0)
        {
            Myinfo.UseRainArrow();
            CoolTimeDelta[0] = Common_Static_Field.ScoutCoolTime;
            StartCoroutine(ArrowSkillSound());
        }
    }
    IEnumerator ArrowSkillSound()
    {
        for (int i = 0; i < 3; i++)
        {
            audio.PlayOneShot(SkillsSound[0]);
            yield return new WaitForSeconds(0.5f);        }
    }
    public void OrcSkill()
    {
        if (CoolTimeDelta[1] < 0)
        {
            Myinfo.Stun();
            CoolTimeDelta[1] = Common_Static_Field.OrcCoolTime;
            audio.PlayOneShot(StunSound);
        }
    }
    public void AngelSkill()
    {
        if (CoolTimeDelta[2] < 0)
        {
            Myinfo.AngelFall();
            CoolTimeDelta[2] = Common_Static_Field.AngelCoolTime;
        }
    }

}
