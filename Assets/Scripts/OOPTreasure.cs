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
    private OOPMapGenerator mapGenerator;

    [SerializeField] private InventorySO inventoryData; 
    [SerializeField] private ItemDatabaseSO itemDatabase;
    [SerializeField] private TextMeshProUGUI statText;

    private int statTotal = 0;

    private void Start()
    {
        mapGenerator = FindObjectOfType<OOPMapGenerator>();
        if (mapGenerator == null)
        {
            Debug.LogError("OOPMapGenerator not found in the scene!");
        }
        statText = UIManager.Instance.statText;

        if (statText == null)
        {
            Debug.LogError("StatText is not assigned in UIManager!");
        }
        UpdateStatText();
    }

    public override void Hit()
    {
        StartCoroutine(WaitForInput());
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
                mapGenerator.choose.SetActive(false);
                optionSelected = true;
                mapGenerator.selectingTreasure = false;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("Option 2 selected");
                Option2Action();
                mapGenerator.choose.SetActive(false);
                optionSelected = true;
                mapGenerator.selectingTreasure = false;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Debug.Log("Option 3 selected");
                Option3Action();
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
        statTotal++; 
        UpdateStatText(); 
        Destroy(gameObject);
    }

    private void Option3Action()
    {
        Debug.Log("Executed action for option 3");
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
            Debug.Log($"Random Item {randomItem.Name} of type {randomItem.itemType} added to inventory.");
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

    private void UpdateStatText()
    {
        if (statText != null)
        {
            statText.text = $"Stat Total: {statTotal}";
        }
        else
        {
            Debug.LogError("Stat Text is not assigned in the inspector!");
        }
    }
}