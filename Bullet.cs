using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    public int speed = 3;
    public int dmg = 20;
    GameObject target; 
    Vector2 dir;
    Vector2 dirNo;

    EnemyHealth enemyHealth;

    void Start()
    {
        target = GameObject.FindWithTag("Enemy");

        dir = target.transform.position - transform.position;
        dirNo = dir.normalized;

        enemyHealth = FindObjectOfType<EnemyHealth>();
    }

    void Update()
    {
        transform.Translate(dir * speed * Time.deltaTime);
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.gameObject.tag == "Enemy")
    //    {
    //        Destroy(collision.gameObject);
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            Destroy(gameObject);
            enemyHealth.maxHp -= dmg;
            if(enemyHealth.maxHp <= 0)
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
