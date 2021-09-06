using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Explain : MonoBehaviour
{
    //public static Explain explain;
    public GameObject explainPanel;
    public Image FaceImage;
    public Text Explaintext;
    public Text Name;
    private void Awake()
    {
    }
    public void Set(int [] Job_Sp)
    {
        explainPanel.SetActive(true);
        FaceImage.sprite = Ingame_View_Controler.ingame_View_Controler.FindShopImage(Job_Sp[0], Job_Sp[1]);
    }
    public void Off()
    {
        explainPanel.SetActive(false);
    }
}
