using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public struct TargetsNum {public int[] SpawnNumbers; }

public class MeleeChampion : Champion, IPunObservable
{
    // public GameObject boxCollider;
    protected BoxBullet box;
    MeleeChampionAtk AtkClass;
    public MeleeJob Mjob;
    ParticleSystem AtkParicle;
    public int AtkTimes = 0;
    public MeleeChampion(int _Rank) { Rank = _Rank; }

    public void Set(MeleeJob job, MeleeChampion champion, bool IsForSetting = false)
    {
        champion.SetJob((Job)champion.Mjob);
        champion.RequiredGold = Common_Static_Field.ChapmGold[(int)champion.Job, (int)champion.species];
        champion.IsMelee = true;
        float[] Stats = Common_Static_Field.GetStat
            (champion.ActorNum, champion.Job, champion.species, champion.Rank);
        champion.Damage = (int)Stats[0];
        champion.MultiTargetNum = (int)Stats[1];
        champion.CoolTime = (float)Stats[2];
        champion.Range = (float)Stats[3];
        if (audio == null)
            audio=GetComponent<AudioSource>();
        switch (job)
        {
            case MeleeJob.assassin:
                champion.audio.clip = Ingame_View_Controler.ingame_View_Controler.BaseAtackSound[(int)Job.assassin];
                champion.audio.volume = 0.3f;
                break;
            case MeleeJob.Warrior:
                champion.audio.clip = Ingame_View_Controler.ingame_View_Controler.BaseAtackSound[(int)Job.Warrior];
                champion.AtkParicle = ObjectPull.objectPull.Warrior;
                break;
        }
        if (!IsForSetting)
            champion.AtkClass = MeleeChampionAtkPclass.Product(champion.Mjob);
    }
    public override void BeforeSet(int Actnum)
    {
        ActorNum = Actnum;
        Set(Mjob, this, true);

        // Debug.Log("Melee Set");
    }
    protected override void Awake()
    {
        base.Awake();
        Set(Mjob, this);
    }
    public override void SetasRank(int _ActNum,int _Rank = 1)
    {
        Rank = _Rank;
        ActorNum = _ActNum;
        Set(Mjob, this);
    }
    public void WorrierAtk()
    {
        //targetsNum.SpawnNumbers = new int[colliders.Count];
        //for (int i = 0; i < colliders.Count; i++)
        //    targetsNum.SpawnNumbers[i] = colliders[i].GetComponent<Enemy>().SpawnNum;
        //Pv.RPC("WorrierAtkRpc", RpcTarget.All, new string[] { JsonUtility.ToJson(targetsNum) });
        AtkTimes++;
        if (AtkTimes > 10)
        {
            AtkTimes = 0;
          //  Debug.Log("전사 시너지 발도용청");
            PlayerInfo_Master.master.MyInfo.JobSynAtk(TowerNumber, new string[] { JsonUtility.ToJson(targetsNum) });
        }
        PlayerInfo_Master.master.MyInfo.TowerAtk(TowerNumber,
new string[] { JsonUtility.ToJson(targetsNum) });
    }
    [PunRPC]
    public void WorrierAtkRpc(string[] targetsNumbersFromJson)
    {
        AtkParicle.transform.position = transform.position;
        ParticleSystem.MainModule MM= AtkParicle.main;
        MM.startSize = Range;
        AtkParicle.Play();
        //  Debug.Log(targetsNumbersFromJson);
        TargetsNum damagedtarget= JsonUtility.FromJson<TargetsNum>(targetsNumbersFromJson[0]);
        //targetsNum = JsonUtility.FromJson<TargetsNum>(targetsNumbersFromJson[0]);
        //1차 마이 인포에서 해당 번호를 가진 적 helth나 enemy 를 가져옴
        //2차 해당 적에게 직접 공격 을 날리거나 직접 데미지 시행
        EnemyHealth health;
        for (int i = 0; i < damagedtarget.SpawnNumbers.Length; i++)
        {
            health = PlayerInfo_Master.master.FindEnemyHealthWithEnemyNum(damagedtarget.SpawnNumbers[i]);
            if (health)
            {
                health.DamageIt(Damage);
                health.Synergy(Job, species, this);
                audio.PlayOneShot(audio.clip);
            }
        }

    }
    public void assasineAtk()
    {
        TargetsNum DamagedTarget;
        List<int> DamagedTargetList = new List<int>();
        //암살자도 여러 마리 공격할수있다는 전제로
        #region 암살자 공격 주변도 공격 하는 형태
        /*
        foreach (Collider2D collider2 in colliders)
        {
            Vector2 Enemy = collider2.transform.position;
            Vector2 Me = transform.position;
            float Degree = Mathf.Rad2Deg * Mathf.Atan2(Enemy.y - Me.y, Enemy.x - Me.x);
            Collider2D[] collider2Ds1 = Physics2D.OverlapBoxAll(collider2.transform.position, new Vector2(0.1f, 2), Degree, layerMask);
            foreach (Collider2D collider in collider2Ds1)
            {
                DamagedTargetList.Add(collider.GetComponent<Enemy>().SpawnNum);
            }
        }
        DamagedTarget.SpawnNumbers = DamagedTargetList.ToArray();
        */
        //해당 정보를 제이슨으로 보냄
        #endregion
        #region 직접 대상즉사확률
        DamagedTargetList.Clear();
        TargetsNum InstanceKill;
        foreach (Collider2D collider2 in colliders)
        {
            int Percent = Random.Range(0, 100);
            if (Percent < Common_Static_Field.GetJobSynergeValue(ActorNum,Job.assassin))
            {
                Debug.Log("IstanceKill");
                DamagedTargetList.Add(collider2.GetComponent<Enemy>().SpawnNum);
            }
        }
        InstanceKill.SpawnNumbers = DamagedTargetList.ToArray();
        #endregion
        PlayerInfo_Master.master.MyInfo.TowerAtk(TowerNumber,
new string[] { JsonUtility.ToJson(targetsNum),  JsonUtility.ToJson(InstanceKill) });
    }
    [PunRPC]
    public void assasineAtkRpc(string[] Maintarget)
    {
        //  Debug.Log(Maintarget[0] + "  " + Maintarget[1]);
        TargetsNum DamagedTarget = JsonUtility.FromJson<TargetsNum>(Maintarget[0]);
        //Maintarget[1]은 데미지 데미지 입는 적들 0번인덱스는 타겟지정된적들
        //타겟대상에는 이미지라던가 추가효과가 필요하다면 메인타겟을 디코딩해서 사용
        //현제는 데미지만 입히기
        //차후에 크리티컬이라던가 입히는 목록이 추가로 필요하다면 크리티컬 입힐 대상만 AllTargetForDamge에 추가안하고 별도로 다시 targetsNum로 매개변수 추가
       // Debug.Log("공격요청받음" + DamagedTarget.SpawnNumbers.Length);
        EnemyHealth health;
        for (int i = 0; i < DamagedTarget.SpawnNumbers.Length; i++)
        {
            health = PlayerInfo_Master.master.FindEnemyHealthWithEnemyNum(DamagedTarget.SpawnNumbers[i]);
            if (health)
            {
                AtkParicle = ObjectPull.objectPull.DeQueue(EtcParticle.blood);
                AtkParicle.transform.position = health.transform.position;
                ObjectPull.objectPull.PlayNifParticleOverAndQue(EtcParticle.blood, AtkParicle);
                health.DamageIt(Damage);
                health.Synergy(Job, species,this);
                audio.PlayOneShot(audio.clip);
            }
        }
        DamagedTarget = JsonUtility.FromJson<TargetsNum>(Maintarget[1]);
       // Debug.Log("공격요청받음크리티컬" + DamagedTarget.SpawnNumbers.Length);
        for (int i = 0; i < DamagedTarget.SpawnNumbers.Length; i++)
        {
            health = PlayerInfo_Master.master.FindEnemyHealthWithEnemyNum(DamagedTarget.SpawnNumbers[i]);
            if (health)
            {
                ParticleSystem IsPrt = ObjectPull.objectPull.DeQueue(EtcParticle.IstanceKill);
                //ObjectPull.objectPull.InstantKill.Play();
                ObjectPull.objectPull.PlayNifParticleOverAndQue(EtcParticle.IstanceKill, IsPrt);
             //   Debug.Log(Damage * (int)Common_Static_Field.JobMergeBounus[(int)Job, Rank]);
                health.DamageIt(Damage *(int)Common_Static_Field.JobMergeBounus[(int)Job,Rank]);
                audio.PlayOneShot(Ingame_View_Controler.ingame_View_Controler.AssasineCriticalSound);
            }
            //health.Synergy(Job, species);
        }
        //1개만 이펙트 쓸때
       // targetsNum = JsonUtility.FromJson<TargetsNum>(Maintarget[0]);
       //// AtkParicle = ObjectPull.objectPull.DeQueue(Job);
       // Vector3 pos = Vector3.zero ;
       // for (int i = 0; i < targetsNum.SpawnNumbers.Length; i++)
       // {
       //     health = PlayerInfo_Master.master.FindEnemyHealthWithEnemyNum(targetsNum.SpawnNumbers[i]);
       //     if (health)
       //     {
       //         pos = health.transform.position;
       //         AtkParicle.transform.position = pos;
       //         AtkParicle.Play();
       //         break;
       //     }
       // }
       // AtkParicle.transform.position = pos;
       // StartCoroutine(Common_Static_Field.PlayNifParticleOverAndQue(Job, AtkParicle));
        //AtkParicle.Play();
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }

