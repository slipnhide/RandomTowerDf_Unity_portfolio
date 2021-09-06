using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public struct ToSimpleJson<T>
{
    public ToSimpleJson(T _value) { value = _value; }
    public T value;
}
public class RangeChampion 
    : Champion
{
    public GameObject Bullet;
    public float BulletSpeed;
    RangeChampionAtk AtkClass;
    public RangeJob Rjob;

    public void Set(RangeJob job, RangeChampion champion,bool IsForSetting=false)
    {
        champion.SetJob((Job)champion.Rjob);
        champion.RequiredGold = Common_Static_Field.ChapmGold[(int)champion.Job, (int)champion.species];
        champion.IsMelee = false;
        //Debug.Log(audio);
        if (audio == null)
            audio = GetComponent<AudioSource>();
        //Debug.Log(audio);
        switch (job)
        {
            case RangeJob.Novel:
                // champion.Damage = 1;
                //  champion.CoolTime = 1f;
                champion.audio.clip = Ingame_View_Controler.ingame_View_Controler.BaseAtackSound[(int)Job.Novel];
                champion.audio.volume = 0.2f;
                champion.BulletSpeed = 4;
              //  champion.Range = 4;
                //champion.MultiTargetNum = 1;
                // champion.RequiredGold = 1;
                break;
            case RangeJob.Mage:
                //  champion.Damage = 1;
                champion.audio.clip = Ingame_View_Controler.ingame_View_Controler.BaseAtackSound[(int)Job.Mage];
                //  champion.CoolTime = 2f;
                champion.audio.volume = 0.5f;
                champion.BulletSpeed = 4;
              //  champion.Range = 3;
               //champion.MultiTargetNum = 3;
                // champion.RequiredGold = 1;
                break;
            case RangeJob.scout:
                //champion.Damage = 1;
                // champion.CoolTime = 1f;
                champion.audio.clip = Ingame_View_Controler.ingame_View_Controler.BaseAtackSound[(int)Job.scout];
                champion.BulletSpeed = 5;
               // champion.Range = 5;
               // champion.MultiTargetNum = 1;
                // champion.RequiredGold = 1;

                break;
        }
        float[] Stats = Common_Static_Field.GetStat(champion.ActorNum, champion.Job, champion.species, champion.Rank);
        champion.Damage = (int)Stats[0];
        champion.MultiTargetNum = (int)Stats[1];
        champion.CoolTime = (float)Stats[2];
        champion.Range = (float)Stats[3];
        champion.AtkClass = RangeChampionAtkPclass.Product(job);
    }

    public override void BeforeSet(int Act)
    {
        ActorNum = Act;
        Set(Rjob, this,true);
        //Debug.Log("Range Set");
    }
    protected override void Awake()
    {
        base.Awake();
     //   Set(Rjob, this);
    }

    public void novelAtk()
    {
        //알고리즘 타직업과 같지만 다른 추가효과등 위해 분리
        //색적한 적을 RPC로 전송
        PlayerInfo_Master.master.MyInfo.TowerAtk(TowerNumber,
            new string[] { JsonUtility.ToJson(targetsNum) });
    }
    [PunRPC]
    public void novelAtkRpc(string[] Maintarget)
    {
        EnemyHealth health;
        targetsNum = JsonUtility.FromJson<TargetsNum>(Maintarget[0]);
        for (int i = 0; i < targetsNum.SpawnNumbers.Length; i++)
        {
            GameObject go;
            go = ObjectPull.objectPull.DeQueue(Job.Novel);
            go.transform.position = transform.position;
            RangeChapBullet temp = go.GetComponent<RangeChapBullet>();
            if (!temp)
                temp = go.AddComponent<RangeChapBullet>();
            health = PlayerInfo_Master.master.FindEnemyHealthWithEnemyNum(targetsNum.SpawnNumbers[i]);
            if (health)
            {
                temp.owner = this;
                temp.SetAll(Damage, Job, species, BulletSpeed, health.gameObject);
                audio.PlayOneShot(audio.clip);
            }
        }
    }
    public void ScoutAtk()
    {
        //Debug.Log("ScoutAtk");
        //알고리즘 타직업과 같지만 다른 추가효과등 위해 분리
        //foreach (Collider2D collider2D in colliders)
        //{
        //    GameObject go = Instantiate(Bullet, transform.position, Quaternion.identity);
        //    go.AddComponent<Bullet>().SetAll(Damage, Job, species, BulletSpeed, collider2D.gameObject);
        //}
        //  Pv.RPC("ScoutAtkRpc", RpcTarget.All, JsonUtility.ToJson(targetsNum));
        PlayerInfo_Master.master.MyInfo.TowerAtk(TowerNumber,
      new string[] { JsonUtility.ToJson(targetsNum) });
    }
    [PunRPC]
    public void ScoutAtkRpc(string[] Maintarget)
    {

        EnemyHealth health;
        targetsNum = JsonUtility.FromJson<TargetsNum>(Maintarget[0]);
        for (int i = 0; i < targetsNum.SpawnNumbers.Length; i++)
        {
            GameObject go;
            go = ObjectPull.objectPull.DeQueue(Job.scout);
            go.transform.position = transform.position;
            RangeChapBullet temp = go.GetComponent<RangeChapBullet>();
            if (!temp)
                temp = go.AddComponent<RangeChapBullet>();
            health = PlayerInfo_Master.master.FindEnemyHealthWithEnemyNum(targetsNum.SpawnNumbers[i]);
            if (health)
            {
                temp.owner = this;
                temp.SetAll(Damage * (int)Common_Static_Field.JobMergeBounus[(int)Job, Rank], Job, species, BulletSpeed, health.gameObject);
                audio.PlayOneShot(audio.clip);
            }
        }
    }
    public void MageAtk()
    {
        //알고리즘 타직업과 같지만 다른 추가효과등 위해 분리
        //foreach (Collider2D collider2D in colliders)
        //{
        //    GameObject go = Instantiate(Bullet, transform.position, Quaternion.identity);
        //    go.AddComponent<Bullet>().SetAll(Damage, Job, species, BulletSpeed, collider2D.gameObject);
        //}
        // Pv.RPC("MageAtkRpc", RpcTarget.All, JsonUtility.ToJson(targetsNum));
        float MagePercente = Common_Static_Field.JobMergeBounus[(int)Job.Mage,Rank];
       // MagePercente = 100;
        int JobSynNum = 0;
        for (int i = 0; i < targetsNum.SpawnNumbers.Length; i++)
        {
            if (Random.Range(0, 100) <= MagePercente)
                JobSynNum++;
        }
       // Debug.Log("To"+JobSynNum+"  "+ JsonUtility.ToJson(new ToSimpleJson<int>(JobSynNum)));
        PlayerInfo_Master.master.MyInfo.TowerAtk(TowerNumber,
     new string[] { JsonUtility.ToJson(targetsNum), JsonUtility.ToJson(new ToSimpleJson<int>(JobSynNum)) });
    }
    [PunRPC]
    public void MageAtkRpc(string[] Maintarget)
    {
        EnemyHealth health;
        targetsNum = JsonUtility.FromJson<TargetsNum>(Maintarget[0]);
        int JobSynergeNum= JsonUtility.FromJson<ToSimpleJson<int>>(Maintarget[1]).value;
        //Debug.Log("Recive" + JobSynergeNum);
        for (int i = 0; i < targetsNum.SpawnNumbers.Length; i++)
        {
            GameObject go;
            go = ObjectPull.objectPull.DeQueue(Job.Mage);
            go.transform.position = transform.position;
            RangeChapBullet temp = go.GetComponent<RangeChapBullet>();
            if (!temp)
                temp = go.AddComponent<RangeChapBullet>();
            health = PlayerInfo_Master.master.FindEnemyHealthWithEnemyNum(targetsNum.SpawnNumbers[i]);
            if (health)
            {
                audio.PlayOneShot(audio.clip);
                temp.owner = this;
                if (JobSynergeNum > 0)
                {
                    temp.SetAll(Damage, Job.Mage, species, BulletSpeed,
                    health.gameObject);
                    audio.PlayOneShot(Ingame_View_Controler.ingame_View_Controler.WindSound);
                }
                else
                    temp.SetAll(Damage, Job.Mage, species, BulletSpeed, health.gameObject, false, true);
            }
            JobSynergeNum--;
        }
    }

    public override void AtkRpc(string[] TargetInfo)
    {
        AtkClass.AttackRpc(this, TargetInfo);
    }

    protected override void ChampionAtk()
    {
        AtkClass.Attack(this);
    }

    public override void SetasRank(int _ActNum,int _Rank = 1)
    {
        Rank = _Rank;
        ActorNum = _ActNum;
        Set(Rjob, this);
    }

    public override void JobSynRpc(string[] TargetInfo)
    {
        throw new System.NotImplementedException();
    }
}
/*
public class Novel2 : RangeChampion
{
    private void Awake()
    {
        Set((RangeJob)job, this);
    }

    private void Update()
    {
        if (CheckEnemyState())
        {
            Atk();
        }
        else
            FindEnemy();
    }

    //public override void Atk()
    //{
    //    GameObject go = Instantiate(Bullet, transform.position, Quaternion.identity);
    //    go.AddComponent<Bullet>().SetAll(Damage, job, species, BulletSpeed, target);
    //}

    public override void FindEnemy()
    {
        Collider2D collider2D = Physics2D.OverlapCircle(transform.position, Range, LayerMask.NameToLayer("Enemy"));
        if (collider2D != null)
            target = collider2D.gameObject;
        else
        {
            IsAttacking = false;
            target = null;
        }
    }
    bool CheckEnemyState()
    {
        //적 공격 가능한 상태인지 전체 판단   적 무적상태,이미 죽은 몬스터 등
        if (target != null)
        {
            if (target.GetComponent<EnemyHealth>().hp < 0)
                return false;
            if (Vector2.Distance(transform.position, target.transform.position) > Range)
                return false;
        }

        return true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsAttacking)
        {
            target = collision.gameObject;
            IsAttacking = true;
        }
    }
}
*/