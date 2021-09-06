using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurverPanel : MonoBehaviour , Notify
{
    public PanelType[] panelTypes;
    public GameObject[] falseObjects;
    public Text[] NullTexts;
    public void ModeChage(PanelType _panelType)
    {
        bool IsOn = false;
        foreach (PanelType panelType in panelTypes)
        {
            if (_panelType == panelType)
            {
                IsOn = true;
                break;
            }
        }

        if (!IsOn)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            if (NullTexts != null)
                foreach (Text text in NullTexts)
                {
                    if (text == null)
                    {
                        Debug.LogWarning("SurverPanel textList Is Missing" + gameObject.name);
                        continue;
                    }
                    text.text = "1";
                }
            if (falseObjects != null)
                foreach (GameObject _gameObject in falseObjects)
                {
                    if (_gameObject == null)
                    {
                        Debug.LogWarning("SurverPanel falseObjectsList Is Missing" + gameObject.name);
                        continue;
                    }
                    _gameObject.SetActive(false);
                }
        }
    }
}
