using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum Pull { Bullet, Tarnado,Hpbar,BaseEnemy}
[System.Serializable]
public enum EtcParticle {TowerMerge,Slow,Tornado,Poison,blood,Gold ,IstanceKill}
[System.Serializable]
public struct PullsObject { public Pull Pullenum; public GameObject Prefab; }
[System.Serializable]
public struct AtkPariclePullsObject { public Job Pullenum; public GameObject Prefab; }
[System.Serializable]
public struct EtcParticles { public EtcParticle Pullenum; public ParticleSystem Prefab; }

public interface PullTing { void DisEnable(); }
public class ObjectPull : MonoBehaviour
{
    public static ObjectPull objectPull;
    public ParticleSystem AngelParticle;
    public ParticleSystem RainArrow;
    public ParticleSystem Slow;
    public ParticleSystem InstantKill;
    public ParticleSystem Assasin;
    public ParticleSystem Warrior;
    public ParticleSystem Poison;
    public ParticleSystem Tornado;
    public ParticleSystem Stun;
    public ParticleSystem Rank2;
    public ParticleSystem Rank3;

    public PullsObject[] test;
    public AtkPariclePullsObject[] AtkParticle;
    public EtcParticles[] EtcEffect;
    public ParticleSystem TowerRange;

    [SerializeField]
    static Dictionary<Pull, Queue<GameObject>> PullDictionary = new Dictionary<Pull, Queue<GameObject>>();
    [SerializeField]
    static Dictionary<Job, Queue<GameObject>> JobAtkDictionary = new Dictionary<Job, Queue<GameObject>>();
    [SerializeField]
    static Dictionary<EtcParticle, Queue<ParticleSystem>> EffectPulling = new Dictionary<EtcParticle, Queue<ParticleSystem>>();

    private void Awake()
    {
        if (objectPull == null)
            objectPull = this;
        else if (objectPull != this)
            Destroy(gameObject);

        Init();
    }
    [ContextMenu("확인")]
    public void Check()
    {
        foreach (KeyValuePair<Pull, Queue<GameObject>> keyValuePair in PullDictionary)
        {
            Debug.Log("Pull : " + keyValuePair.Key + "   value : " + keyValuePair.Value.Count);
        }
    }
    void Init()
    {
        for (int i = 0; i < test.Length; i++)
        {
            PullDictionary.Add(test[i].Pullenum, new Queue<GameObject>());
            CreatePull(test[i].Pullenum);
        }
        for (int i = 0; i < AtkParticle.Length; i++)
        {
            JobAtkDictionary.Add(AtkParticle[i].Pullenum, new Queue<GameObject>());
            CreatePull(AtkParticle[i].Pullenum);
        }
        for (int i = 0; i < EtcEffect.Length; i++)
        {
            EffectPulling.Add(EtcEffect[i].Pullenum, new Queue<ParticleSystem>());
            CreatePull(EtcEffect[i].Pullenum);
        }
    }

    #region 투사체
    void CreatePull(Pull pull, int CreateNumber = 10)
    {
        Debug.Log("CreatePull");
        GameObject pullObject = null;
        //해당 오브젝트를 목록에서 찾아서
        for (int i = 0; i < test.Length; i++)if (test[i].Pullenum == pull){ pullObject = test[i].Prefab;break;}
        // pullObject = PullDictionary[pull];

        //찾는데 성공했다면
        if (pullObject != null)
        {
            //해당오브젝트 생성
            GameObject enqueueObject = null;
            for (int i = 0; i < CreateNumber; i++)
            {
                enqueueObject = Instantiate(pullObject);
                enqueueObject.transform.parent = transform;
                enqueueObject.SetActive(false);
                //PullDictionary[pull].Enqueue(enqueueObject);
            }
        }
        else Debug.LogWarning("Cant Find Object");
    }
    void CreatePull(Job pull, int CreateNumber = 3)
    {
       // Debug.LogWarning("CreatePull : Job" + pull);
        //Debug.Log("CreatePull");
        GameObject pullObject = null;
        for (int i = 0; i < AtkParticle.Length; i++) if (AtkParticle[i].Pullenum == pull) { pullObject = AtkParticle[i].Prefab; break; }
        // pullObject = PullDictionary[pull];
        if (pullObject != null)
        {
            GameObject enqueueObject = null;
            for (int i = 0; i < CreateNumber; i++)
            {
                enqueueObject = Instantiate(pullObject);
                enqueueObject.GetComponent<RangeChapBullet>().JobSet(pull, 1);
                enqueueObject.transform.parent = transform;
                enqueueObject.SetActive(false);
            }
        }
        else Debug.LogWarning("Cant Find Object");
    }
    void CreatePull(EtcParticle pull, int CreateNumber = 3)
    {
        ParticleSystem pullObject = null;
        for (int i = 0; i < EtcEffect.Length; i++) 
            if (EtcEffect[i].Pullenum == pull) 
            { pullObject = EtcEffect[i].Prefab; break; }
        if (pullObject != null)
        {
            ParticleSystem enqueueObject = null;
            for (int i = 0; i < CreateNumber; i++)
            {
                enqueueObject = Instantiate(pullObject);
                enqueueObject.transform.parent = transform;
                EffectPulling[pull].Enqueue(enqueueObject);
                enqueueObject.gameObject.SetActive(false);
                
            }
        }
        else Debug.LogWarning("Cant Find Object");
    }

