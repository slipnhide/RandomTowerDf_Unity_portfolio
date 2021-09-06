using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
[System.Serializable]
public class PlayerInfo_Photon : MonoBehaviourPunCallbacks, IPunObservable
{
    ParticleSystem AngelParticle = null;
    ParticleSystem RainArrow;
    ParticleSystem StunParticle;
    int life = Common_Static_Field.StartLife;
    public int Life { 
        set { Pv.RPC(nameof(LifeChgRpc), RpcTarget.All, value); } 
        get { return life; } }
    [PunRPC]
    void LifeChgRpc(int value)
    {
        life = value;
        InfoUpdating.LifeUpdate(value);
    }

    [SerializeField]
    //프로퍼티 및 Rpc호출
    #region Gold 프로퍼티
    int gold=0;
    public int Gold { set { Pv.RPC("ChgGoldRpc", RpcTarget.All, value); if (Pv.IsMine) Ingame_View_Controler.ingame_View_Controler.GoldUpdate(); } get { return gold; } }
    [PunRPC]
    void ChgGoldRpc(int _gold)
    {
        gold = _gold;
        Debug.Log("골드RPC +" + ActorNumber+"  value : "+ _gold);
        if(InfoUpdating!=null)
        InfoUpdating.GoldUpdate(gold);
    }
    public bool TryGoldChg(int _gold) 
    {
        if (gold+_gold >= 0)
        {
            Gold += _gold;
            return true;
        }
        else  return false;
    }
    #endregion
    int Lv = 1;
    public int lv { set { Pv.RPC("Chglv", RpcTarget.All, value); } get { return Lv; } }
    [PunRPC]
    void Chglv(int lv)
    {
        Lv = lv;
    }
    int RequiredXp;
    int CurXp=0;
    [HideInInspector]
    public float XpPercentate { get { return (float)CurXp / RequiredXp; } }
    #region 시너지 정보 프로퍼티,RPc
    public Dictionary<Job, int> CurJobSynergyRank = new Dictionary<Job, int>();
    public Dictionary<Species, int> CurSpeciesSynergyRank = new Dictionary<Species, int>();
    public Dictionary<Job, int> CurJobNum = new Dictionary<Job, int>();
    public Dictionary<Species, int> CurSpeciesNum = new Dictionary<Species, int>();
    [PunRPC]
    public void ChgsSynergy(string _synergyUpdate3)
    {
        Debug.Log(Pv.Owner.NickName);
        SynergyUpdate3 synergyUpdate3 = JsonUtility.FromJson<SynergyUpdate3>(_synergyUpdate3);
        CurJobNum = PlayerInfo_Master.JobSynergyDecord2(synergyUpdate3.CurJobNum);
        CurJobSynergyRank= PlayerInfo_Master.JobSynergyDecord2(synergyUpdate3.CurJobSynergyRank);
        CurSpeciesNum = PlayerInfo_Master.SpiecesSynergyDecord2(synergyUpdate3.CurSpeciesNum);
        CurSpeciesSynergyRank = PlayerInfo_Master.SpiecesSynergyDecord2(synergyUpdate3.CurSpeciesSynergyRank);
    }
    #endregion
    [ContextMenu("시너지 확인")]
    #region 시너지확인 테스트
    public void CheckSy()
    {
        for (int i = 0; i < (int)Job.LastNumber; i++)
        {
            Debug.Log((Job)i+" : NuB : " + CurJobNum[(Job)i]+ "   "+" : Rank : " + CurJobSynergyRank[(Job)i]);
        }
        for (int i = 0; i < (int)Species.LastNumber; i++)
        {
            Debug.Log((Species)i + " : NuB : " + CurSpeciesNum[(Species)i]+(Species)i + " : Rank : " + CurSpeciesSynergyRank[(Species)i]);
        }
    }
    #endregion

