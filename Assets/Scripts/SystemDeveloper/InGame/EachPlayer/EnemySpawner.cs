using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

struct WaveInfo
{
    RuntimeAnimatorController Anim;
    int Hp;
    float Speed;
}

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner enemySpawner;
    //public Transform[] WayPoints;
    //  List<GameObject> SpawnMonsters = new List<GameObject>();
    PlayerInfo_Master master;
    Wave curWave;
   // Player_Info player;
    public PlayerInfo_Photon Myinfo;
    public int CurEnemyHp{ get { return curWave.Hp; } }
    private void Awake()
    {
        if (enemySpawner == null)
            enemySpawner = this;
       // player = GetComponent<Player_Info>();
        master = FindObjectOfType<PlayerInfo_Master>();
    }
    private void Start()
    {
      //  Myinfo = PlayerInfo_Master.master.MyInfo;
    }

    public void OnePlayer(int ActNum, int SpawnNum)
    {
        StartCoroutine(OnePlayerSpawn(ActNum, SpawnNum));
    }
    IEnumerator OnePlayerSpawn(int ActNum, int SpawnNum)
    {
        int WaveSpawnNum = curWave.MonsterNum+1;
        GameObject curObject;
        PlayerInfo_Photon player=master.infos[ActNum];
        Debug.Log(SpawnNum);
        for (int CurSpawnNum = WaveSpawnNum; CurSpawnNum < SpawnNum+ WaveSpawnNum; CurSpawnNum++)
        {
            Debug.Log(CurSpawnNum);
            curObject = ObjectPull.objectPull.DeQueue(Pull.BaseEnemy);
            Enemy newenemy = curObject.GetComponent<Enemy>();
            if (newenemy == null)
                newenemy = curObject.AddComponent<Enemy>();
            newenemy.Create(player.WayPt, curWave.Hp, curWave.Speed, curWave.anim);
            if (curWave.MonsterNum == 1)
                newenemy.Big = true;
            //newenemy.Create(player.WayPt);
            newenemy.OwnerNum = player.ActorNumber;
            player.ownEnemy++;
            int num = master.EncodeEnemyNumber(player.ActorNumber, CurSpawnNum);
            newenemy.SpawnNum = num;
            if (master.enemys.ContainsKey(num))
                master.enemys[num] = curObject.GetComponent<EnemyHealth>();
            else
                master.enemys.Add(num, curObject.GetComponent<EnemyHealth>());
            yield return new WaitForSeconds(curWave.SpawnTime);
        }
        
    }
    public void Spawn(Wave wave)
    {    curWave = wave;
        //Debug.Log("전달받은 목록");
        //Debug.Log(wave.Hp+"  "+ wave.MonsterNum + "  " + wave.SpawnTime + "  " + wave.Speed);
        //Debug.Log("현제 웨이브정보");
        //Debug.Log(curWave.Hp + "  " + curWave.MonsterNum + "  " + curWave.SpawnTime + "  " + curWave.Speed);
        //    SpawnMonsters.Clear();
        // Debug.Log("Spawn: Ready: false");
        //  player.photon.ReadyToPlayer(false);
        if (Myinfo.IsReady)
        Myinfo.IsReady = false;
        StartCoroutine(SpawnEnemy());  
    }
    IEnumerator SpawnEnemy()
    {
       // Debug.Log("스폰시작");
        GameObject curObject;
        //if (curWave.WaveMonster != null)
            //for (int CurSpawnNum = 0; CurSpawnNum < curWave.MonsterNum; CurSpawnNum++)
            //{
            //    curObject = Instantiate(curWave.WaveMonster);
            //    SpawnMonsters.Add(curObject);
            //    if (curObject.GetComponent<Enemy>() == null)
            //        curObject.AddComponent<Enemy>();
            //    curObject.GetComponent<Enemy>().Create(WayPoints);
            //    yield return new WaitForSeconds(curWave.SpawnTime);
            //}
         //   Debug.Log(curWave.MonsterNum);
            for (int i = 0; i < master.playerInfo_s.Count; i++)
            {
               // Debug.Log(master.playerInfo_s[i]);
               // Debug.Log(master.playerInfo_s[i].ownEnemy);
                master.playerInfo_s[i].ownEnemy = curWave.MonsterNum;
                if (curWave.LastMonster != null)
                    master.playerInfo_s[i].ownEnemy++;
              //  Debug.Log(master.playerInfo_s[i].ownEnemy);
            }
     //   Debug.Log(master.MyInfo.ownEnemy);
        //for (int CurSpawnNum = 0; CurSpawnNum < curWave.MonsterNum; CurSpawnNum++)
        //{
        //    for (int i = 0; i < master.playerInfo_s.Count; i++)
        //    {
        //            curObject = Instantiate(curWave.WaveMonster);
        //            Enemy newenemy = curObject.GetComponent<Enemy>();
        //            if (newenemy == null)
        //                newenemy = curObject.AddComponent<Enemy>();
        //            //if (curObject.GetComponent<EnemyHealth>() == null)
        //            //    curObject.AddComponent<EnemyHealth>();
        //            newenemy.Create(master.playerInfo_s[i].WayPt);

        //            newenemy.OwnerNum = master.playerInfo_s[i].ActorNumber;
        //            //윗줄의 결과는 몬스터가 어느 플레이어의 몬스터인지-> 몬스터가 삭제될때 플레이어에게 콜백되서 플레이어에게 남은 몬스터를 알수 있음
        //            int num= master.EncodeEnemyNumber(master.playerInfo_s[i].ActorNumber, CurSpawnNum);
        //            newenemy.SpawnNum = num;
        //            //윗줄의 결과로 모든 몬스터는 각 로컬에서 생성되지만 모든 로컬에서 생성순서에 따라 모두 같은 번호를 가짐
        //            //또한 몬스터를 검색하기위해서 아래의 if문으로 master에게 각 고유 번호와 함께 딕셔너리형태 전달
        //           // Debug.Log("num : " + num + " ActorNum : " + master.playerInfo_s[i].ActorNumber);
        //            if (master.enemys.ContainsKey(num))
        //                master.enemys[num] = curObject.GetComponent<EnemyHealth>();
        //            else
        //                master.enemys.Add(num, curObject.GetComponent<EnemyHealth>());

        //    }
        //    yield return new WaitForSeconds(curWave.SpawnTime);
        //}
        for (int CurSpawnNum = 0; CurSpawnNum < curWave.MonsterNum; CurSpawnNum++)
        {
            for (int i = 0; i < master.playerInfo_s.Count; i++)
            {
                curObject = ObjectPull.objectPull.DeQueue(Pull.BaseEnemy);
                Enemy newenemy = curObject.GetComponent<Enemy>();
                if (newenemy == null)
                    newenemy = curObject.AddComponent<Enemy>();
                //전달받은 몬스터 정보로몬스터 초기화
                newenemy.Create(master.playerInfo_s[i].WayPt,curWave.Hp,curWave.Speed,curWave.anim);
                if (curWave.MonsterNum == 1)
                {
                    newenemy.Big = true;
                    Ingame_View_Controler.ingame_View_Controler.BossSound();
                }
                newenemy.OwnerNum = master.playerInfo_s[i].ActorNumber;
                //윗줄의 결과는 몬스터가 어느 플레이어의 몬스터인지->
                //몬스터가 삭제될때 플레이어에게 콜백되서 플레이어에게 남은 몬스터를 알수 있음
                int num = master.EncodeEnemyNumber(master.playerInfo_s[i].ActorNumber, CurSpawnNum);
                newenemy.SpawnNum = num;
                //윗줄의 결과로 모든 몬스터는 각 로컬에서 생성되지만 모든 로컬에서 생성순서에 따라 모두 같은 번호를 가짐
                //또한 몬스터를 검색하기위해서 아래의 if문으로 master에게 각 고유 번호와 함께 딕셔너리형태 전달
                // Debug.Log("num : " + num + " ActorNum : " + master.playerInfo_s[i].ActorNumber);
                if (master.enemys.ContainsKey(num))
                    master.enemys[num] = curObject.GetComponent<EnemyHealth>();
                else
                    master.enemys.Add(num, curObject.GetComponent<EnemyHealth>());

            }
            yield return new WaitForSeconds(curWave.SpawnTime);
        }


        //if (curWave.LastMonster!=null)
        //{
        //    for (int i = 0; i < master.playerInfo_s.Count; i++)
        //    {
        //        curObject = Instantiate(curWave.LastMonster);
        //        Enemy newenemy = curObject.GetComponent<Enemy>();
        //        if (newenemy == null)
        //            newenemy=curObject.AddComponent<Enemy>();
        //        newenemy.Big = true;
        //        newenemy.Create(master.playerInfo_s[i].WayPt);
        //        newenemy.OwnerNum = master.playerInfo_s[i].ActorNumber;
        //        master.playerInfo_s[i].ownEnemy++;
        //        //  SpawnMonsters.Add(curObject);
        //        int num = master.EncodeEnemyNumber(master.playerInfo_s[i].ActorNumber, curWave.MonsterNum);
        //        newenemy.SpawnNum = num;
        //        if (master.enemys.ContainsKey(num))
        //            master.enemys[num] = curObject.GetComponent<EnemyHealth>();
        //        else
        //            master.enemys.Add(num, curObject.GetComponent<EnemyHealth>());
        //    }
        //}
        curObject = null;
    }

    //public void DestroyMonster(GameObject monsterForDestroy)
    //{
    //    SpawnMonsters.Remove(monsterForDestroy);
    //    Destroy(monsterForDestroy);
    //    if (SpawnMonsters.Count <= 0)
    //        WhenKillAllMoster();
    //}
    //void WhenKillAllMoster()
    //{
    //    player.GoldChg(Common_Static_Field.WaveEndGold);
    //    if (synergySystem.CurJobSynergyRank[Job.Novel] > 0)
    //        player.GoldChg(synergySystem.CurJobSynergyRank[Job.Novel]);
    //}
    //public void DestroyAllMonster_ForTest()
    //{
    //    GameObject temp;
    //    while (SpawnMonsters.Count > 0)
    //    {
    //        temp = SpawnMonsters[0];
    //        DestroyMonster(temp);
    //    }
    //}

    //public void SpawnReadyButton(Text _ButtonText)
    //{
    //    if (!SpawnReady)
    //    {
    //        if (SpawnMonsters.Count == 0)
    //        {
    //            SpawnReady = true;
    //            player.photon.ReadyToPlayer(true);
    //            _ButtonText.text = "Now_You_are_Ready";
    //        }
    //    }
    //    else
    //    {
    //        SpawnReady = false;
    //        player.photon.ReadyToPlayer(false);
    //        if (SpawnMonsters.Count == 0)
    //            _ButtonText.text = "ToReady";
    //        else
    //            _ButtonText.text = "Waving";
    //    }
    //}
}