    public GameObject DeQueue(Pull pull)
    {
        GameObject value;
        if (PullDictionary[pull].Count > 0)
            value = PullDictionary[pull].Dequeue();
        else
        {
            CreatePull(pull);
            value = PullDictionary[pull].Dequeue();
        }
        value.SetActive(true);
        return value;
    }

    public GameObject DeQueue(Job pull)
    {
        GameObject value;
        if (JobAtkDictionary[pull].Count > 0)
            value = JobAtkDictionary[pull].Dequeue();
        else
        {
            CreatePull(pull);
            value = JobAtkDictionary[pull].Dequeue();
        }
        value.gameObject.SetActive(true);
        return value;
    }
    public ParticleSystem DeQueue(EtcParticle pull)
    {
        ParticleSystem value;
       
        if (EffectPulling[pull].Count > 0)
            value = EffectPulling[pull].Dequeue();
        else
        {
            CreatePull(pull);
            value = EffectPulling[pull].Dequeue();
        }
        value.gameObject.SetActive(true);

        return value;
    }

    public void EnQueue(GameObject Used, Pull pull)
    {
        // Debug.LogWarning("EnQueue");
        Used.transform.parent = transform;

        PullDictionary[pull].Enqueue(Used);
    }
    public void EnQueue(GameObject Used, Job pull)
    {
        // Debug.LogWarning("EnQueue");
        Used.transform.parent = transform;
     //   Debug.Log(pull);
        JobAtkDictionary[pull].Enqueue(Used);
    }
    public void EnQueue(ParticleSystem _paticle, 
        EtcParticle pull)
    {
        _paticle.transform.parent = transform;
        _paticle.gameObject.SetActive(false);
        EffectPulling[pull].Enqueue(_paticle);
      
    }
    #endregion
    public GameObject FindParticle(Job effectPull)
    {
        for (int i = 0; i < AtkParticle.Length; i++) if (AtkParticle[i].Pullenum == effectPull) return AtkParticle[i].Prefab;

        return null;
    }
    public void PlayNifParticleOverAndQue(EtcParticle pull, ParticleSystem particle) { StartCoroutine(PlayNifParticleOverAndQueCorutine(pull, particle)); }
    public static IEnumerator PlayNifParticleOverAndQueCorutine(EtcParticle pull, ParticleSystem particle)
    {
        //  Debug.LogWarning("진입"+ particle.name);
        particle.Play();
        // while (true)
        {
            yield return new WaitForEndOfFrame();
            // if (!particle.isPlaying)
            // if(particle.particleCount==0)
            //if(!particle.IsAlive())

           // yield return new WaitForSeconds(particle.main.duration);
           while(true)
            {
                if (!particle.isPlaying)
                {
                    particle.Pause();
                    particle.gameObject.SetActive(false);
                    ObjectPull.objectPull.EnQueue(particle, pull);

                     break;
                }

                yield return new WaitForEndOfFrame();
                // Destroy(particle.gameObject);
                // break;
            }
            //else
            //{
            //    Debug.LogWarning("particle.isPlaying" + particle.isPlaying);
            //}
        }
    }
}
