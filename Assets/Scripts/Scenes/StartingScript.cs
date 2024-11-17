using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartingScript : MonoBehaviour
{
    public void GoToMenu(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
