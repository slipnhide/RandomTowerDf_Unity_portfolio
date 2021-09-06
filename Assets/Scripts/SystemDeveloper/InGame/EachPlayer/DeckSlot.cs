using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class DeckSlot : MonoBehaviour
{
    bool IsDraging=false;
   // RaycastHit hit;
    Ray ray;
    protected Image SlotImg;
   // public Sprite BaseSlotImage;
   // protected GameObject CurSlotTower = null;
    protected Explain explain;
    // Ray2D ray;
    RaycastHit2D hit;
    public Image Species;
    public Image Job;
    public int ChapionRank=1;
    protected int[] Job_Spiceis;
    protected virtual void Awake()
    {
        SlotImg = GetComponent<Image>();
        explain = FindObjectOfType<Explain>();
        ColorOnOff(false);
    }
    public virtual void SetSlot(int[] Info,int Rank=1)
    {
        ColorOnOff(true);
        Job_Spiceis = Info;
        ChapionRank = Rank;
        //Champion champion = _Tower.GetComponent<Champion>();
        if(Rank<Common_Static_Field.MaxMerge)
        if (SynergySystem.synergySystem.IfAddChampIsMerged(Info, Rank))
        {
            Debug.Log("MergeTrue");
            //삭제처리
            SynergySystem.synergySystem.DestroyRank(Info, Rank,transform);
            SetSlot(Info, Rank + 1);
            return;
        }
        
        SlotImg.sprite = Ingame_View_Controler.ingame_View_Controler.FindShopImage(Info[0], Info[1]);
        Job.sprite = Ingame_View_Controler.ingame_View_Controler.FindMedal((Job)Job_Spiceis[0], Rank-1);
        Species.sprite = Ingame_View_Controler.ingame_View_Controler.FindMedal((Species)Job_Spiceis[1], Rank-1);
    }
    public virtual void ToEmpty()
    {
        Job_Spiceis = null;
        ColorOnOff(false);
        ChapionRank = 1;
    }
    public int[] GetJob_Sp() { return Job_Spiceis; }
    public bool IsEmpty() { if (Job_Spiceis==null) return true; else return false; }
    protected virtual void ColorOnOff(bool IsOn)
    {
        if (IsOn)
        {
            Species.color = Color.white;
            Job.color = Color.white;
            SlotImg.color = Color.white;
        }
        else
        {
            Species.color = Color.clear;
            Job.color = Color.clear;
            SlotImg.color = Color.clear;
        }
    }
    public void DragFromDeck()
    {
        if (IsEmpty() || IsDraging) return;
        IsDraging = true;
        GameObject temp = new GameObject("CreateImage");
        ParticleSystem Liner = Instantiate(ObjectPull.objectPull.TowerRange);
        Liner.transform.parent = temp.transform;
        ParticleSystem.ShapeModule ps = Liner.shape;
        // ps.radius = CurSlotTower.GetComponent<Champion>().Range;
        float[] stats= Common_Static_Field.GetStat(PlayerInfo_Master.master.MyInfo.ActorNumber,(Job)Job_Spiceis[0], (Species)Job_Spiceis[1],ChapionRank);
        ps.radius = stats[3];
         SpriteRenderer sr = temp.AddComponent<SpriteRenderer>();
        sr.sprite = Ingame_View_Controler.ingame_View_Controler.FindShopImage(Job_Spiceis[0], Job_Spiceis[1]);
        sr.color = new Color(1, 1, 1, .5f);
        Ingame_View_Controler.ingame_View_Controler.TileOpen(true);
        StartCoroutine(Drag(temp));
    }

    IEnumerator Drag(GameObject temp)
    {
        Camera Mc = Camera.main;
        Debug.Log(Mc.name);
        Vector3 pos = Mc.ScreenToWorldPoint(Input.mousePosition);
        //yield return null;
        while (true)
        {

            //ray = Mc.ScreenPointToRay(Input.mousePosition);
            //ray = Mc.ScreenPointToRay(Input.mousePosition);
            // if(Physics.Raycast(ray,out hit))
            //if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            if (Mc.orthographic)
            {
                pos = Mc.ScreenToWorldPoint(Input.mousePosition);
                Debug.Log("pos" + pos);
                hit = Physics2D.Raycast(pos, transform.forward, Mathf.Infinity);
            }
            else
            {
                #region 시도2
                {
                    ray = Mc.ScreenPointToRay(Input.mousePosition);
                    hit = Physics2D.GetRayIntersection(ray);
                }
                #endregion
                #region 시도1
                /*
                {
                    RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray, Mathf.Infinity, LayerMask.NameToLayer("Tile"));
                    Debug.Log(hits.Length);
                    foreach (RaycastHit2D h in hits)
                    {
                        if (h.transform.CompareTag("Tile"))
                        {
                            hit = h;
                            break;
                        }
                    }
                }
                */
                #endregion
                //hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity,LayerMask.NameToLayer("Tile"));
                //if (hit!=null)
                // Debug.Log(hit.transform.name);
            }
            if (hit)
            {
            //     Debug.Log("충돌");
                if (hit.transform.CompareTag("MyTile"))
                {
                    Tile _tile = hit.transform.GetComponent<Tile>();
                  //  Debug.Log("타일"+hit.point+"  Pos"+pos);
                    if (!_tile.IsBuildTower)
                    {
                        temp.transform.position = hit.transform.position;
                        if (Input.GetMouseButtonUp(0))
                        {
                            IsDraging = false;
                           // Debug.Log("설치");
                            Destroy(temp);
                            //if (towerSpawner.TowerSpawn(CurSlotTower, hit))
                            if (Ingame_View_Controler.ingame_View_Controler.TowerSpawn(Job_Spiceis, _tile, hit.transform.position,ChapionRank))
                            {
                                ToEmpty();
                                Ingame_View_Controler.ingame_View_Controler.DeckSlotReTake();
                            }
                            break;
                        }
                    }
                }
            }
            else
            {
                /*
                temp.transform.position = new Vector3(pos.x, pos.y, 0);
                Debug.Log(new Vector3(pos.x, pos.y, 0));*/
            }
            
            if (Input.GetMouseButtonUp(0))
            {
                //Debug.Log("마우스 띔");
                Destroy(temp);
                IsDraging = false;
                break;            
            }
            yield return null;
        }
        Ingame_View_Controler.ingame_View_Controler.TileOpen(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //if (CurSlotTower == null || IsDraging) return;
        //IsDraging = true;
        //GameObject temp = new GameObject("CreateImage");
        //SpriteRenderer sr = temp.AddComponent<SpriteRenderer>();
        //sr.sprite = CurSlotTower.ShopImage;
        //// StartCoroutine(Drag(temp));
        //temp.AddComponent<MouseDrag>();
        //temp.GetComponent<MouseDrag>().OnBeginDrag(eventData);
        //temp.GetComponent<MouseDrag>().OnDrag(eventData);
    }
    //public override void Set(Player_Info _player_Info, GachaSystem _gachaSystem)
    //{
    //    base.Set(_player_Info, _gachaSystem);
    //    towerSpawner = player_Info.gameObject.GetComponent<TowerSpawner>();
    //}
}
