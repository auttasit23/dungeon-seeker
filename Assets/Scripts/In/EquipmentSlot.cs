using System;
using UnityEngine;
using UnityEngine.UI;
using Inventory.Model;
using Searching;

namespace Inventory.UI
{
    public class EquipmentSlotManager : MonoBehaviour
    {
        [SerializeField]
        private Image slotImage; // Display the equipped item's sprite
        [SerializeField]
        private Itemtype allowedType; // The type of item this slot accepts
        [SerializeField] ItemSO item;
        [SerializeField] OOPPlayer player;
        private UIInventoryItem currentItem;
        public Action<ItemSO> OnEquipItem; // Event triggered when an item is equipped
        public Action<ItemSO> OnUnequipItem; // Event triggered when an item is unequipped

        public bool IsSlotEmpty => currentItem == null;

        private void Awake()
        {
            ResetSlot();
        }
        private void Start()
        {
            item = GetComponent<ItemSO>();
            player = GetComponent<OOPPlayer>();
        }
        public void ResetSlot()
        {
            slotImage.sprite = null;
            slotImage.enabled = false;
            currentItem = null;
        }

        public bool CanAcceptItem(ItemSO item)
        {
            return item.itemType == allowedType;
        }

        public void EquipItem(UIInventoryItem inventoryItem, Sprite itemSprite)
        {
            if (currentItem != null)
            {
                // Handle swapping or unequipping
                UnequipCurrentItem();
            }

            currentItem = inventoryItem;
            slotImage.sprite = itemSprite;
            slotImage.enabled = true;

            OnEquipItem?.Invoke(inventoryItem.ItemSO);
            Debug.Log($"Equipped {inventoryItem.ItemSO.Name} in {allowedType} slot.");
        }

        public void UnequipCurrentItem()
        {
            if (currentItem != null)
            {
                OnUnequipItem?.Invoke(currentItem.ItemSO);
                ResetSlot();
                Debug.Log($"Unequipped {currentItem.ItemSO.Name} from {allowedType} slot.");
            }
        }

        public void Update()
        {
            if(currentItem != null)
            {
               if(allowedType == Itemtype.Weapon)
                {
                    player.damage += item.itemStat;
                }
               if (allowedType == Itemtype.Armor)
               {
                    player.evasion += item.itemStat;
               }
               if (allowedType == Itemtype.Weapon)
               {
                    player.maxHealth += item.itemStat;
               }
            }
        }
    }
}
