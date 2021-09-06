using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeChapBullet : MonoBehaviour, PullTing
{
    public Champion owner;
    public int Dmg;
    ParticleSystem ownparticle;
    ParticleSystem.Particle[] particles;
    [SerializeField]
    Job job;
    [SerializeField]
    int jobrank;
    [SerializeField]
    Species spiecies;
    [SerializeField]
    int spieciesrank;
    GameObject target;
    public float Speed = 5;
    bool JobActive = true;
    bool SpActive = true;
    private void Awake()
    {
        ownparticle = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[ownparticle.main.maxParticles];
    }
    public void SetAll(int _Dmg, Job _job, Species _spiecies, float _Speed, GameObject _target, bool _JobActive = true, bool _SpActive = true)
    {
        Dmg = _Dmg;
        target = _target;
        job = _job;
        spiecies = _spiecies;
        Speed = _Speed;
        JobActive = _JobActive;
        SpActive = _SpActive;
    }
    public RangeChapBullet Set(GameObject _target) { target = _target; return this; }
    public RangeChapBullet JobSet(Job _job, int _jobrank) {  job = _job; jobrank = _jobrank; return this; }
    public RangeChapBullet SpieciesSet(Species _spiecies, int _spieciesrank) { spiecies = _spiecies; spieciesrank = _spieciesrank; return this; }

    public RangeChapBullet SetSpeed(float _Speed) { Speed = _Speed; return this; }

    private void FixedUpdate()
    {
        if (target != null && target.activeInHierarchy)
        {  
            float radian = Mathf.Atan2(target.transform.position.y-transform.position.y,  target.transform.position.x- transform.position.x);
            int num = ownparticle.GetParticles(particles);
            for (int i = 0; i < num; i++)
            {
                //Debug.Log(-radian * Mathf.Rad2Deg );
                particles[i].rotation = (-radian * Mathf.Rad2Deg);
            }
            ownparticle.SetParticles(particles, num);

            Vector2 dir = target.transform.position - transform.position;
            transform.Translate(dir.normalized * Speed * Time.deltaTime);
        }
        else
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (target == collision.gameObject)
        {
            Job tempJob = job;
            Species Tempspecies = spiecies;
            if (!JobActive)
                tempJob = Job.LastNumber;
            if (!SpActive)
                Tempspecies = Species.LastNumber;
            EnemyHealth temp = collision.GetComponent<EnemyHealth>();
            if (temp == null) Debug.LogError("EnemyHealth null");
            else
            {
                temp.Synergy(tempJob, Tempspecies,owner);
                temp.DamageIt(Dmg);
                gameObject.SetActive(false);
            }
        }
    }
    private void OnDisable()
    {
        DisEnable();
    }

    public void DisEnable()
    {
        ObjectPull.objectPull.EnQueue(gameObject, job);
    }
}
public class BoxBullet : MonoBehaviour
{
    public int Dmg;
    Job job;
    [SerializeField]
    int jobrank;
    [SerializeField]
    Species spiecies;
    [SerializeField]
    int spieciesrank;

    bool IsReUse = false;
    public int MaxNum = 1;
    int curnum = 0;
    public float time;
    public float _CurTime;
    Champion owner;
    public void SetAll(int _Dmg, Job _job, Species _spiecies, float _Time, bool _IsReUse, int _MaxNum)
    {
        job = _job;
        spiecies = _spiecies;
        IsReUse = _IsReUse;

        MaxNum = _MaxNum;
        time = _Time;
    }
    public BoxBullet JobSet(Job _job, int _jobrank) { job = _job; jobrank = _jobrank; return this; }
    public BoxBullet SpieciesSet(Species _spiecies, int _spieciesrank) { spiecies = _spiecies; spieciesrank = _spieciesrank; return this; }

    private void Update()
    {
        _CurTime += Time.deltaTime;
        if (_CurTime > time)
        {
            if (IsReUse) gameObject.SetActive(false);
            else Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Enter");
        EnemyHealth temp = collision.GetComponent<EnemyHealth>();
        if (temp == null) Debug.LogError("EnemyHealth null");
        else
        {
            temp.Synergy(job, spiecies, owner);
            temp.DamageIt(Dmg);
            curnum++;
            if (curnum >= MaxNum)
            {
                if (IsReUse) gameObject.SetActive(false);
                else Destroy(gameObject);
            }
        }

    }
    public void use()
    {
        gameObject.SetActive(true);
        curnum = 0;
        _CurTime = 0;
    }
    public void Off()
    {
        gameObject.SetActive(false);
    }
}