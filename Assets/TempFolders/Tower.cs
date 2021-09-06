using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Tower : MonoBehaviour
{
    [Header("Shop")]
    [Range(1,5)]
    public int NeedGold;
    public Sprite ShopImage;
    public int TowerRank;
    //public string Name;
    [Header("Character")]
    public Job job;
    public Species species;
}
