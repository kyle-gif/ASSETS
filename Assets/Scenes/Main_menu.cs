using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_menu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Dev2_scene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