    #region 뉴 시너지정도
    int[] CurJobRank =new int[(int)Job.LastNumber];
    int[] CurSpeciesRank = new int[(int)Species.LastNumber];
    public int CurRank(Job Job) { return CurJobRank[(int)Job];}
    public int CurRank(Species species) { return CurSpeciesRank[(int)species]; }
    public int[][] CurRanks() { int[][] value = new int[2][]; value[0]= CurJobRank; value[1] = CurSpeciesRank; return value; }
    public void ChgRank(int jobRank = -1, int JobNum=-1 ,int speciesRank=-1,int speciesNum = -1) { Pv.RPC(nameof(ChgRankRpc), RpcTarget.All, jobRank, JobNum, speciesRank, speciesNum);  }
    [PunRPC]
    void ChgRankRpc(int jobRank = -1, int JobNum = -1, int speciesRank = -1, int speciesNum = -1) 
    {
        //Debug.Log("ChgRankRpc" + jobRank + "  " + JobNum + "  " + speciesRank + "  " + speciesNum);
        if (jobRank != -1) 
            CurJobRank[JobNum] = jobRank;
        if (speciesRank != -1)
            CurSpeciesRank[speciesNum] = speciesRank;
     //   Debug.Log("ChgRankRpc" + ActorNumber+" Owner:"+Pv.Owner+ "  LookActorNum :"+ LookActorNum);
        if (master.MyInfo.LookActorNum == ActorNumber)
            InfoUpdating.MedalUpdate(CurJobRank, CurSpeciesRank);
    }
    [ContextMenu("new Check")]
    public void CheckSyn()
    {
        string D = "";
        for (int i = 0; i < (int)Job.LastNumber; i++)
        {
            D += ((Job)i).ToString();
            D += (" : " + CurJobRank[i]);
        }
        Debug.Log(D);
        D = "";
        for (int i = 0; i < (int)Species.LastNumber; i++)
        {
            D += ((Species)i).ToString();
            D += (" : " + CurSpeciesRank[i]);
        }
        Debug.Log(D);
    }
    #endregion

    bool Angel = false;
    #region ready프로퍼티 -> 마스터
    public bool isready = false;
    public bool IsReady { set { /*Pv.RPC("ReadyToPlayer", RpcTarget.All, ActorNumber,value);*/ Pv.RPC("IsReadyRpc", RpcTarget.All, value); InfoUpdating.ReadyUpdating(value); } get { return isready; } }
    [PunRPC]
    void IsReadyRpc(bool value) { isready = value; }//    master.PlayerReady();}
    //void ReadyToPlayer(bool value) { master.Pv.RPC(nameof(master.PlayerReady), RpcTarget.All, ActorNumber, value); }
    #endregion
    [SerializeField]
    int OwnEnemyNum = 0;
    public int ownEnemy { get { return OwnEnemyNum; } set { if(Pv.IsMine) Pv.RPC(nameof(ChgOwnEnemyRpc), RpcTarget.All, value);  } }
   // public List<Enemy> ownEnemys = new List<Enemy>();
    //public Dictionary<int, EnemyHealth> ownEnemys = new Dictionary<int, EnemyHealth>();
    [PunRPC]
    void ChgOwnEnemyRpc(int i)
    {
        OwnEnemyNum = i;
        if (OwnEnemyNum == 0)
            master.CheckAllEndMonster();
    }



