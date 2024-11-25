using System;
using UnityEngine;
using UnityEngine.UI;
using Inventory.Model;
using Searching;
using TMPro;

namespace Inventory.UI
{
    public class EquipmentSlotManager : MonoBehaviour
    {
        [SerializeField] public ItemSO[] equipmentSlots = new ItemSO[3];

        public void UpdateEtext()
        {
            var inventoryItems = FindObjectsOfType<UIInventoryItem>();
            foreach (var item in inventoryItems)
            {
                if (item.equipUI != null)
                {
                    bool isEquipped = item.ItemSO != null && Array.Exists(equipmentSlots, slot => slot == item.ItemSO);
                    
                    item.equipUI.gameObject.SetActive(isEquipped);
                    if (isEquipped)
                    {
                        item.equipUI.text = "E";
                    }
                }
            }
        }
    }
}