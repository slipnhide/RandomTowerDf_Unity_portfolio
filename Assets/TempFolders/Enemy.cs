using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
    public bool Big = false;
    public int WayNum = 0;
    public int MaxwayPoint = 0;
    public Vector2 Direction = Vector2.zero;
    public Vector3 NextPoint = Vector2.zero;
    public float Speed = 1;
    public Vector3[] Waypoints;
    public bool Test = false;
    public int OwnerNum;
    public int SpawnNum;
    bool ContectEnd;
    float SpeedChger = 1;
    bool IsStuned = false;
    // Animation anim;
    Animator anim;
    SpriteRenderer sr;
    Motion motion;
    EnemyHealth enemyHealth;
    public void Create(Vector3[] _WayPoints, int Hp, float _Speed, RuntimeAnimatorController Anim)
    {
        Waypoints = _WayPoints;
        transform.position = _WayPoints[0];
        WayNum = 1;
        
        MaxwayPoint = _WayPoints.Length;
        enemyHealth.MaxHp = Hp;
        enemyHealth.hp = Hp;
        Speed = _Speed;
        anim.runtimeAnimatorController = Anim;
        //GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        //GetComponent<BoxCollider2D>().isTrigger = true;
        //GetComponent<BoxCollider2D>().size = Vector2.one;
        Direction = ChgDir(WayNum);
    }
    public float speedchger{get{ return SpeedChger; } set { SpeedChger = value; } }

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        enemyHealth = GetComponent<EnemyHealth>();
        anim = GetComponent<Animator>();
        //anim = GetComponent<Animator>();
        // Debug.Log(anim.runtimeAnimatorController.name);
        //  anim.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Rat");
        // Debug.Log(anim.runtimeAnimatorController.name);
    }
    public void Set(int Hp,float Speed, RuntimeAnimatorController Anim)
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        // if (Test) return;
        if (!IsStuned)
        {
            transform.Translate(Direction * Speed * speedchger * Time.deltaTime);
            if (Vector2.Distance(transform.position, NextPoint) < 0.1f)
            {
                WayNum++;
                if (WayNum >= MaxwayPoint)
                {
                    ContectEnd = true;
                    TempDestroy();
                    return;
                }
                //Direction = ChgDir(WayNum);
                ChgDir(WayNum);
            }
        }
    }

    Vector2 ChgDir(int Waynum)
    {
        NextPoint= Waypoints[Waynum];
        Vector2 value = (Waypoints[Waynum] - transform.position).normalized;
       // Debug.Log(value);
        anim.SetFloat("x", value.x);
        if (value.x < 0)
            sr.flipX = true;
        else
            sr.flipX = false;
        anim.SetFloat("y", value.y);
        Direction = value;
        return value;
    }
    Vector2 ChgDir(int Waynum, Vector3 Pos)
    {
        NextPoint = Waypoints[Waynum];
        Vector2 value = (Waypoints[Waynum] - Pos).normalized;
        Debug.Log(value);
        anim.SetFloat("x", value.x);
        if (value.x < 0)
            sr.flipX = true;
        else
            sr.flipX = false;
        anim.SetFloat("y", value.y);
        return value;
    }
    public void MoveBack(float Num)
    {
        if (Num <= 0) return;
        if (Big) Num = Num * 0.25f;
        ParticleSystem particle = ObjectPull.objectPull.DeQueue(EtcParticle.Tornado);
        particle.gameObject.transform.position = transform.position;
        ObjectPull.objectPull.PlayNifParticleOverAndQue(EtcParticle.Tornado, particle);
        float SumDis = 0;
        int TempWaynum;
        //바로 이전 포인트 까지 갈필요 없다면 해당위치까지 이동하고 종료
        if (Vector3.Distance(transform.position, Waypoints[WayNum - 1]) > Num)
        {
            Debug.Log(transform.position +"  "+ (Waypoints[WayNum - 1] - transform.position).normalized + "  "+Num);
            transform.position += (Waypoints[WayNum - 1] - transform.position).normalized * Num;
            ChgDir(WayNum);
            return;
        }
        else
        {
            //이전 포인트보다 추가로 가야 한다면 검색
            TempWaynum = WayNum - 1;
            SumDis += Vector3.Distance(transform.position, Waypoints[TempWaynum]);
            float Dis = 0;
            while (SumDis < Num)
            {
                //검색 도중 마지막 포인트까지 왔다면 종료
                if (TempWaynum == 0)
                {
                    WayNum = 1;
                    transform.position = Waypoints[0];
                    ChgDir(1);
                    return;
                }

                Dis = Vector2.Distance(Waypoints[TempWaynum], Waypoints[TempWaynum - 1]);
                Debug.Log(SumDis + "  :" + Dis);
                if (SumDis + Dis > Num)
                    break;
                else
                {
                    SumDis += Dis;
                    TempWaynum--;
                }
            }
            //마지막 바라보는곳 설정해준뒤
            WayNum = TempWaynum;
            //위치 초기화 위치는 마지막 바라보는 곳으로 부터 마저 못채운만큼 뒤로 이동
            Debug.Log(WayNum);
            Debug.Log(Num+" "+"TempWaynum" + TempWaynum + " (Num - SumDis) :  " + (Num - SumDis) + " " + (Waypoints[TempWaynum - 1] - Waypoints[TempWaynum]).normalized);
            transform.position = Waypoints[TempWaynum] + (Waypoints[TempWaynum - 1] - Waypoints[TempWaynum]).normalized * (Num - SumDis);
            ChgDir(WayNum);

        }
      //  Direction = ChgDir(WayNum);
    }
    public void ReadyDestroy()
    {
        //파괴 전 준비
    }
    public void TempDestroy()
    {
        //일단은 단순 파괴 이후 매니저에 전달해서 파괴 하게 변경
        gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        if (!Test)
        {
            PlayerInfo_Master.master.EnemyDie(this,OwnerNum,ContectEnd);
            ObjectPull.objectPull.EnQueue(gameObject, Pull.BaseEnemy);
        }
    }
    public void Stun(float time)
    {
        IsStuned = true;
        Invoke("DeStun", time);
    }
    void DeStun() { IsStuned = false; }
}
