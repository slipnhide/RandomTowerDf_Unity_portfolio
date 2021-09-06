using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SynergyUpdate
{
   // public GameObject JobSynergyPanel;
   // public GameObject SpeciesSynergyPanel;
    public Dictionary<Job, int> CurJobSynergyRank;
    public Dictionary<Species, int> CurSpeciesSynergyRank;
    public Dictionary<Job, int> CurJobNum;
    public Dictionary<Species, int> CurSpeciesNum;
    // int Gold;
    public SynergyUpdate(Dictionary<Job, int> _CurJobSynergyRank, Dictionary<Species, int> _CurSpeciesSynergyRank, Dictionary<Job, int> _CurJobNum, Dictionary<Species, int> _CurSpeciesNum)
    {
        CurJobSynergyRank = _CurJobSynergyRank;
        CurSpeciesSynergyRank = _CurSpeciesSynergyRank;
        CurJobNum = _CurJobNum;
        CurSpeciesNum = _CurSpeciesNum;
    }
    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}
public class SynergyUpdate2
{
    // public GameObject JobSynergyPanel;
    // public GameObject SpeciesSynergyPanel;
    public int[,] CurJobSynergyRank;
    public int[,] CurSpeciesSynergyRank;
    public int[,] CurJobNum;
    public int[,] CurSpeciesNum;
    // int Gold;

    public SynergyUpdate2(Dictionary<Job, int> _CurJobSynergyRank, Dictionary<Species, int> _CurSpeciesSynergyRank, Dictionary<Job, int> _CurJobNum, Dictionary<Species, int> _CurSpeciesNum)
    {
        CurJobSynergyRank = PlayerInfo_Master.JobSynergyEncord(_CurJobSynergyRank);
        CurSpeciesSynergyRank = PlayerInfo_Master.SpiecesSynergyEncord(_CurSpeciesSynergyRank);
        CurJobNum = PlayerInfo_Master.JobSynergyEncord(_CurJobNum);
        CurSpeciesNum = PlayerInfo_Master.SpiecesSynergyEncord(_CurSpeciesNum);
    }
    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}
public class SynergyUpdate3
{
    // public GameObject JobSynergyPanel;
    // public GameObject SpeciesSynergyPanel;
    public int[] CurJobSynergyRank;
    public int[] CurSpeciesSynergyRank;
    public int[] CurJobNum;
    public int[] CurSpeciesNum;
    // int Gold;

    public SynergyUpdate3(Dictionary<Job, int> _CurJobSynergyRank, Dictionary<Species, int> _CurSpeciesSynergyRank, Dictionary<Job, int> _CurJobNum, Dictionary<Species, int> _CurSpeciesNum)
    {
        CurJobSynergyRank = PlayerInfo_Master.JobSynergyEncord2(_CurJobSynergyRank);
        CurSpeciesSynergyRank = PlayerInfo_Master.SpiecesSynergyEncord2(_CurSpeciesSynergyRank);
        CurJobNum = PlayerInfo_Master.JobSynergyEncord2(_CurJobNum);
        CurSpeciesNum = PlayerInfo_Master.SpiecesSynergyEncord2(_CurSpeciesNum);
    }
    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
}


public struct ChampInfo
{
    public int Rank;
    public Job job;
    public Species species;
    public ChampInfo(int _Rank, Job _job, Species _species) { Rank = _Rank; job = _job; species = _species; }
}

public class SynergySystem : MonoBehaviour
{
    public static SynergySystem synergySystem;

