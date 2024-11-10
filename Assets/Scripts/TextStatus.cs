using System;
using System.Collections;
using System.Collections.Generic;
using Searching;
using UnityEngine;
using TMPro;

public class TextStatus : MonoBehaviour
{
    public GameObject player;
    private OOPPlayer playerScript;
    [SerializeField] private TextMeshProUGUI health;

    // Start is called before the first frame update
    void Start()
    {
        playerScript = player.GetComponent<OOPPlayer>();
    }

    private void Update()
    {
        health.text = "Health: " + playerScript.health.ToString("F0");
    }
}