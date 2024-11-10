using System.Collections.Generic;
using UnityEngine;

public class UIInventoryPage : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private UIInventoryItem inventoryItemPrefab;
    [SerializeField] private RectTransform inventoryPanel;
    [SerializeField] private UIItemDescription inventoryItemUIDescription; // Description UI to reset on show

    [Header("Equipment Slots")]
    [SerializeField] private UIInventoryItem weaponSlot;
    [SerializeField] private UIInventoryItem armorSlot;
    [SerializeField] private UIInventoryItem accessorySlot;

    private List<UIInventoryItem> inventorySlots = new List<UIInventoryItem>();
    private UIInventoryItem selectedSlot;

    private void Start()
    {
        InitializeInventory(10); // Initialize inventory with 10 slots
        SetupEquipmentSlots();
        Hide(); // Hide inventory initially
    }

    public void InitializeInventory(int slotCount)
    {
        for (int i = 0; i < slotCount; i++)
        {
            var slot = Instantiate(inventoryItemPrefab, inventoryPanel);
            inventorySlots.Add(slot);
            slot.OnItemClicked += SelectSlot;
            slot.OnItemDropped += SwapItems;
            slot.OnItemRightClicked += QuickEquipItem; // Listen for right-click events
        }
    }

    private void SetupEquipmentSlots()
    {
        weaponSlot.OnItemClicked += SelectSlot;
        armorSlot.OnItemClicked += SelectSlot;
        accessorySlot.OnItemClicked += SelectSlot;
    }

    private void SelectSlot(UIInventoryItem slot)
    {
        if (selectedSlot != null) selectedSlot.Deselect();
        selectedSlot = slot;
        selectedSlot.Select();
    }

    private void SwapItems(UIInventoryItem targetSlot)
    {
        if (selectedSlot == null || targetSlot == selectedSlot) return;

        var selectedData = (selectedSlot.itemImage.sprite, selectedSlot.quantityText.text);
        var targetData = (targetSlot.itemImage.sprite, targetSlot.quantityText.text);

        selectedSlot.SetData(targetData.Item1, int.Parse(targetData.Item2));
        targetSlot.SetData(selectedData.Item1, int.Parse(selectedData.Item2));
    }

    private void QuickEquipItem(UIInventoryItem itemSlot)
    {
        if (itemSlot == null || itemSlot.isEmpty) return;

        if (itemSlot.name.Contains("Weapon"))
        {
            EquipItem(itemSlot, weaponSlot);
        }
        else if (itemSlot.name.Contains("Armor"))
        {
            EquipItem(itemSlot, armorSlot);
        }
        else if (itemSlot.name.Contains("Accessory"))
        {
            EquipItem(itemSlot, accessorySlot);
        }
    }

    private void EquipItem(UIInventoryItem sourceSlot, UIInventoryItem targetSlot)
    {
        Sprite sourceSprite = sourceSlot.itemImage.sprite;
        string sourceQuantity = sourceSlot.quantityText.text;

        targetSlot.SetData(sourceSprite, int.Parse(sourceQuantity));
        sourceSlot.ResetData();
    }

    public void AddItemToInventory(Sprite itemSprite, int quantity)
    {
        foreach (var slot in inventorySlots)
        {
            if (slot.isEmpty)
            {
                slot.SetData(itemSprite, quantity);
                return;
            }
        }
    }
    // Show and Hide methods to control the panel's visibility
    public void Show()
    {
        gameObject.SetActive(true);
        inventoryItemUIDescription.ResetDescription(); // Reset description when showing
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
