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
    
    private void Update()
    {
        if (mapGenerator.treasure.ContainsKey(transform.position))
        {
            mapGenerator.treasure[transform.position].Remove(this);
            if (mapGenerator.treasure[transform.position].Count == 0)
            {
                mapGenerator.treasure.Remove(transform.position);
            }
        }
        if (!mapGenerator.treasure.ContainsKey(transform.position))
        {
            mapGenerator.treasure[transform.position] = new List<OOPTreasure>();
        }
        mapGenerator.treasure[transform.position].Add(this);
    }
    
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
        if (this != null)
        {
            StartCoroutine(WaitForInput());
            audioManager.PlaySFX(audioManager.chest);
        }
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
            ItemSO clonedItem = CreateItemClone(randomItem);
            ModifyItemStats(clonedItem);
            inventoryData.AddItem(clonedItem, 1);
            Debug.Log(
                $"Random Item {clonedItem.itemName} of type {clonedItem.itemType} added to inventory with modified stats.");
        }
        else
        {
            Debug.LogError("Random item is null!");
        }
    }
    
    private ItemSO CreateItemClone(ItemSO originalItem)
    {
        ItemSO clonedItem = Instantiate(originalItem);
        return clonedItem;
    }

    private void ModifyItemStats(ItemSO item)
    {
        int randomStatBoost = UnityEngine.Random.Range(1, 3+GameManager.level);
        if (item.itemType == Itemtype.Armor)
        {
            List<string> suffixes = new List<string>
            {
                "Flame", "Frost", "Shadow", "Light", "Storm", 
                "Earth", "Mystic", "Venom", "Thunder", "Wind", 
                "Ocean", "Crystal", "Inferno", "Guardian", "Chaos", 
                "Void", "Phoenix", "Dragon", "Eclipse", "Star"
            };
            string randomSuffix = suffixes[UnityEngine.Random.Range(0, suffixes.Count)];
            item.name = $"Armor of {randomSuffix}";
        }
        if (item.itemType == Itemtype.Weapon)
        {
            List<string> suffixes = new List<string>
            {
                "Flame", "Frost", "Shadow", "Light", "Storm", 
                "Earth", "Mystic", "Venom", "Thunder", "Wind", 
                "Ocean", "Crystal", "Inferno", "Guardian", "Chaos", 
                "Void", "Phoenix", "Dragon", "Eclipse", "Star"
            };
            string randomSuffix = suffixes[UnityEngine.Random.Range(0, suffixes.Count)];
            item.name = $"{randomSuffix} Sword";
        }
        if (item.itemType == Itemtype.Accessory)
        {
            List<string> suffixes = new List<string>
            {
                "Flame", "Frost", "Shadow", "Light", "Storm", 
                "Earth", "Mystic", "Venom", "Thunder", "Wind", 
                "Ocean", "Crystal", "Inferno", "Guardian", "Chaos", 
                "Void", "Phoenix", "Dragon", "Eclipse", "Star"
            };
            string randomSuffix = suffixes[UnityEngine.Random.Range(0, suffixes.Count)];
            item.name = $"Bracelet of {randomSuffix}";
        }
        item.itemStat += randomStatBoost;
        item.Description = $"{item.Description} {item.itemStat}";
        FindObjectOfType<ItemSpriteRandomizer>().AssignRandomSprite(item);
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