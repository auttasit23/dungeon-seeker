using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public void GoToGameStart(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void Credit(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