    public PhotonView Pv;
    public Ingame_Update_PlayerInfo InfoUpdating;
    public Vector3 Pos;
    public Vector3[] WayPt;
    PlayerInfo_Master master;
    public int ActorNumber;
    public int LookActorNum;
    int TowerNum = 0;
   // List<Champion> MyCham = new List<Champion>();
    Dictionary<int, Champion> MyChampions = new Dictionary<int, Champion>();
    public void Create(GameObject _tower, Vector3 Pos, int _Rank = 1)
    {
        Pv.RPC("CreateRpc", RpcTarget.All, _tower.name, Pos, _Rank);
    }
    public Dictionary<int, Champion> GetMyChampions() { return MyChampions; }
    [PunRPC]
    void CreateRpc(string TowerName, Vector3 Pos, int _Rank=1)
    {
       // Debug.Log("CreateRpc");
        Champion temp = Instantiate(Ingame_View_Model.ingame_View_Model.FindTower(TowerName),
            Pos, Quaternion.identity).GetComponent<Champion>();
        temp.Rank = _Rank;
        temp.SetasRank(Pv.Owner.ActorNumber,_Rank);
        int num = ActorNumber * 10000 + TowerNum;
        if (_Rank == 2)
            Instantiate(Ingame_View_Controler.ingame_View_Controler.RankParticle[0], temp.transform);
        else if (_Rank == 3)
            Instantiate(Ingame_View_Controler.ingame_View_Controler.RankParticle[1], temp.transform);
        temp.TowerNumber = num;
        TowerNum++;
        MyChampions.Add(num, temp);
    }
    public void DisableChampion(string Numbers,bool IsMerged,Vector2 UiPos)
    {
        Pv.RPC(nameof(DisableChampionRpc), RpcTarget.All, Numbers, IsMerged, UiPos);
    }
    [PunRPC]
    void DisableChampionRpc(string Numbers,bool IsMerged,Vector2 UiPos)
    {
        ToSimpleJson<int[]> toSimpleJson = JsonUtility.FromJson<ToSimpleJson<int[]>>(Numbers);
        for (int i = 0; i < toSimpleJson.value.Length; i++)
        {
            if (IsMerged)
                MergedEffect(MyChampions[toSimpleJson.value[i]].transform.position, UiPos);
            else
            {
                ChampSellEffect(MyChampions[toSimpleJson.value[i]].transform.position);
                int ChampGold = Common_Static_Field.ChapmGold[(int)MyChampions[toSimpleJson.value[i]].Job, (int)MyChampions[toSimpleJson.value[i]].species];
                gold += ChampGold * MyChampions[toSimpleJson.value[i]].Rank;
                if (InfoUpdating != null)
                    InfoUpdating.GoldUpdate(gold);
                Ingame_View_Controler.ingame_View_Controler.GoldUpdate();
            }
            Destroy(MyChampions[toSimpleJson.value[i]].gameObject);
            MyChampions.Remove(toSimpleJson.value[i]);
        }
    }
    void ChampSellEffect(Vector3 Pos) 
    {  
        ParticleSystem particleSystem = ObjectPull.objectPull.DeQueue(EtcParticle.Gold);
        particleSystem.transform.position = Pos;
        Debug.Log(particleSystem);
        Debug.Log(Pos);
        Vector2 ToPos = Ingame_View_Controler.ingame_View_Controler.Gold.transform.position;
        Debug.Log(ToPos);
        if (Pv.IsMine)
        {
            particleSystem.GetComponent<WaitAndGoParticle>().Set(2, ToPos);
            Debug.Log("Mine");
        }
        else
        {
            Debug.Log(Camera.main.WorldToScreenPoint(Pos));
            particleSystem.GetComponent<WaitAndGoParticle>().Set(2, Camera.main.WorldToScreenPoint(Pos));
        }
        ObjectPull.objectPull.PlayNifParticleOverAndQue(EtcParticle.Gold, particleSystem);
    }
    void MergedEffect(Vector3 Pos,Vector2 ToPos) 
    {
       ParticleSystem particleSystem= ObjectPull.objectPull.DeQueue(EtcParticle.TowerMerge);
        particleSystem.transform.position = Pos;
        ObjectPull.objectPull.PlayNifParticleOverAndQue(EtcParticle.TowerMerge, particleSystem);
        particleSystem.GetComponent<ParticleTest>().Set(ToPos);
    }
    public void TowerAtk(int TowerNumber, string[] TargetInfo)
    {
       // Debug.Log(TowerNumber);
        Pv.RPC("TowerAtkRpc", RpcTarget.All, TowerNumber, TargetInfo);
    }
    [PunRPC]
    void TowerAtkRpc(int TowerNumber, string[] TargetInfo)
    {
        MyChampions[TowerNumber].AtkRpc(TargetInfo);
    }
    public void JobSynAtk(int TowerNumber, string[] TargetInfo)
    {
        // Debug.Log(TowerNumber);
        //Debug.Log("전사 시너지 발도요청전달");
        Pv.RPC("JobSynAtkRpc", RpcTarget.All, TowerNumber, TargetInfo);
    }
    [PunRPC]
    void JobSynAtkRpc(int TowerNumber, string[] TargetInfo)
    {
        //Debug.Log("전사 시너지 발동수신");
        MyChampions[TowerNumber].JobSynRpc(TargetInfo);
    }

    [ContextMenu("Curgold")]
    void curgold()
    {
        Debug.Log("Gold _ " + Gold + "   gold : " + gold);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
       // Debug.Log("메세지"+gameObject.name);
        if (stream.IsWriting)
        {
          //  stream.SendNext(player.Gold);
           // Debug.Log(CurJobSynergyRank[Job.assassin]);
        }
        else
        {
          //  Gold = (int)stream.ReceiveNext();
           // Debug.Log(CurJobSynergyRank[Job.assassin]);
        }
    }

