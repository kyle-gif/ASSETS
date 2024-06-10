using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game_Over : MonoBehaviour
{
    public void Retry()
    {
        SceneManager.LoadScene("Dev2_scene");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
