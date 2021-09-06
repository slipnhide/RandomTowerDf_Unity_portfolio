using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct Wave
{
    public GameObject WaveMonster;
    public GameObject LastMonster;
    public int MonsterNum;
    public float SpawnTime;
    public RuntimeAnimatorController anim;
    public int Hp;
    public float Speed;
}
 
public class WaveSystem :MonoBehaviour
{
    static WaveSystem Wavesystem;

    public static WaveSystem waveSystem
    {
        get 
        {
            #region getter
            if (Wavesystem == null)
            {
                WaveSystem temp = FindObjectOfType<WaveSystem>();
                if (temp != null)
                    Wavesystem = temp;
                else
                    Wavesystem = new GameObject().AddComponent<WaveSystem>();
            }
            return Wavesystem;
            #endregion
        }
    }
    [SerializeField]
    public List<Wave> waves; //웨이브 정보 저장
    int WaveCircle = 1;
    int nextWave = 0;
    //public int NextWave =0; //현제 웨이브 단계
    public int NextWave { get {/* Debug.LogError("NextWaveget");*/ return nextWave; } set { /*Debug.LogError("NextWaveset"+value); */nextWave = value; } }
    bool AllwaveOver = false;
    public RuntimeAnimatorController[] controller;
    public GameObject HpbarCanvas;
    //bool Waving = false;
    public Animator animator;
    private void Awake()
    {
        #region SingleToneWay2
        /*if (FindObjectsOfType<WaveSystem>().Length != 1)
        {
            Debug.LogError("waveSystem is not One: TryObjectNamee : " + gameObject.name);
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);*/
        #endregion
        #region SingleToneWay1
        if (Wavesystem == null)
        {
            Wavesystem = this;
        }
        else if (Wavesystem != this)
        {
            Debug.LogError("Wavesystem is not One: TryObjectNamee : " + gameObject.name);
            Destroy(gameObject);
            return;
        }
        animator = Resources.Load<Animator>("ResourcesEnemyMove.controller");
       // Dont
       // OnLoad(gameObject);
        #endregion
    }
    private void Update()
    {
        //시스템 완성전에는 테스트 버튼으로 나오게 설정
    }
    public void WaveStart()
    {
        //Debug.LogError("CheckWave---------------");
        //웨이브 의 몬스터가 셋팅되있는지, 모든 플레이어가 준비가 되어있는지 확인
       // if (CheckWave())
        {
            //Debug.LogError("CheckWave-----------------");
            EnemySpawner.enemySpawner.Spawn(MadeWave());
          //  EnemySpawner.enemySpawner.Spawn(waveSystem.waves[NextWave]);
            // Debug.LogError("Before"+ NextWave);
            NextWave++;
            //Debug.LogError("After"+NextWave);
        }
    }
    const int WaveNuber= 25;
    public readonly static int[] Numbers = new int[WaveNuber] 
    {10,10,10,10,1,10,10,10,10,1,10,10,10,10,1,10,10,10,10,1,10,10,10,10,1};
    public readonly static int[] Hps = new int[WaveNuber] 
    { 20, 30, 40, 50, 600, 70, 80, 90, 100,1100, 120,130,140,150,1600,170,180,190,200,2100,220,230,240,250,2600};
    public readonly static float[] Speeds = new float[WaveNuber] 
    {1,2,3,4,1,1,2,3,4,1,1,2,3,4,1,1,2,3,4,1,1,2,3,4,1};
    Wave MadeWave()
    {
        //Debug.Log("웨이브 생성: "+ Wavesystem.NextWave);
        if (NextWave >= WaveNuber)
        {
            NextWave = 0;
            WaveCircle++;
        }
        Wave NWave = new Wave();
        NWave.
            anim =
           Wavesystem.controller
            [NextWave];
        NWave.Speed = Speeds[NextWave] * (1 + (WaveCircle - 1)*0.5f);
        NWave.Hp = Hps[NextWave] * (1 + (WaveCircle - 1) * WaveNuber);
        NWave.SpawnTime = 1;
        NWave.MonsterNum = Numbers[NextWave];
       // Debug.Log("Curwave: " + NextWave);
       // Debug.Log(NWave.Hp + "  " + NWave.MonsterNum + "  " + NWave.SpawnTime + "  " + NWave.Speed);
        return NWave;
    }
    public bool CheckWave()
    {
       // Debug.Log(NextWave);
        //Debug.Log(waveSystem.waves.Count);
        if (waveSystem.NextWave >=
            waveSystem.waves.Count) 
        { AllwaveOver = true; Debug.Log("등록된 모든 웨이브 종료"); return false; }
        if (waveSystem.waves[NextWave].LastMonster == null && waveSystem.waves[NextWave].WaveMonster == null)
        {
            Debug.LogWarning("WaveMonster_Is_not_Setting  WaveNum:"+ waveSystem.NextWave);
            return false;
        }
        else
            return true;
    }
    //public void WaveEndForTest()
    //{
    //    for (int index = 0; index < enemySpawner.Count; index++)
    //        enemySpawner[index].DestroyAllMonster_ForTest();
    //}
}
