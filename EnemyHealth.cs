using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public static EnemyHealth instance;
    [SerializeField]
    public float maxHp = 100;

    public Image hpBar;


    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
    }

    private void Update()
    {
        hpBar.fillAmount = maxHp / 100f;
    }

}
