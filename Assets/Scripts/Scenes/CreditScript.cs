using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditScript : MonoBehaviour
{
    public void CreditScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
