using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public enum Job { Warrior, assassin,scout, Mage, Novel, LastNumber }
public enum RangeJob { scout = 2, Mage = 3, Novel = 4, LastNumber }
public enum MeleeJob { Warrior = 0, assassin = 1, LastNumber }
public enum Species { Angel, Demon, Human, Machine,MerMaid, Orc,Undead,LastNumber }

public enum StatNum { Dmg,TargetNum,AtkSpeed,Range,LastNumber }
public struct floatArray
{ public float[] array; }
public struct intArray
{ public int[] array; }

[System.Serializable]
public struct RankPerLv
{
    public int[] Rank_Percentage;
    #region 초기화
    public RankPerLv(int[] array)
    {
        Rank_Percentage = array;
    }
    #endregion
}


//public struct Wave
//{
//    public GameObject WaveMonster;
//    public GameObject LastMonster;
//    public int MonsterNum;
//    public float SpawnTime;
//}
public class Common_Static_Field : MonoBehaviour
{
    public static PlayerInfo_Photon MyInfo;

    public static int StartLife = 999;
     public static int MaxLv = 5;
    public  const int MaxMerge = 3;
    public static int MergeNum = 3;
    //public static int TowerGold_MaxRange = 5;
    public static int RollPay { get { return 1; } }
    public static int WaveEndGold { get { return 999; } }
    public static int XpPay { get { return 1; } }

    //0~3 -> 기본 1단계 -~4단계 까지
    public readonly static float[] RankBonus = new float[] {1.2f, 1.5f, 1.8f };
    public const float DevilCoolTime = 60;
    public const float ScoutCoolTime = 90;
    public const float OrcCoolTime = 30;
    public const float AngelCoolTime = 30;
    public static float WaitTime = 10;
    public static float FreeTime = 10;
    public static int ChanceInstaceKill = 10;
    public static int NovelGold = 10;

    //리롤비용-> 차후에 리롤비용 유동적으로 바뀐다면 별도의 함수를 만들고 프로퍼티로 변경
    #region 직업과,종족 시너지 단계 조정, 가챠시 플레이어 레벨별 티어 등장확률 조정, (하게된다면) 직업x종족별 데미지, 사정거리등 스텟조정 공간 (더블클릭)
    public static Dictionary<Job, int[]> JobSynergyRank = new Dictionary<Job, int[]>()
    {
         //Job,Species  동일하게 첫번째 요소부터 첫단계 시너지 발동에 필요한 갯수 
            {Job.Warrior, new int[]   { 3, 6, 7 } },
            {Job.scout, new int[]     { 3, 6, 7 } },
            {Job.Mage, new int[]      { 3, 6, 7 } },
            {Job.assassin, new int[]  { 3, 6, 7 } },
            {Job.Novel, new int[]     { 3, 6, 7 } }
    };
    public static Dictionary<Species, int[]> SpeciesSynergyRank = new Dictionary<Species, int[]>()
    {
        //Job,Species  동일하게 첫번째 요소부터 첫단계 시너지 발동에 필요한 갯수 
            {Species.Human, new int[]   { 2 ,4, 5 } },
            {Species.Undead, new int[]  { 2 ,4, 5 } },
            {Species.MerMaid, new int[] { 2 ,4, 5 } },
            {Species.Orc, new int[]     { 2 ,4, 5 } },
            {Species.Machine, new int[] { 2 ,4, 5 } },
            {Species.Angel, new int[]   { 2 ,4, 5 } },
            {Species.Demon, new int[]   { 2 ,4, 5 } }
    };
    public static RankPerLv[] rankPerLvs = new RankPerLv[]
    {
            //위에서부터 1래벨때 확률 타워의 랭크가 올라가면 뒤에 이어서 붙이면됨,
            //레벨또한 아래에 추가로 붙이면 늘리기 가능
            //상위티어 를 추가한다면 해당 레벨에 맞는 타워도 추가해야함,
            //현제는 해당레벨에 타워가 없다면 해당 타워 확률이 있어도 아래단계로 가챠가 시도됨
            new RankPerLv(new int[]{ 70, 30, 0 , 0 , 0  } ),
            new RankPerLv(new int[]{ 60, 30, 10, 0 , 0  } ),
            new RankPerLv(new int[]{ 50, 40, 10, 0 , 0  } ),
            new RankPerLv(new int[]{ 40, 40, 15, 5 , 0  } ),
            new RankPerLv(new int[]{ 30, 40, 15, 10, 5  } )
    };
    public readonly static float[,] JobMergeBounus = new float[(int)Job.LastNumber,4]
    {
            {0,2,4,6},
            {5,6,7,9},
            {1,1.2f,1.5f,1.8f},
            {10,20,30,40,},
            {0,5,10,15}
    };
    public readonly static float[,] SpeciesMergeBounus = new float[(int)Species.LastNumber,4]
{
            {0,0,0,0},
            {0,0,0,0},
            {0,0,0,0},
            {0,0,0,0},
            {1,1.5f,2,2.5f},
            {0,0,0,0},
            {1,1.5f,2,2.5f}
};
    public readonly static int[,] JobStat = new int[(int)Job.LastNumber, (int)StatNum.LastNumber]
        {
            //스텟 점수 순서
            //공격력,멀티타겟,공격쿨타임,사거리
            {1,2,1,1},
            {8,0,8,1},
            {1,1,1,1},
            {4,2,3,2},
            {2,1,2,2}
        };
    readonly static int[,] SpeciesStat 
        = new int[(int)Species.LastNumber, (int)StatNum.LastNumber]
    {
            {4,2,1,4},
            {9,3,3,2},
            {2,0,2,0},
            {2,1,3,1},
            {3,0,1,3},
            {2,0,2,1},
            {1,2,1,1}
    };
    readonly static int[,] JobSynergeValue= new int[(int)Job.LastNumber, 4] 
    {
        {0, 2,4,8 },
        {0,2,4,8 },
        {0,5,8,10 },
        {0,1,2,4 },
        {1,8,10,13,}
    };
    public static int GetJobSynergeValue(int ActNum,Job _job){
      //  Debug.Log(PlayerInfo_Master.master.infos[ActNum].CurRank(_job));
      //  Debug.Log(JobSynergeValue[(int)_job, PlayerInfo_Master.master.infos[ActNum].CurRank(_job)]);
        return JobSynergeValue[(int)_job,PlayerInfo_Master.master.infos[ActNum].CurRank(_job)];  }
    public readonly static float[,] SpiciesSynergeValue = new float[(int)Species.LastNumber, 4]
    {
        {0,0.15f,0.25f,0.35f},
        {0 ,0.2f,0.3f,0.5f},
        { 0,1.1f,1.2f,1.3f},
        {1,1.1f,1.2f,1.3f},
        {0,0.05f,0.08f,0.12f},
        {0,1,2,4},
        { 999,1,0.7f,0.3f}
    };
    public static float GetSpSynergeValue(int ActNum, Species _species) { return SpiciesSynergeValue[(int)_species, PlayerInfo_Master.master.infos[ActNum].CurRank(_species)]; }
    public static float[] GetStat(int ActNum,Job _job, Species _Sp,int Rank)
    {

        float machinevalue = GetSpSynergeValue(ActNum,Species.Machine);
        if (Rank > 1)
            machinevalue *= RankBonus[Rank-1];
        float[] values =new float[4];
        //데미지
        values[0] = JobStat[(int)_job, 0]+  SpeciesStat[(int)_Sp,0];
        values[0] *= machinevalue;
        //멀티타겟
        values[1] = JobStat[(int)_job, 1] + SpeciesStat[(int)_Sp, 1];
        values[1] *= machinevalue;
        values[1] *= 0.5f;
        values[1] += 1;
        //쿨타임
        values[2] = JobStat[(int)_job, 2] + SpeciesStat[(int)_Sp, 2];
        values[2] *= machinevalue;
        values[2] = 10 / (values[2] * 3);
        //사거리
        values[3] = JobStat[(int)_job, 3] + SpeciesStat[(int)_Sp, 3];
        values[3] *= machinevalue;
        values[3] = 2 + values[3]*0.5f;
        string v = "";
        for (int i = 0; i < 4; i++)
        {
            v += values[i].ToString() + "  ";
        }
       // Debug.Log(v);
        return values;

    }

