using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TempDoNotDestroyType { LoginPanel}

public class DoNotDestroy : MonoBehaviour
{
    public TempDoNotDestroyType tempDoNotDestroyType;
    private void Awake()
    {
        DoNotDestroy[] DoNotDestroys = FindObjectsOfType<DoNotDestroy>();

        foreach (DoNotDestroy doNotDestroy in DoNotDestroys)
        {
            if (doNotDestroy.tempDoNotDestroyType == tempDoNotDestroyType)
            {
                if (doNotDestroy != this)
                {
                    //gameObject.name = "Destroyed";
                    Debug.Log("Destroy_" + gameObject.name);
                    Destroy(gameObject);
                    return;
                }
            }
        }
        DontDestroyOnLoad(gameObject);
    }
}

