using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] public static int level;
    
    
    public void GotoMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
