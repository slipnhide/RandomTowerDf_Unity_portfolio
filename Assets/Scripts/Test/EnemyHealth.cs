using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public int MaxHp = 100;
    int Hp = 100;
    public int hp { get { return Hp; } set { Hp = value; Hpbar.fillAmount = (float)Hp / MaxHp; } }
    public Image Hpbar;
    float UntouchTime = 0;
    Enemy enemy;
    public bool IsUntouchable { get { return UntouchTime > 0; } }
    //bool MerMaidSlow = false;
    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }
    private void Update()
    {
        if (UntouchTime > 0)
            UntouchTime -= Time.deltaTime;
    }
    [ContextMenu("damage")]
    public void DamageIt(int Dmg)
    {
       // int i = 10;
        hp -= Dmg;
        if (hp <= 0)
        {
            StopAllCoroutines();
            gameObject.SetActive(false);
        }
    }

    public void Synergy(Job job, Species spiecies,Champion champion)
    {
        bool JobSel = false;
        switch (job)
        {
            case Job.Mage:
                {
                    if(champion!=null)
                    JobSynergy_Mage(Common_Static_Field.GetJobSynergeValue(champion.ActorNum,Job.Mage));
                    JobSel = true;
                    break;
                }
            case Job.scout:
                {
                    JobSynergy_Ranger();
                    JobSel = true;
                    break;
                }
            case Job.LastNumber:
                {
                    JobSel = true;
                    break;
                }
        }
       // if (JobSel == false) Debug.LogWarning("잘못된 Job시너지" + job);
        switch (spiecies)
        {
            case Species.MerMaid:
                SpieciesSynergy_Mermaid(champion);
                break;
            case Species.Orc:
                SpieciesSynergy_Orc();
                break;
            case Species.Undead:
                SpieciesSynergy_Undead(champion);
                break;
            case Species.LastNumber:
                break;

        }
    }
    void JobSynergy_Mage(int Rank)
    {
       // particle.Play();
        enemy.MoveBack(Rank);
    }

    void JobSynergy_Ranger()
    {
      //  Debug.Log("_Ranger synergy");
    }
    void SpieciesSynergy_Mermaid(Champion champ)
    {
        if (champ == null) return;
        if (Common_Static_Field.GetSpSynergeValue(champ.ActorNum, Species.MerMaid) <= 0) return;
        float Slowrate = Common_Static_Field.GetSpSynergeValue(champ.ActorNum, Species.MerMaid);
        //Debug.Log("슬로우");
        if (enemy.Big)
            Slowrate *= 0.25f;
        enemy.speedchger *= (1 - Slowrate);
        if(gameObject.activeInHierarchy==false)
        StartCoroutine(SlowBack(Slowrate, Common_Static_Field.SpeciesMergeBounus[(int)Species.MerMaid,champ.Rank]));

    }
    IEnumerator SlowBack(float Slowrate,float Slowtime)
    {
        float Times = Slowtime;
        ParticleSystem particle;
        //Debug.Log(Slowtime);
        while (Times > 0)
        {
            Times -= 1;
            particle = ObjectPull.objectPull.DeQueue(EtcParticle.Slow);
           // Debug.LogWarning(particle);
            particle.transform.position = transform.position;
            ObjectPull.objectPull.PlayNifParticleOverAndQue(EtcParticle.Slow, particle);
           
           // particle.Play();
            yield return new WaitForSeconds(1);
        }
       // yield return new WaitForSeconds(Times);
        enemy.speedchger /= (1-Slowrate);
       // MerMaidSlow = false;
    }
    void SpieciesSynergy_Orc()
    {
       // Debug.Log("Orc synergy");
    }
    void SpieciesSynergy_Undead(Champion champ)
    {
        if (champ == null||!gameObject.activeInHierarchy) return;
        if (PlayerInfo_Master.master.infos[champ.ActorNum].CurRank(Species.Undead) < 1)
            return;
        int damage= champ.Damage;
        StartCoroutine(
            Poison(
            damage, 
            Common_Static_Field.GetSpSynergeValue(champ.ActorNum,Species.Undead), 
            Common_Static_Field.SpeciesMergeBounus[(int)Species.Undead, champ.Rank])
            );
    }
    IEnumerator Poison(int Damage,float inteval,float duration)
    {
        float Times= duration;
        float Interval = inteval;
        int Dmg = Damage;
        ParticleSystem particleSystem;
        while (Times>0)
        {
            DamageIt(Dmg);
            if (gameObject.activeInHierarchy)
            {
                particleSystem = ObjectPull.objectPull.DeQueue(EtcParticle.Poison);
                particleSystem.transform.position = transform.position;
                ObjectPull.objectPull.PlayNifParticleOverAndQue(EtcParticle.Poison, particleSystem);
                Times -= Interval;
            }
            yield return new WaitForSeconds(Interval);
        }
    }
    
    
}
