using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingame_View_Model : MonoBehaviour
{
    public static Ingame_View_Model ingame_View_Model;
    //public SynergySystem synergySystem;
    public Transform RollTowerPanel;
    //초기화 이후 player_Info 쪽에서 Is Mine이면 초기화
    //  public Player_Info player_Info;
    public PlayerInfo_Photon playerInfo;
    //public TowerListPerRank[] towerListPerRanks;
    public GameObject[] champions;
    // public WaveSystem system;
    public float PlayerXpPercentate { get { return playerInfo.XpPercentate; } }
    public int CheckGold { get { return playerInfo.Gold; } }

    private void Awake()
    {
        if (ingame_View_Model == null)
        {
            ingame_View_Model = this;
        }
        else if (ingame_View_Model != this)
        {
            Destroy(gameObject);
        }

    }
    public void ChampSet()
    {
        foreach (GameObject go in champions)
        {

            go.GetComponent<Champion>().BeforeSet(playerInfo.ActorNumber);
        }
    }

    public int[][] Roll()
    {
        int[][] RollTowers;
        //돈이 부족하다면 실패 충분하다면 진행
        {
            if (RollTowerPanel != null)
            {
                if (RollTowerPanel.childCount == 0)
                { Debug.LogWarning("롤 판넬 버튼 비어있음"); return null; }

            }
            else return null;
            RollTowers = new int[RollTowerPanel.childCount][];

            for (int index = 0; index < RollTowers.Length; index++)
            {
                #region 방법3-플레이어 레벨의 확률에 따른 등장 골드 선택
                int SelectedGold = RollSelGold();
                if (SelectedGold <= 0 || SelectedGold > 5) Debug.LogError("골드가 범위를 넘어감 SelectedGold : " + SelectedGold);
                #endregion
                #region 선택골드에 해당하는 직업과 종족 각각 찾음
                //골드에 맞는 종족, 직업들을 가져온뒤 랜덤으로 선택
                Job[] Seljobs = Common_Static_Field.FindJobRequiredGold(SelectedGold);
                Job Seljob = Seljobs[Random.Range(0, Seljobs.Length)];
                SelectedGold = RollSelGold();
                if (SelectedGold <= 0 || SelectedGold > 5) Debug.LogError("골드가 범위를 넘어감 SelectedGold : " + SelectedGold);
                Species[] SelSpeciess = Common_Static_Field.FindSpeciesRequiredGold(SelectedGold);
                Species SelSpecies = SelSpeciess[Random.Range(0, SelSpeciess.Length)];
                #endregion
                RollTowers[index] = new int[2] { (int)Seljob, (int)SelSpecies };

                #region 방법3-골드에 맞는 직업과 종족 선택
                /*
                //Job Seljob = Job.LastNumber; Species SelSpecies = Species.LastNumber;
                //혹시 찾는 골드 단위의 타워가 없다면 아래 단계 골드로 재 검색
                while (true)
                {
                    if (SelectedGold <= 0) break;
                    //int[?][2] 의 형태로 각 [찾은순서][직업,종족] 을 가르키는 희소행렬을 받아옴

                    int[][] FinedType = Common_Static_Field.FindChampForGold(SelectedGold);
                    //  Debug.Log("찾아야될 골드:SelectedGold"+ SelectedGold);
                    //  Debug.Log(FinedType.Length);
                    if (FinedType.Length <= 0) { SelectedGold--; continue; }
                    else
                    {
                        int SelType = Random.Range(0, FinedType.Length);
                        Seljob = (Job)FinedType[SelType][0];
                        SelSpecies = (Species)FinedType[SelType][1];
                        break;
                    }

                }
                */

                #endregion

                #region 방법3-선택한 직업과 종족에 맞게 게임 오브젝트 찾아 RollTowers에전달(차후에 애니메이션 스프라이트등 전체를 Awake에서 Set하게 된다면 클래스만 전달로 간소화)
                //for (int i = 0; i < champions.Length; i++)
                //{
                //    if (champions[i].GetComponent<MeleeChampion>())
                //    {
                //        if (champions[i].GetComponent<MeleeChampion>().Mjob == (MeleeJob)Seljob && champions[i].GetComponent<Champion>().species == SelSpecies)
                //            RollTowers[index] = champions[i];
                //    }
                //    else if (champions[i].GetComponent<RangeChampion>())
                //    {
                //        if (champions[i].GetComponent<RangeChampion>().Rjob == (RangeJob)Seljob && champions[i].GetComponent<Champion>().species == SelSpecies)
                //            RollTowers[index] = champions[i];
                //    }
                //}
                #endregion
                #region 방법2
                /*
                int SelectedRank = 0;
                RankPerLv CurPercentage = Common_Static_Field.rankPerLvs[playerInfo.lv];
                int MaxPercentage = 0;
                int length = CurPercentage.Rank_Percentage.Length;
                for (int i = 0; i < length; i++)
                {
                    MaxPercentage += CurPercentage.Rank_Percentage[i];
                }
                int random = Random.Range(0, MaxPercentage);
                for (int i = length - 1; i > -1; i--)
                {
                    if (random <= CurPercentage.Rank_Percentage[i])
                        SelectedRank = i;
                }
                */
                #endregion
                #region 방법1
                /*
                //현제 플레이어 레벨에 따른 타워등급별 확률 가져옴
                RankPerLv RankPercent = Common_Static_Field.rankPerLvs[player_Info.PlayerLv];
                //현제 레벨에 세팅되어있는 타워의 등급 개수
                int Langht = RankPercent.Rank_Percentage.Length;
                if (RankPercent.Rank_Percentage == null || Langht == 0)
                    Debug.LogError("RollFuctionError_ RankPercent Is Not Setted curPlayerLv:" + player_Info.PlayerLv);

                //주사위를 굴려서 높은 등급부터 확인하고 해당 등급이 안뽑혓다면 확률 누적하여 아래등급확인
                int SelectedRank = 0; //선택된 등급
                int CurPercent = 0;                //누적 확률                              
                int Randomize = Random.Range(0, 100);//주사위를 굴려서 나온확률

                for (int i = Langht - 1; i > -1; i--)
                {
                    CurPercent += RankPercent.Rank_Percentage[i];
                    // Debug.Log("i:" + i + "  CurPercent" + CurPercent);
                    if (Randomize < CurPercent)
                    {
                        SelectedRank = i;
                        //선택된 등급에 타워가 존재하지않는다면 오류 방출하고 아래 단계 타워로 진행
                        if (TowerList.towerList.TowerPerRank[SelectedRank].Towers.Length == 0)
                        {
                            Debug.LogWarning("RollFuctionError: TowerList Is Empty _ CurRank:" + (SelectedRank));
                            continue;
                        }
                        else
                        {
                            break; //현제 등급에 타워가 있다면 현제 등급의 타워로 정하고 탈출
                        }
                    }
                }
                */
                #endregion
                #region 방법1,2 의 후처리
                /*
                //현제 등급의 타워중 랜덤한 타워 선택
                //int RandomTower = Random.Range(0, TowerList.towerList.TowerPerRank[SelectedRank].Towers.Length);
                //RollTowers[index] = TowerList.towerList.TowerPerRank[SelectedRank].Towers[RandomTower].GetComponent<Tower>();
                int RandomTower = Random.Range(0,towerListPerRanks[SelectedRank].Towers.Length);
                RollTowers[index] = towerListPerRanks[SelectedRank].Towers[RandomTower].GetComponent<Champion>();
                */
                #endregion
            }
        }
        return RollTowers;
    }

    public int RollSelGold()
    {
        int SelectedGold = 0;
        //현제 레벨의 골드별 뽑기확률
        RankPerLv CurPercentage = Common_Static_Field.rankPerLvs[playerInfo.lv - 1];
        int MaxPercentage = 0;
        int length = CurPercentage.Rank_Percentage.Length;
        //현제 레벨에 나올수있는 전체 수치 구함
        for (int i = 0; i < length; i++)
            MaxPercentage += CurPercentage.Rank_Percentage[i];
        //전체 수치중 일부를 고름
        int random = Random.Range(1, MaxPercentage);

        MaxPercentage = 0;
        //가장 높은곳(가장 낮은확률)부터 뽑기 검사
        for (int i = length - 1; i > -1; i--)
        {
            MaxPercentage += CurPercentage.Rank_Percentage[i];
            if (random <= MaxPercentage)
            {
                SelectedGold = i + 1;
                break;
            }
            //인덱스는 0 부터 시작 해서 마지막 0인덱스를 기준으로 잡으면 0원 골드를 찾을 수가 없음
        }
        return SelectedGold;
    }
        public bool Buy(int NeedsGold)
    {

        if (playerInfo.TryGoldChg(NeedsGold * -1))
        {
            return true;
        }
        return false;
    }

    public bool BuyXp()
    {
       // if (playerInfo.lv >= 5) return false;
        if (!playerInfo.TryGoldChg(Common_Static_Field.XpPay * -1)) return false;
        else
            playerInfo.AddXp();
        return true;
    }

    public bool SpawnTower(int[] Job_Sp, Tile _Tile, Vector3 Pos, int _Rank = 1)
    {
        //모델에서 해당 타워가 프리팹으로 존재하는지 확인하거나 오브젝트로 변환하여 생성
        //기타 모든검사는 덱슬롯 쪽으로 처리위임
        //생성시 일어나야할 이미지등 처리는 ViewControler에서 , 데이터 처리는 모델에서 처리
        GameObject towerPrefab = Ingame_View_Model.ingame_View_Model.FindTower(Job_Sp);
        if (towerPrefab != null)
        {
            Create(towerPrefab, Pos, _Rank);
            _Tile.IsBuildTower = true;
            //synergySystem.AddTower(towerPrefab.GetComponent<Champion>());
            SynergySystem.synergySystem.TowerNumChg(towerPrefab.GetComponent<Champion>(), true, _Rank);
            return true;
        }
        else
        { 
            Debug.LogError("방금 설치하려한 타워를 타워리스트에서 찾을수 없습니다.");
            return false;
        }
    }
    void Create(GameObject _tower, Vector3 Pos, int _Rank = 1)
    {
        //타워 Pv없에고 플레이어인포에서 전체 처리한다면
          playerInfo.Create(_tower, Pos, _Rank);
       // PhotonNetwork.Instantiate("Towers/" + _tower.name, Pos,Quaternion.identity);
    }
    public GameObject FindTower(int[] Job_Sp)
    {
        Champion temp;
       // Debug.Log("찾고자 하는 직업: " + (Job)Job_Sp[0] + "찾고자 하는 종족: " + (Species)Job_Sp[1]);
        for (int i = 0; i < champions.Length; i++)
        {
            temp = champions[i].GetComponent<Champion>();
            if (temp.species == (Species)Job_Sp[1] && temp.Job == (Job)Job_Sp[0])
            {
               // Debug.Log("찾은 직업: " + temp.Job + "찾은 종족: " + temp.species);
                return champions[i];
            }
        }
        return null;
    }
    public GameObject FindTower(string _TowerName)
    {
        for (int i = 0; i < champions.Length; i++)
        {
            if (champions[i].name.Equals(_TowerName))
                return champions[i];
        }
        return null;
    }
}
