using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerInfo_Master : MonoBehaviour
{
    public static PlayerInfo_Master master;
    public Text StateText;
    public PhotonView Pv;
    WaveSystem waveSystem;
    bool IsWaving;
    public Text Message;
    [SerializeField]
    public Dictionary<int, PlayerInfo_Photon> infos = new Dictionary<int, PlayerInfo_Photon>();
    public List<PlayerInfo_Photon> playerInfo_s = new List<PlayerInfo_Photon>();
    public PlayerInfo_Photon MyInfo;
    public int EndWavePlayer = 0;
    public int ReadyedPlayer = 0;
    public Dictionary<int, EnemyHealth> enemys = new Dictionary<int, EnemyHealth>();
    public int EncodeEnemyNumber(int actorNum, int SpawnNum) 
    { return (actorNum * 10000) + SpawnNum; }
    public EnemyHealth FindEnemyHealthWithEnemyNum(int EnemyNum) 
    { if (enemys.ContainsKey(EnemyNum)) return enemys[EnemyNum]; else return null; }
    public void Stun(int ActNum)
    {
        int Time = infos[ActNum].CurRank(Species.Orc);
        ParticleSystem StunParticle=ObjectPull.objectPull.Stun;
        StunParticle.transform.position = infos[ActNum].Pos;
        StunParticle.Play();
       // RainArrow.gameObject.transform.position = new Vector3(Pos.x, RainArrow.gameObject.transform.position.y, RainArrow.gameObject.transform.position.z);
        foreach (var enemy in enemys)
        {
            Debug.Log(enemy.Key + "  ActNum:" + ActNum + "  enemy.Key / 10000:" + (int)(enemy.Key / 10000));
            if ((int)(enemy.Key / 10000) == ActNum)
            {
                enemy.Value.GetComponent<Enemy>().Stun(Time);
            }
        }
    }
    public void RainArrow(int ActNum)
    {
        int Dmg = Common_Static_Field.GetJobSynergeValue(ActNum, Job.scout);
        // int Time = infos[ActNum].CurRank(Species.);
        //Debug.Log(((float)Dmg / 100));
        Dmg = (int)(EnemySpawner.enemySpawner.CurEnemyHp * ((float)Dmg / 100));
        //Debug.Log(EnemySpawner.enemySpawner.CurEnemyHp);
        if (Dmg == 0)
            Dmg = 1;
        // Debug.Log(Dmg);
        // KeyValuePair<int, Enemy> keyValuePair =
        Dictionary<int, EnemyHealth>.ValueCollection enemiesvalue = enemys.Values;
        Enemy value;
        List<EnemyHealth> enemies=new List<EnemyHealth>();
        foreach (EnemyHealth health in enemiesvalue)
        {
            value = health.GetComponent<Enemy>();
            if (value.OwnerNum == ActNum)
            {
                enemies.Add(health);
            }
        }
        int Count = enemies.Count;
        for (int i = Count - 1; i >= 0; i--)
        {
            enemies[i].DamageIt(Dmg);
        }

        //foreach (var enemy in enemys)
        //{
        //   // Debug.Log(enemy.Key + "  ActNum:" + ActNum + "  enemy.Key / 10000:" + (int)(enemy.Key / 10000));
        //    if ((enemy.Value.GetComponent<Enemy>().OwnerNum) == ActNum)
        //    {
        //        enemy.Value.DamageIt(Dmg);
        //    }
        //}
    }
    public bool DevilThing(int ActNum)
    {
        if (IsWaving)
        {
            Pv.RPC(nameof(DevilThingRpc), RpcTarget.All, ActNum);
            return true;
        }
        else
            return false;
    }
    [PunRPC]
    void DevilThingRpc(int ActNum)
    {
        EnemySpawner.enemySpawner.OnePlayer(ActNum, 10);
    }
    IEnumerator StartCountDown;
    private void Awake()
    {
        if (master == null)
            master = this;
        Pv = GetComponent<PhotonView>();
        //StateText = Ingame_View_Controler.ingame_View_Controler.test;
    }
    private void OnEnable()
    {
        infos.Clear();
        waveSystem = WaveSystem.waveSystem;
    }
    public void join(int num, PlayerInfo_Photon playerInfo_Photon)
    {
        //각 플레이어 로딩 종료시 플레이어 전체 데이터 관리하는 클래스에등록
        infos.Add(num, playerInfo_Photon);
        playerInfo_s.Add(playerInfo_Photon);
        if (playerInfo_Photon.Pv.IsMine)
            MyInfo = playerInfo_Photon;
        //한 플레이어가 봤을때 전체 플레이어가 등록 하였을때 동시에 게임준비완료
        if (playerInfo_s.Count >= PhotonNetwork.CurrentRoom.PlayerCount)
        {
            //모든 플레이어 마스터 동기화
            //게임 시작 
            Pv.RPC("PlayerJoinComplite", RpcTarget.All);
        }
    }
    [PunRPC]
    void PlayerJoinComplite() { EndWavePlayer++; 
        //모든 플레이어가 각 플레이어의 등록이 완료되었다면 게임시작
        if (EndWavePlayer >= PhotonNetwork.CurrentRoom.PlayerCount)
            StartWait(); }
    [PunRPC]
    void StartWait()
    {
        //  Debug.Log("StartWait");
        //StateText.text = "StartWait";
        StartCountDown = CountDown();
        StartCoroutine(StartCountDown);
        IsWaving = false;
    }
    IEnumerator CountDown()
    {
        MyInfo.WaveEndGold();
        Ingame_View_Controler.ingame_View_Controler.WaveStart();
        yield return new WaitForSecondsRealtime(Common_Static_Field.WaitTime);

        float FreeTimeDelta = 0;
        //Debug.Log(FreeTimeDelta + "  " + Common_Static_Field.FreeTime);
        while (FreeTimeDelta < Common_Static_Field.FreeTime)
        {
            // Debug.Log(FreeTimeDelta);
            if (CheckPlayersReady())
            {
                Ingame_View_Controler.ingame_View_Controler.NomoreWait();
                break;
            }
            FreeTimeDelta += 0.1f;
            yield return new WaitForSecondsRealtime(0.1f);
        }
        CountOver();
    }

    void CountOver()
    {
        Debug.Log("CountOver");
        Pv.RPC(nameof(WaveStart), RpcTarget.All);
    }
    [PunRPC]
    void WaveStart()
    {
        if (!IsWaving)
        {
             Ingame_View_Controler.ingame_View_Controler.NomoreWait();
            StopCoroutine(StartCountDown);
            EndWavePlayer = 0;
            waveSystem.WaveStart();
            IsWaving = true;
        }
    }
    public bool CheckPlayersReady()
    {
       // StateText.text = "";
        bool AllReady = true;
        for (int i = 0; i < playerInfo_s.Count; i++)
        {
           // StateText.text += (playerInfo_s[i].name + playerInfo_s[i].IsReady);
            if (!playerInfo_s[i].IsReady)
            {
                AllReady = false;
                break;
            }
        }
        return AllReady;
    }
    [PunRPC]
    public void EndEnemy()
    {
        EndWavePlayer++;
        if (EndWavePlayer == infos.Count)
            StartWait();
    }

    [PunRPC]
    public void GoldSet(int ActorNumber, int Gold)
    {
        infos[ActorNumber].Gold = Gold;
    }
    //[PunRPC]
    //public void JobSynergyRankChg(int ActorNumber,int[,] vs)
    //{
    //    Debug.Log("디코드 이전");
    //    infos[ActorNumber].CurJobSynergyRank = PlayerInfo_Master.JobSynergyDecord(vs);
    //    Debug.Log("디코드 이후");
    //}
    //[PunRPC]
    //public void CurJobNumChg(int ActorNumber, int[,] vs)
    //{
    //    infos[ActorNumber].CurJobNum = PlayerInfo_Master.JobSynergyDecord(vs);
    //}
    public static int[,] JobSynergyEncord(Dictionary<Job, int> Job_Int)
    {
        int[,] vs = new int[(int)Job.LastNumber, 2];
        for (int y = 0; y < (int)Job.LastNumber; y++)
        {
            vs[y, 0] = y;
            vs[y, 1] = Job_Int[(Job)y];
        }
        return vs;
    }
    public static int[] JobSynergyEncord2(Dictionary<Job, int> Job_Int)
    {
        int[] vs = new int[(int)Job.LastNumber];
        for (int y = 0; y < (int)Job.LastNumber; y++)
        {
            vs[y] = Job_Int[(Job)y];
        }
        return vs;
    }
    public static Dictionary<Job, int> JobSynergyDecord(int[,] Job_Int)
    {
        Dictionary<Job, int> keys = new Dictionary<Job, int>();
        for (int y = 0; y < (int)Job.LastNumber; y++)
        {
            keys.Add((Job)Job_Int[y, 0], Job_Int[y, 1]);
        }
        return keys;
    }
    public static Dictionary<Job, int> JobSynergyDecord2(int[] Job_Int)
    {
        Dictionary<Job, int> keys = new Dictionary<Job, int>();
        for (int y = 0; y < (int)Job.LastNumber; y++)
        {
            keys.Add((Job)y, Job_Int[y]);
        }
        return keys;
    }

    //[PunRPC]
    //public void SpiecesSynergyRankChg(int ActorNumber, int[,] vs)
    //{
    //    infos[ActorNumber].CurSpeciesSynergyRank = PlayerInfo_Master.SpiecesSynergyDecord(vs);
    //}
    //[PunRPC]
    //public void CurSpeciesNumChg(int ActorNumber, int[,] vs)
    //{
    //    infos[ActorNumber].CurSpeciesNum = PlayerInfo_Master.SpiecesSynergyDecord(vs);
    //}
    public static int[,] SpiecesSynergyEncord(Dictionary<Species, int> Species_Int)
    {
        int[,] vs = new int[(int)Species.LastNumber, 2];
        for (int y = 0; y < (int)Species.LastNumber; y++)
        {
            vs[y, 0] = y;
            vs[y, 1] = Species_Int[(Species)y];
        }
        return vs;
    }
    public static int[] SpiecesSynergyEncord2(Dictionary<Species, int> Species_Int)
    {
        int[] vs = new int[(int)Species.LastNumber];
        for (int y = 0; y < (int)Species.LastNumber; y++)
        {
            vs[y] = Species_Int[(Species)y];
        }
        return vs;
    }
    public static Dictionary<Species, int> SpiecesSynergyDecord(int[,] Species_Int)
    {
        Dictionary<Species, int> keys = new Dictionary<Species, int>();
        for (int y = 0; y < (int)Job.LastNumber; y++)
        {
            keys.Add((Species)Species_Int[y, 0], Species_Int[y, 1]);
        }
        return keys;
    }
    public static Dictionary<Species, int> SpiecesSynergyDecord2(int[] Species_Int)
    {
        Dictionary<Species, int> keys = new Dictionary<Species, int>();
        for (int y = 0; y < (int)Species.LastNumber; y++)
        {
            keys.Add((Species)y, Species_Int[y]);
        }
        return keys;
    }
    [PunRPC]
    public void SpiecesSynergyRankChg(int ActorNumber, int _Spieces, int value)
    {
        infos[ActorNumber].CurSpeciesSynergyRank[(Species)_Spieces] = value;
        Debug.Log("JsonPass1");
    }
    [PunRPC]
    public void SpiecesSynergyNumChg(int ActorNumber, int _Spieces, int value)
    {
        infos[ActorNumber].CurSpeciesNum[(Species)_Spieces] = value;
        Debug.Log("JsonPass2");
    }
    public void CheckAllEndMonster()
    {
        EndWavePlayer++;

        if (EndWavePlayer >= PhotonNetwork.CurrentRoom.PlayerCount)
        {
            if (Pv.IsMine)
                Pv.RPC("StartWait", RpcTarget.All);
        }
    }

    public void EnemyDie(Enemy _enemy,int ActNum,bool IsContectEnd)
    {
        if (infos.ContainsKey(ActNum))
        {
            infos[ActNum].ownEnemy--;

            if (IsContectEnd)
                infos[ActNum].LifeDown();
            enemys.Remove(_enemy.SpawnNum);
        }
    }
    //void EnemyDieRpc(Enemy _enemy, int ActNum, bool IsContectEnd)
    //{ 

    //}
    public void LifeDown(int ActNum)
    {

    }
    [PunRPC]
    public void DeleteForMergeRpc(int ActorName, string Towernums)
    {
       Dictionary<int,Champion> chms= infos[ActorName].GetMyChampions();
        ToSimpleJson<int[]> toSimpleJson = JsonUtility.FromJson<ToSimpleJson<int[]>>(Towernums);
        for (int i = 0; i < toSimpleJson.value.Length; i++)
        {
            Destroy(chms[toSimpleJson.value[i]].gameObject);
            infos.Remove(toSimpleJson.value[i]);
        }
    }
}