    List<Champion> Champions=new List<Champion>();
    Dictionary<ChampInfo, Champion[]> ChampList=new Dictionary<ChampInfo, Champion[]>();
    ChampInfo key = new ChampInfo();
    public GameObject JobSynergyPanel;
    public GameObject SpeciesSynergyPanel;
    public PlayerInfo_Photon photon = null;
    int[,,] curChampList = new int[Common_Static_Field.MaxMerge,(int)Job.LastNumber, (int)Species.LastNumber];
    int[] curJobRank = new int[(int)Job.LastNumber];
    int[] curSpeciesRank = new int[(int)Species.LastNumber];
    private void Awake()
    {
        // Set();
        if (synergySystem == null)
            synergySystem = this;
        else if (synergySystem != null)
            Destroy(this);
    }
    int CheckSynergy(Job _ChgedJob)
    {
        #region Job숫자체크
        int CurJobNum = 0;
        int JobNum = (int)_ChgedJob;
        int JobRank = 0;
        for(int EachRank=0; EachRank < Common_Static_Field.MaxMerge; EachRank++)
        for (int i = 0; i < (int)Species.LastNumber; i++)
        {
            CurJobNum += curChampList[EachRank,JobNum, i];
        }
       // Debug.Log("JobCheck Num" + CurJobNum);
        #endregion
        #region Job랭크확인
        int[] index = Common_Static_Field.JobSynergyRank[_ChgedJob];
        for (int i = index.Length - 1; i >= 0; i--)
        {
            if (CurJobNum >= index[i])
            {
                JobRank = i+1;
                break;
            }
        }
        #endregion
      //  Debug.Log("JobCheck Rank" + JobRank);
        return JobRank;
    }
    int CheckSynergy(Species _ChagedSp)
    {
        #region Job숫자체크
        int CurSpeciesNum = 0;
        int SpeciesNum = (int)_ChagedSp;
        int SpeciesRank = 0;
        for (int EachRank = 0; EachRank < Common_Static_Field.MaxMerge; EachRank++)
            for (int i = 0; i < (int)Job.LastNumber; i++)
            {
            CurSpeciesNum += curChampList[EachRank,i, SpeciesNum];
            }
        #endregion
        #region Job랭크확인
        int[] index = Common_Static_Field.SpeciesSynergyRank[_ChagedSp];
        for (int i = index.Length-1; i >= 0; i--)
        {
            if (CurSpeciesNum >= index[i])
            {
                SpeciesRank = i+1;
                break;
            }
        }
        #endregion
        return SpeciesRank;
    }
    public void TowerNumChg(Champion tower, bool IsAdd, int Rank=1)
    {
        int JobRankChg = -1;
        int speciesRankChg = -1;
        if (IsAdd)
        {
            curChampList[Rank-1,(int)tower.Job, (int)tower.species]++;
            Champions.Add(tower);
        }
        else
        {
            curChampList[Rank - 1, (int)tower.Job, (int)tower.species]--;
            Champions.Remove(tower);
        }


        int Check = CheckSynergy(tower.Job);
        if (curJobRank[(int)tower.Job] != Check)
        {
            JobRankChg = Check;
            curJobRank[(int)tower.Job] = Check;
        }
      //  Debug.Log("Job" + "JobRankChg : " + JobRankChg + "  "+Check);
        Check = CheckSynergy(tower.species);
        if (curSpeciesRank[(int)tower.species] != Check)
        {
            speciesRankChg = Check;
            curSpeciesRank[(int)tower.species] = Check;
        }
        //Debug.Log("species" + "speciesRankChg : " + speciesRankChg + "  " + Check);
        if (JobRankChg != -1 || speciesRankChg != -1)
        {
            photon.ChgRank(JobRankChg, (int)tower.Job, speciesRankChg, (int)tower.species);
            if (tower.Job == Job.scout)
            {
                if(curJobRank[(int)Job.scout] > 0)
                Ingame_View_Controler.ingame_View_Controler.SkiilOnOff(0, true);
                else
                    Ingame_View_Controler.ingame_View_Controler.SkiilOnOff(0, false);
            }
            if (tower.species == Species.Orc)
            {
                if (curSpeciesRank[(int)Species.Orc] > 0)
                    Ingame_View_Controler.ingame_View_Controler.SkiilOnOff(1, true);
                else
                    Ingame_View_Controler.ingame_View_Controler.SkiilOnOff(1, false);
            }
            if (tower.species == Species.Angel)
            {
                if (curSpeciesRank[(int)Species.Angel] > 0)
                    Ingame_View_Controler.ingame_View_Controler.SkiilOnOff(2, true);
                else
                    Ingame_View_Controler.ingame_View_Controler.SkiilOnOff(2, false);
            }

        }

    }
    [ContextMenu("NewCheckNum")]
    public void NumCheck()
    {
        string C = "";
        for (int EachRank = 0; EachRank < Common_Static_Field.MaxMerge; EachRank++)
        {
            for (int j = 0; j < (int)Job.LastNumber; j++)
            {
                C = ((Job)j).ToString() + " : ";
                for (int s = 0; s < (int)Species.LastNumber; s++)
                {
                    C += curChampList[EachRank,j, s].ToString() + " ";
                }
                Debug.Log(C);
            }
        }
    }
    public bool IfAddChampIsMerged(int[] Job_Spicies, int Rank)
    {
        int NumIndeck = Ingame_View_Controler.ingame_View_Controler.FindDeckSlotNum(Job_Spicies, Rank);
        if (curChampList[Rank-1, Job_Spicies[0], Job_Spicies[1]]+ NumIndeck >= Common_Static_Field.MergeNum)
            return true;
        else
            return false;
    }

    public void DestroyRank(int[] Info, int Rank, Transform _transform)
    {
        Dictionary<int, Champion> chmps = photon.GetMyChampions();
        Species Findspecies = (Species)Info[1];
        Job FindJob = (Job)Info[0];
        int FindTime = 0;
        DeckSlot[] deckSlots = Ingame_View_Controler.ingame_View_Controler.FindDeckSlot(Info, Rank);
        int DeckSlotNum = deckSlots.Length;
        Debug.Log(DeckSlotNum);
        if(DeckSlotNum>0)
        for (int i = 0; i < DeckSlotNum; i++)
        {
               // Debug.Log(deckSlots[0]);
                 deckSlots[i].ToEmpty();
                   FindTime++;
        }

        List<Champion> deleteList = new List<Champion>();
        // KeyValuePair<int, Champion>. temp2 
        Debug.Log(_transform.position);
        if (FindTime < Common_Static_Field.MergeNum)
        {
            Dictionary<int, Champion>.ValueCollection temp3 = chmps.Values;
            foreach (Champion champion in temp3)
            {
                Debug.Log(champion.species + "  " + champion.Job + "  " + champion.Rank + "  ");
                if (champion.species == Findspecies && champion.Job == FindJob && champion.Rank == Rank)
                {
                    Debug.Log("필드 삭제 등록");
                    FindTime++;
                    //삭제 이벤트
                    deleteList.Add(champion);
                    if (FindTime >= Common_Static_Field.MergeNum)
                        break;
                }
            }
        }
        List<int> TowerNums = new List<int>();
        for (int i = deleteList.Count - 1; i >= 0; i--)
        {
            TowerNums.Add(deleteList[i].TowerNumber);
        }
        ToSimpleJson<int[]> toSimpleJson = new ToSimpleJson<int[]>();
        toSimpleJson.value = TowerNums.ToArray();
        PlayerInfo_Master.master.MyInfo.DisableChampion(JsonUtility.ToJson(toSimpleJson),true,_transform.position);
       // PlayerInfo_Master.master.Pv.RPC(nameof(PlayerInfo_Master.master.DeleteForMergeRpc),Photon.Pun.RpcTarget.All,photon.ActorNumber, JsonUtility.ToJson(toSimpleJson));
    }
}