    public override void AtkRpc(string[] TargetInfo)
    {
        AtkClass.AttackRpc(this, TargetInfo);
    }

    protected override void ChampionAtk()
    {
        if (AtkClass == null)
            Debug.Log("AtkClass is null");
        else
            AtkClass.Attack(this);
    }

    public override void JobSynRpc(string[] TargetInfo)
    {
        if (Job == Job.Warrior)
        {
            //Debug.Log("전사 직업시너지");
            TargetsNum _targetsNum = JsonUtility.FromJson<TargetsNum>(TargetInfo[0]);
            if(PlayerInfo_Master.master.FindEnemyHealthWithEnemyNum(_targetsNum.SpawnNumbers[0]).gameObject)
            StartCoroutine(WarriorJobSyn(PlayerInfo_Master.master.FindEnemyHealthWithEnemyNum(_targetsNum.SpawnNumbers[0]).gameObject));
        }
    }
    IEnumerator WarriorJobSyn(GameObject target)
    {
       // Debug.Log("ActorNum" + ActorNum);
       // Debug.Log(Common_Static_Field.GetJobSynergeValue(ActorNum, Job.Warrior));
       // Debug.Log((int)Common_Static_Field.JobMergeBounus[(int)Job.Warrior, Rank]);
        int interval = Common_Static_Field.GetJobSynergeValue(ActorNum, Job.Warrior)+(int) Common_Static_Field.JobMergeBounus[(int)Job.Warrior,Rank];
        for (int i = 0; i < interval; i++)
        {
            GameObject go;
            go = ObjectPull.objectPull.DeQueue(Job.LastNumber);
            go.transform.position = transform.position;
            RangeChapBullet temp = go.GetComponent<RangeChapBullet>();
            if (!temp)
                temp = go.AddComponent<RangeChapBullet>();
            temp.owner = this;
            temp.SetAll(Damage, Job.LastNumber, species, Damage,target);
            yield return new WaitForSeconds(0.2f);
        }
    }
}