    private void Awake()
    {
        XpSet();
        if (Pv == null) Pv = GetComponent<PhotonView>();
        master = PlayerInfo_Master.master;
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 60;
        if (Pv.IsMine)
        {
            EnemySpawner.enemySpawner.Myinfo = this;
            ActorNumber = (int)PhotonNetwork.LocalPlayer.ActorNumber;
            LookActorNum = ActorNumber;
          //  Instantiate(new GameObject(ActorNumber.ToString()));
            gameObject.name = PhotonNetwork.LocalPlayer.NickName;
            Pos = new Vector3(ActorNumber * 60, 0, 0);
            Camera.main.transform.position = Pos + new Vector3(0, 0, -10);
            Transform tiles = PhotonNetwork.Instantiate("Map", Pos, Quaternion.identity).transform.GetChild(0);
            //  Transform[] temp = tiles.GetComponentInParent<WayPoints>().wayPoints;
            Transform[] temp = new Transform[tiles.parent.GetChild(1).childCount];
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = tiles.parent.GetChild(1).GetChild(i);
            }
           float[] Pt_x = new float[temp.Length];
            float[] Pt_y = new float[temp.Length];
            for (int i = 0; i < temp.Length; i++)
            {
                Pt_x[i] = temp[i].transform.position.x;
                Pt_y[i] = temp[i].transform.position.y;
               // Debug.Log(WayPt[i].transform.position.x + "   "+ WayPt[i].transform.position.y);
            }
            floatArray x;
            x.array = Pt_x;
            floatArray y;
            y.array = Pt_y;
            Debug.Log(JsonUtility.ToJson(x) +"  "+JsonUtility.ToJson(y));
            Ingame_View_Controler.ingame_View_Controler.Myinfo = this;
            Ingame_View_Model.ingame_View_Model.playerInfo = this;
            
            Pv.RPC("Set", RpcTarget.All, JsonUtility.ToJson(x), JsonUtility.ToJson(y));
            //  EnemySpawner enemySpawner = GameObject.Find("PlayerManager").GetComponent<EnemySpawner>();
            // enemySpawner.WayPoints = WayPt;
            //Ingame_View_Controler.ingame_View_Controler.Tiles = new SpriteRenderer[tiles.childCount];
            Ingame_View_Controler.ingame_View_Controler.Tiles = tiles.GetComponentsInChildren<SpriteRenderer>();
            Ingame_View_Controler.ingame_View_Controler.TielsColliders = tiles.GetComponentsInChildren<BoxCollider2D>();
            for (int index = 0; index < tiles.childCount; index++)
            {
                //Ingame_View_Controler.ingame_View_Controler.Tiles[index] = tiles.GetChild(index);
                tiles.GetChild(index).tag = "MyTile";
            }
            Ingame_View_Controler.ingame_View_Controler.TileOpen(false);
           PhotonNetwork.Instantiate("UiPreFab/InGame/Ingame_PlayerInfo", Pos, Quaternion.identity);
            Common_Static_Field.MyInfo = this;

            Ingame_View_Model.ingame_View_Model.ChampSet();
            //Gold += 10;
            //Debug.Log("골드 요청"+ActorNumber);
        }
        else
        {
            gameObject.name = Pv.Owner.NickName;
            ActorNumber = (int)Pv.Owner.ActorNumber;
        }
        RainArrow = ObjectPull.objectPull.RainArrow;
        StunParticle = ObjectPull.objectPull.Stun;
    }
    float DevilCoolDown = Common_Static_Field.DevilCoolTime;
    float DevilDelta = 0;
    public float CoolTimePerDevil { get {return DevilDelta / DevilCoolDown; } }
    private void Update()
    {
        if (Pv.IsMine)
        {
            foreach (KeyValuePair<int, Champion> champion in MyChampions)
            {
               
                champion.Value.UpdateThingWhenIsMine();
            }
            if(DevilDelta < DevilCoolDown)
                DevilDelta += Time.deltaTime;
        }
    }
    [PunRPC]
    void Set(string _tr_x, string _tr_y)
    {
        master.join(ActorNumber, this);
        floatArray x = JsonUtility.FromJson<floatArray>(_tr_x);
        floatArray y = JsonUtility.FromJson<floatArray>(_tr_y);
        Pos = new Vector3(ActorNumber * 60, 0, 0);
      //  Debug.Log(x.array.Length);
        WayPt = new Vector3[x.array.Length];
//Debug.Log(WayPt.Length);
        int index = x.array.Length;
       // Debug.Log(index);
       // Debug.Log(_tr_x+"  "+ _tr_y);
       // Debug.Log(x.array[0])   ;
        for (int i = 0; i < index; i++)
        {
           // Debug.Log("i: " + i + "  x.array[i] : " + x.array[i] + "   y.array[i]: " + y.array[i]);
            Vector3 pos = new Vector3(x.array[i], y.array[i], 0);
            WayPt[i] = pos;
        }
    }
    //[PunRPC]
    //void MakeInfoPanel(Vector3 instancePos, string name,int ActorNumber)
    //{
    //    Ingame_CameraChanger ingame_CameraChanger = GetComponent<Ingame_CameraChanger>();
    //    ingame_CameraChanger.Set(PhotonNetwork.LocalPlayer.NickName, this, Camera.main.transform.position);
    //    ingame_CameraChanger.transform.parent = Ingame_View_Controler.ingame_View_Controler.eachPlayerInfoPanel;
    //    ingame_CameraChanger.transform.SetAsFirstSibling();
    //    InfoUpdating = ingame_CameraChanger;
    //}
    private void Start()
    {
        if (Pv.IsMine)
        {
          //  FindObjectOfType<Player_Info>().photon = this;
            SynergySystem.synergySystem.photon = this;
        }
    }

    [ContextMenu("testc")]
    void test()
    {
        //for (int i = 0; i < (int)Job.LastNumber; i++)
        //{
        //    Debug.Log((Job)i + "  " + CurJobNum[(Job)i]);
        //}
        for (int i = 0; i < (int)Species.LastNumber; i++)
        {
            if(CurSpeciesSynergyRank.ContainsKey((Species)i))
            Debug.Log((Species)i + "  " + CurSpeciesSynergyRank[(Species)i]);
        }
    }

    void XpSet()
    {
        RequiredXp = (lv) * 1;
        CurXp = 0;
      //  Xpimage.transform.GetChild(0).GetComponent<Text>().text = "CurLv: " + PlayerLv.ToString();
    }
    public void AddXp()
    {
        CurXp++;
        if (CurXp >= RequiredXp)
        {
            lv++;
            XpSet();
        }

       // Xpimage.fillAmount = (float)CurXp / RequiredXp;
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        int ActNum = otherPlayer.ActorNumber;
        Champion[] champions = FindObjectsOfType<Champion>();
        Debug.Log(ActNum);
        for (int i = 0; i < champions.Length; i++)
        {
            Debug.Log(champions[i].TowerNumber / 10000);
            if (champions[i].TowerNumber / 10000 == ActNum)
                Destroy(champions[i].gameObject);
        }
        Enemy[] Enemys = FindObjectsOfType<Enemy>();
        //Debug.Log(ActNum);
        for (int i = 0; i < Enemys.Length; i++)
        {
            if (Enemys[i].OwnerNum == ActNum)
                Enemys[i].GetComponent<EnemyHealth>().hp = 0;
        }
        for (int i = 0; i < master.playerInfo_s.Count; i++)
        {
            if (master.playerInfo_s[i].name.Equals(otherPlayer.NickName))
            {
                master.playerInfo_s.RemoveAt(i);
                break;
            }
        }
        master.infos.Remove(otherPlayer.ActorNumber);
       // master.CheckAllEndMonster();
    }
    public void LifeDown()
    {
        if (Pv.IsMine)
        {
            Life -= 1;
            if (Life == 0)
                Ingame_View_Controler.ingame_View_Controler.LifeOver();
        }
    }
    public void WaveEndGold()
    {
        Debug.Log("WaveEndGold");
        int ChgedGold= Common_Static_Field.WaveEndGold;
        ChgedGold += Common_Static_Field.GetJobSynergeValue(ActorNumber, Job.Novel);
        Debug.Log(ChgedGold);
        Gold += ChgedGold;
    }

    public void Stun() {  if(CurSpeciesRank[(int)Species.Orc]>0) Pv.RPC(nameof(StunRpc), RpcTarget.All); }
    [PunRPC]
    public void StunRpc() 
    {
        master.Stun(ActorNumber); 
    }

    public void AngelFall()
    {
        StartCoroutine(AngelFallMouse(10));
    }
    public LayerMask layerMask;
    IEnumerator AngelFallMouse(float time)
    {
        Debug.Log("AngelFallMouse");
        Vector3 _position = Vector3.zero;
        TargetsNum targetsNum = new TargetsNum();
        float CoolTime = 1;
        float CoolTimedelta = 0;
        Collider2D[] collider2Ds;
        while (time > 0)
        {
            Debug.Log(CoolTimedelta);
            if (CoolTimedelta>= CoolTime)
           // if (Input.GetMouseButtonDown(0))
            {
                    Debug.Log("GetMouseButtonDown");
                    CoolTimedelta = 0;
                if (Camera.main.orthographic)
                {
                    Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    _position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
                else
                {

                    _position = Input.mousePosition;
                    Debug.Log(_position);
                    // _position.z = Camera.main.farClipPlane;
                    //_position.z = Camera.main.nearClipPlane;
                    _position.z = 10;
                    _position = Camera.main.ScreenToWorldPoint(_position);
                    Debug.Log(_position);
                }

              collider2Ds= Physics2D.OverlapCircleAll(_position, 1, layerMask);
                targetsNum.SpawnNumbers = new int[collider2Ds.Length];
                for (int i = 0; i < collider2Ds.Length; i++)
                {
                    targetsNum.SpawnNumbers[i]= collider2Ds[i].GetComponent<Enemy>().SpawnNum;
                }
                Debug.Log(_position);
                Pv.RPC(nameof(CallAngelFallRpc), RpcTarget.All, _position, JsonUtility.ToJson(targetsNum));
            }

            CoolTimedelta += Time.deltaTime;
            time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
    [PunRPC]
    public void CallAngelFallRpc(Vector3 _position, string Damaged)
    {
        if (AngelParticle == null)
            AngelParticle = ObjectPull.objectPull.AngelParticle;
        //int dmg = (int)Common_Static_Field.GetSpSynergeValue(ActorNumber, Species.Angel);
        int dmg = (int)(EnemySpawner.enemySpawner.CurEnemyHp * Common_Static_Field.GetSpSynergeValue(ActorNumber, Species.Angel));
        if (dmg == 0) dmg = 1;
        AngelParticle.transform.position = _position;
        AngelParticle.Play();
        TargetsNum targetsNum = JsonUtility.FromJson<TargetsNum>(Damaged);
        Debug.Log("targetsNum : "+targetsNum.SpawnNumbers.Length);
        EnemyHealth health;
        int Length = targetsNum.SpawnNumbers.Length;
        for (int i = Length - 1; i >= 0; i--)
        {
            health = master.FindEnemyHealthWithEnemyNum(targetsNum.SpawnNumbers[i]);
            if (health) health.DamageIt(dmg);
        }
        //for (int i = 0; i < Length; i++)
        //{
        //    health = master.FindEnemyHealthWithEnemyNum(targetsNum.SpawnNumbers[i]);
        //    if(health) health.DamageIt(dmg);
        //}
    }
    /*
    public void CallBackAngelFallRpc()
    { }
    IEnumerator AngelFallStart(float time)
    {
        Vector3 _position = Vector3.zero;
        ParticleSystem particle;
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Camera.main.orthographic)
                {
                    Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    _position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
                else
                {

                    _position = Input.mousePosition;
                    Debug.Log(_position);
                    // _position.z = Camera.main.farClipPlane;
                    //_position.z = Camera.main.nearClipPlane;
                    _position.z = 10;
                    _position = Camera.main.ScreenToWorldPoint(_position);
                    Debug.Log(_position);
                }
                particle.transform.position = _position;
                particle.Play();
            }
        }
    }
    */

    public bool DevilDo(int ActNum)
    {
        if (master.DevilThing(ActNum))
        {
            //후속처리
            DevilDelta = 0;
            return true;
        }
        else
            return false;
        
    }
    public void UseRainArrow(){ Pv.RPC(nameof(UseRainArrowRpc), RpcTarget.All); }
    [PunRPC]
    void UseRainArrowRpc(){StartCoroutine(RainTime());}
    IEnumerator RainTime()
    {
        for (int i = 0; i < 3; i++)
        {
            RainArrow.gameObject.transform.position = new Vector3(Pos.x, RainArrow.gameObject.transform.position.y, RainArrow.gameObject.transform.position.z);
            master.RainArrow(ActorNumber);
            RainArrow.Play();
            yield return new WaitForSeconds(0.5f);
        }
    }
}
