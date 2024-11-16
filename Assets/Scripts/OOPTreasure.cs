using System;
using System.Collections;
using System.Collections.Generic;
using Inventory.Model;
using System.Linq;
using Searching;
using UnityEngine;
using Inventory;

public class OOPTreasure : Identity
{
    private OOPMapGenerator mapGenerator;

    [SerializeField] private InventorySO inventoryData; // เพิ่มการอ้างอิงถึง InventorySO

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
        ChooseRandomEquipment();
        Destroy(gameObject);
    }

    private void Option3Action()
    {
        Debug.Log("Executed action for option 3");
        ChooseRandomEquipment();
        Destroy(gameObject);
    }


    public void ChooseRandomEquipment()
    {
        Itemtype[] equipmentTypes = { Itemtype.Weapon, Itemtype.Armor, Itemtype.Accessory };
        Itemtype randomType = equipmentTypes[UnityEngine.Random.Range(0, equipmentTypes.Length)];

        // สุ่มไอเทมตามประเภทที่เลือก
        ItemSO randomItem = GetRandomItemByType(randomType);

        // เพิ่มไอเทมลงใน Inventory
        if (randomItem != null)
        {
            inventoryData.AddItem(randomItem, 1);  // เพิ่มไอเทมที่สุ่มได้ลงใน inventory
            Debug.Log($"Random Item {randomItem.Name} of type {randomItem.itemType} added to inventory.");
        }
        else
        {
            Debug.LogError("Random item is null!");
        }
    }

    private ItemSO GetRandomItemByType(Itemtype itemType)
    {
        // สร้างรายการไอเทมที่มีประเภทตรงกับที่เลือก
        List<ItemSO> availableItems = new List<ItemSO>();
        foreach (ItemSO item in Resources.LoadAll<ItemSO>("Data"))
        {
            if (item.itemType == itemType)
            {
                availableItems.Add(item);
            }
        }

        // เลือกไอเทมสุ่มจากรายการที่ได้
        if (availableItems.Count > 0)
        {
            return availableItems[UnityEngine.Random.Range(0, availableItems.Count)];
        }

        return null;
    }
}