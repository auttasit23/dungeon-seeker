using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Inventory.UI;

namespace Inventory.Model
{
    [CreateAssetMenu]

    public class InventorySO : ScriptableObject
    {
        [SerializeField]
        private List<InventoryItem> inventoryItems;

        [field: SerializeField]
        public int Size { get; private set; } = 10;

        public event Action<Dictionary<int, InventoryItem>> OnInventoryUpdated;

        public void Initialize()
        {
            inventoryItems = new List<InventoryItem>();

            for (int i = 0; i < Size; i++)
            {
                inventoryItems.Add(InventoryItem.GetEmptyItem());
            }
        }

        public void AddItem(ItemSO item, int quantity)
        {
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i].IsEmpty)
                {
                    inventoryItems[i] = new InventoryItem
                    {
                        item = item,
                        quantity = quantity
                    };

                    // ตรวจสอบว่ามีไอเทมที่สวมใส่อยู่และอัปเดต
                    UpdateEquippedStatus(item);
                    InformAboutChange();
                    return;
                }
            }
        }
        
        private void UpdateEquippedStatus(ItemSO item)
        {
            var equippedItems = FindObjectOfType<EquipmentSlotManager>().equipmentSlots;
            for (int i = 0; i < equippedItems.Length; i++)
            {
                if (equippedItems[i] == item)
                {
                    Debug.Log($"Item {item.itemName} is already equipped in slot {i}");
                    return;
                }
            }
        }

        
        public void RemoveItem(ItemSO itemToRemove)
        {
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i].item == itemToRemove)
                {
                    inventoryItems[i] = InventoryItem.GetEmptyItem();
                    InformAboutChange(); // แจ้ง UI ให้รีเฟรชข้อมูล
                    return;
                }
            }
        }
        /*public void RemoveItemAt(int index)
        {
            if (index >= 0 && index < inventoryItems.Count)
            {
                inventoryItems[index] = InventoryItem.GetEmptyItem();
                InformAboutChange();
            }
        }*/



        public Dictionary<int, InventoryItem> GetCurrentInventoryState()
        {
            Dictionary<int, InventoryItem> returnValue =
                new Dictionary<int, InventoryItem>();

            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i].IsEmpty)
                    continue;
                returnValue[i] = inventoryItems[i];
            }
            return returnValue;
        }

        internal InventoryItem GetItemAt(int itemIndex)
        {
            return inventoryItems[itemIndex];
        }

        public void AddItem(InventoryItem item)
        {
            AddItem(item.item, item.quantity);
        }

        public void SwapItems(int itemIndex_1, int itemIndex_2)
        {
            if (itemIndex_1 < 0 || itemIndex_1 >= inventoryItems.Count ||
                itemIndex_2 < 0 || itemIndex_2 >= inventoryItems.Count)
            {
                Debug.LogWarning($"Invalid indices for SwapItems: {itemIndex_1}, {itemIndex_2}");
                return;
            }

            InventoryItem item1 = inventoryItems[itemIndex_1];
            inventoryItems[itemIndex_1] = inventoryItems[itemIndex_2];
            inventoryItems[itemIndex_2] = item1;

            InformAboutChange();
        }


        private void InformAboutChange()
        {
            OnInventoryUpdated?.Invoke(GetCurrentInventoryState());
        }
    }

    [Serializable]
    public struct InventoryItem
    {
        public int quantity;
        public ItemSO item;
        public bool IsEmpty => item == null;

        public InventoryItem ChangeQuantity(int newQuantity)
        {
            return new InventoryItem
            {
                item = this.item,
                quantity = newQuantity,
            };
        }

        public static InventoryItem GetEmptyItem()
            => new InventoryItem
            {
                item = null,
                quantity = 0,
            };
    }

}
