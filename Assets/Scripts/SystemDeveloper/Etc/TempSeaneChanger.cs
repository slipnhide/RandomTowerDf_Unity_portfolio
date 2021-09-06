using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempSeaneChanger : MonoBehaviour
{
    public void ToGameSeane()
    {
        SceneManager.LoadScene("InGame");
    }
    public void ToLogin()
    {
        NetworkManager.networkManager.EndGame();
    }
}