    public static int[,,] AtkSpeed = new int[MaxMerge, (int)Job.LastNumber, (int)Species.LastNumber]
    {
        {
            {1,1,2,2,2,3,3},
            {1,2,2,2,2,3,4},
            {2,2,2,3,3,4,4},
            {2,2,3,3,4,5,5},
            {3,3,3,4,5,5,5}
        },
        {
            {1,1,2,2,2,3,3},
            {1,2,2,2,2,3,4},
            {2,2,2,3,3,4,4},
            {2,2,3,3,4,5,5},
            {3,3,3,4,5,5,5}
        },
        {
            {1,1,2,2,2,3,3},
            {1,2,2,2,2,3,4},
            {2,2,2,3,3,4,4},
            {2,2,3,3,4,5,5},
            {3,3,3,4,5,5,5}
        }
    };
    #endregion

    public readonly static int[] JobRequiredGold = new int[] { 2, 5, 1, 4, 3 };
    public static Job[] FindJobRequiredGold(int Gold)
    {
        List<Job> a = new List<Job>();
        for (int i = 0; i < JobRequiredGold.Length; i++)
            if (Gold == JobRequiredGold[i])
                a.Add((Job)i);
        return a.ToArray();
    }
    public readonly static int[] SpeciesRequiredGold = new int[] {4,5,1,3,3,2,2 };
    public static Species[] FindSpeciesRequiredGold(int Gold)
    {
        List<Species> a = new List<Species>();
        for (int i = 0; i < SpeciesRequiredGold.Length; i++)
            if (Gold == SpeciesRequiredGold[i])
                a.Add((Species)i);
        return a.ToArray();
    }
    public static int[,] ChapmGold = new int[(int)Job.LastNumber, (int)Species.LastNumber]
    {
        {6,7,3,5,5,4,4},
        {9,10,6,8,8,7,7},
        {5,6,2,4,4,3,3},
        {8,9,5,7,7,6,6},
        {7,8,4,6,6,5,5}
    };
    public static int[][] FindChampForGold(int RequiredGold)
    {
        List<int[]> value=new List<int[] > ();
        for (int spiceis = 0; spiceis < (int)Species.LastNumber; spiceis++)
        {
            for (int job = 0; job < (int)Job.LastNumber; job++)
            {
                if (ChapmGold[job, spiceis] == RequiredGold)
                    value.Add(new int[2] { job, spiceis });
            }
        }
        return value.ToArray();
    }
}