public abstract class Champion : MonoBehaviour
{
    protected PhotonView Pv;
    //public Job job;
    public Species species;
    [HideInInspector]
    public int Damage;
    //[HideInInspector]
    public float CoolTime;
    //[HideInInspector]
    [SerializeField]
    protected float CoolTimedelta = 0;
    public float Range;
    [HideInInspector]
    public int MultiTargetNum = 1;
    public GameObject target;
    public bool IsAttacking = false;
    public LayerMask layerMask;
    public bool IsMelee = false;
    public List<Collider2D> colliders = new List<Collider2D>();
    protected TargetsNum targetsNum = new TargetsNum();
    public int TowerNumber=0;
    public int Rank = 1;
    public int ActorNum = 0;
    protected AudioSource audio;
    Tile Ontile;
    //public abstract void Spi
    public abstract void JobSynRpc(string[] TargetInfo);
    public abstract void BeforeSet(int Actnum);
    public abstract void AtkRpc(string[] TargetInfo);
    //FopShop
    public Sprite ShopSprite;
    public int RequiredGold;
    Job job;
    public Job Job { get { return job; } }
    protected void SetJob(Job _job) { job = _job; }
    // public abstract void Atk();
    // public abstract void FindEnemy();
    public abstract void SetasRank(int ActNum,int _Rank = 1);
    protected virtual void Awake()
    {
        Pv = GetComponent<PhotonView>();
        audio = GetComponent<AudioSource>();
    }
    public virtual void  OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Range);
        //Gizmos.DrawSphere(transform.position, Range);
    }
    protected abstract void ChampionAtk();

    //전투시작과 대상이 범위를 나갔을때 다시 색적을 하고 전투를 이어 나가는 방법
    /*
     * 방법1
     * 1. ontrigger에서 전투 시작을 알리면서 타겟을 설정한다.
     * 2. 타겟이 사정거리를 벗어나면 Physics.Overlap을 이용해서 사정거리내 적을 찾아내고 생존시간이 제일 긴 적을 공격대상으로 지정한다.
     * 3. 적을 찾아 낼수 없다면 전투를 종료 한다.
     * 방법2
     * 1. 적들이 ontrigger로 들어 올때마다 List<Gameobject>로 대상을 등록한다.
     * 2. 적이 exittrigger로 나간다면 대상을 리스트에서 제거한다.
     * 3. 공격 대상이 사정거리를 벗어난다면 list안에서 널값이 아닌 가장 낮은 index의 적을 공격대상으로 지정한다.
     * 4. 공격중 공격 대상이 죽어 null로 변경되었다면 
     */
    public virtual void FindEnemy()
    {
        /*
         * 색적방법 1 ontrigger로 들어온 대상 리스트 관리
         * 색적방법 2 physics2D overlap- 탐색 대상의 transform 이 트리거 범위에 들어와야함 -> 탐색 대상의 collison 을 중심으로 탐색 하지 않음
         * 색적방법 3 Physics2D OverlapCollider 
         */
        for (int i = colliders.Count - 1; i >= 0; i--) if (colliders[i] == null) colliders.RemoveAt(i);
        if (colliders.Count >= MultiTargetNum)
        {
           // Debug.Log("colliders.Count : " + colliders.Count + "   MultiTargetNum : " + MultiTargetNum+ "   targetsNum.SpawnNumbers"+ targetsNum.SpawnNumbers.Length);
            return;
        }
       // Debug.Log("FindEnemy");
        Collider2D[] collider2Ds;
        // Collider2D collider2D = Physics2D.OverlapCircle(transform.position, Range, layerMask);
        //int num = Physics2D.OverlapCircleNonAlloc(transform.position, Range, collider2Ds, layerMask);
        // Collider2D collider2D = Physics2D.OverlapCircle(transform.position, Range);
        collider2Ds=Physics2D.OverlapCircleAll(transform.position, Range, layerMask);
      //  Debug.Log(collider2Ds.Length);
        if (collider2Ds.Length > 0)
        {
            IsAttacking = true;
            //foreach (Collider2D collider2D in collider2Ds)
            for (int i = 0; i < collider2Ds.Length; i++)
            {
                if (colliders.Contains(collider2Ds[i]))
                    continue;
                else if(!collider2Ds[i].GetComponent<EnemyHealth>().IsUntouchable)
                {
                    colliders.Add(collider2Ds[i]);
                    if (collider2Ds.Length >= MultiTargetNum)
                        break;
                }
            }
            targetsNum.SpawnNumbers = new int[colliders.Count];
            for (int i = 0; i < colliders.Count; i++)
            {
                targetsNum.SpawnNumbers[i] = colliders[i].GetComponent<Enemy>().SpawnNum;
            }
           // Debug.Log(targetsNum.SpawnNumbers.Length);
        }
        else
        {
            IsAttacking = false;
            target = null;
        }
        //if (collider2D != null)
        //{
        //    target = collider2D.gameObject;
        //    IsAttacking = true;
        //}
        //else
        //{
        //    IsAttacking = false;
        //    target = null;
        //}
    }
    protected bool CheckEnemyState()
    {
        //  Debug.Log("CheckEnemyState");
        //적 공격 가능한 상태인지 전체 판단  이미 죽은 몬스터 나 멀어진 몬스터 등,   적 무적상태등이 있다면 여기서 판단? 몬스터에서 판단?
        for (int i = colliders.Count - 1; i >= 0; i--)
        {

            if (colliders[i] == null || colliders[i].GetComponent<EnemyHealth>().hp <= 0 || (Vector2.Distance(transform.position, colliders[i].transform.position) > Range+1))
            {
                //Debug.Log((Vector2.Distance(transform.position, colliders[i].transform.position) > Range+1));
                colliders.RemoveAt(i);
            }
        }
        //Debug.Log("CheckEnemyState" + (colliders.Count > 0));
        if (colliders.Count > 0)
            return true;
        else
            return false;
        //if (target != null)
        //{
        //    if (target.GetComponent<EnemyHealth>().hp <= 0)
        //    {
        //        return false;
        //    }
        //    if (Vector2.Distance(transform.position, target.transform.position) > Range)
        //    {
        //        return false;
        //    }
        //}
        //else
        //{
        //    return false;
        //}

        {
            //Debug.Log("i: " + i + "colliders[i] : " + colliders[i]);
            //if (colliders[i] != null)
            //    Debug.Log(colliders[i].GetComponent<EnemyHealth>().hp);
            //else
            //    Debug.Log("null");
            //Debug.Log(Vector2.Distance(transform.position, colliders[i].transform.position) + "  " + Range);
        }
    }


    //private  void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (!IsAttacking)
    //    {
    //        Debug.Log(collision.name);
    //        target = collision.gameObject;
    //        IsAttacking = true;
    //    }

    //}
    public void UpdateThingWhenIsMine()
    {
        CoolTimedelta += Time.deltaTime;

        if (CoolTime < CoolTimedelta)
        {
            CheckEnemyState();
            FindEnemy();
           if (colliders.Count > 0)
           {
            CoolTimedelta = 0;
            ChampionAtk();
           }
        }
       // }
    }
    //public override void OnDisable()
    //{
    //    PlayerInfo_Master.master.MyInfo.DisableChampion(TowerNumber);
    //    SynergySystem.synergySystem.TowerNumChg(this, false, Rank);
    //}
    private void OnDisable()
    {
        //PlayerInfo_Master.master.MyInfo.DisableChampion(TowerNumber);
        if(ActorNum==PlayerInfo_Master.master.MyInfo.ActorNumber)
        SynergySystem.synergySystem.TowerNumChg(this, false, Rank);
        Ingame_View_Controler.ingame_View_Controler.TileOpen(true);
        Collider2D[] collider2D= Physics2D.OverlapPointAll(transform.position);
        foreach (Collider2D collider in collider2D)
        {
            if (collider.CompareTag("MyTile"))
            {
                collider.GetComponent<Tile>().IsBuildTower = false;
                break;
            }
        }
        Ingame_View_Controler.ingame_View_Controler.TileOpen(false);
        Debug.Log(collider2D);
    }
    private void OnMouseDown()
    {
        Debug.Log("마우스 위"+gameObject.name);
        Ingame_View_Controler.ingame_View_Controler.OnChampion(this);
    }
}