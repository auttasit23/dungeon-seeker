using System;
using System.Collections;
using System.Collections.Generic;
using Inventory.Model;
using System.Linq;
using Searching;
using UnityEngine;
using Inventory;
using TMPro;
using UnityEngine.UI;
public class OOPTreasure : Identity
{
    private OOPPlayer player;
    [SerializeField] private InventorySO inventoryData; 
    [SerializeField] private ItemDatabaseSO itemDatabase;


    private void Start()
    {
        player = FindObjectOfType<OOPPlayer>();
        mapGenerator = FindObjectOfType<OOPMapGenerator>();
        if (mapGenerator == null)
        {
            Debug.LogError("OOPMapGenerator not found in the scene!");
        }
    }

    public override void Hit()
    {
        StartCoroutine(WaitForInput());
        audioManager.PlaySFX(audioManager.chest);
    }

    private IEnumerator WaitForInput()
    {
        mapGenerator.selectingTreasure = true;
        bool optionSelected = false;
        mapGenerator.choose.SetActive(true);
        while (!optionSelected)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log("Option 1 selected");
                Option1Action();
                audioManager.PlaySFX(audioManager.complete);
                mapGenerator.choose.SetActive(false);
                optionSelected = true;
                mapGenerator.selectingTreasure = false;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("Option 2 selected");
                Option2Action();
                audioManager.PlaySFX(audioManager.complete);
                mapGenerator.choose.SetActive(false);
                optionSelected = true;
                mapGenerator.selectingTreasure = false;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Debug.Log("Option 3 selected");
                Option3Action();
                audioManager.PlaySFX(audioManager.complete);
                mapGenerator.choose.SetActive(false);
                optionSelected = true;
                mapGenerator.selectingTreasure = false;
            }

            yield return null;
        }
    }

    private void Option1Action()
    {
        Debug.Log("Executed action for option 1");
        ChooseRandomEquipment();
        Destroy(gameObject);
        

    }

    private void Option2Action()
    {
        Debug.Log("Executed action for option 2");
        player.IncreaseStatPoints(5);
        Destroy(gameObject);
    }

    private void Option3Action()
    {
        Debug.Log("Executed action for option 3");
        mapGenerator.player.Heal(25f);
        Destroy(gameObject);
    }
    public void ChooseRandomEquipment()
    {
        Itemtype[] equipmentTypes = { Itemtype.Weapon, Itemtype.Armor, Itemtype.Accessory };
        Itemtype randomType = equipmentTypes[UnityEngine.Random.Range(0, equipmentTypes.Length)];

        ItemSO randomItem = GetRandomItemByType(randomType);

        if (randomItem != null)
        {
            inventoryData.AddItem(randomItem, 1);
            Debug.Log($"Random Item {randomItem.itemName} of type {randomItem.itemType} added to inventory.");
        }
        else
        {
            Debug.LogError("Random item is null!");
        }

    }

    public ItemSO GetRandomItemByType(Itemtype type)
    {
        if (itemDatabase == null)
        {
            Debug.LogError("ItemDatabaseSO is not assigned to OOPTreasure!");
            return null;
        }

        List<ItemSO> itemsOfType = itemDatabase.GetItemsByType(type);

        if (itemsOfType == null || itemsOfType.Count == 0)
        {
            Debug.LogError($"No items found for type {type}");
            return null;
        }

        return itemsOfType[UnityEngine.Random.Range(0, itemsOfType.Count)];
    }

    
}