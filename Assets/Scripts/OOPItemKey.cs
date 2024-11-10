using System;
using System.Collections;
using System.Collections.Generic;
using Searching;
using UnityEngine;

public class OOPItemKey : Identity
{
    public string key;

    private void Start()
    {
        mapGenerator = FindObjectOfType<OOPMapGenerator>();
        if (mapGenerator == null)
        {
            Debug.LogError("OOPMapGenerator not found in the scene!");
        }
    }

    public override void Hit()
    {
        Debug.Log("yes");
        mapGenerator.player.inventory.AddItem(key);
        Destroy(gameObject);
    }
}
