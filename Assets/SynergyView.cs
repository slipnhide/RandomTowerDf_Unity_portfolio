using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SynergyView
{
    Image[] images;
    public void SetImages(Image[] _images) { images = _images; }

    public void MedalUpdate(int[] job, int[] Sp)
    {
        Debug.Log("SynergyViewMedalUpdate");
        //ToSimpleJson<int[]> Rank = JsonUtility.FromJson<ToSimpleJson<int[]>>(job);
        int[] Rank = job;
        for (int i = 0; i < (int)Job.LastNumber; i++)
        {
            if (Rank[i] == 0)
                images[i].color = Color.black;
            else
            {
                images[i].color = Color.white;
                images[i].sprite = Ingame_View_Controler.ingame_View_Controler.FindMedal((Job)i, Rank[i]-1);
            }
        }
        Rank = Sp;
        int JobLast = (int)Job.LastNumber;
        for (int i = 0; i < (int)Species.LastNumber; i++)
        {
            if (Rank[i] == 0)
                images[JobLast+i].color = Color.black;
            else
            {
                images[JobLast + i].color = Color.white;
                images[JobLast + i].sprite = Ingame_View_Controler.ingame_View_Controler.FindMedal((Species)i, Rank[i]-1);
            }
        }
    }
}
