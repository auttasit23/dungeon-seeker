using Inventory;
using Inventory.Model;
using Searching;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Searching.OOPPlayer;

public class ChestManager : MonoBehaviour
{
    public InventorySO inventoryController;
    public OOPPlayer player;
    // Start is called before the first frame update
   /* private void Start()
    {
        player = GetComponent<OOPPlayer>();
    }
    public void ChooseRandomEquipment()
    {
        Itemtype[] equipmentTypes = { Itemtype.Weapon, Itemtype.Armor, Itemtype.Accessory };
        Itemtype randomType = equipmentTypes[Random.Range(0, equipmentTypes.Length)];
        ItemSO randomItem = ItemFactory.CreateRandomItem(randomType, spriteCollection);
        inventoryController.AddItem(randomItem);

        UpdateUI(randomItem);
        CloseChest();
    }

    public void ChooseHealthPotion()
    {
        ItemSO potion = ItemFactory.CreateRandomItem(Itemtype.Potion, spriteCollection);
        player.IncreaseHealth(50);
        UpdateUI(potion);
        CloseChest();
    }

    public void ChooseStatPoint()
    {
        ItemSO statPoint = ItemFactory.CreateRandomItem(Itemtype.Skillpoint, spriteCollection);
        player.IncreaseStatPoints(1);
        UpdateUI(statPoint);
        CloseChest();
    }

    private void UpdateUI(ItemSO item)
    {
        itemImageDisplay.sprite = item.ItemImage;
        itemDescriptionDisplay.text = $"{item.Name}\n{item.Description}";
    }

    private void CloseChest()
    {
        FindObjectOfType<ChestInteraction>().CloseChestUI();
    }*/
}
