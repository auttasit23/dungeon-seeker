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
        private Image slotImage; 
        [SerializeField]
        private Itemtype allowedType; 

        private UIInventoryItem currentItem;

        [SerializeField]
        private OOPPlayer player; 
        private ItemSO equippedItem; 

        public Action<ItemSO> OnEquipItem; 
        public Action<ItemSO> OnUnequipItem; 

        public bool IsSlotEmpty => equippedItem == null;

        private void Awake()
        {
            ResetSlot();
        }

        private void Start()
        {
            if (player == null)
            {
                player = FindObjectOfType<OOPPlayer>();
                if (player == null)
                {
                    Debug.LogError("OOPPlayer is not assigned or found in the scene!");
                }
            }
        }

        public void ResetSlot()
        {
            slotImage.sprite = null;
            slotImage.enabled = false;
            equippedItem = null;
            UpdatePlayerStats(null); 
        }

        public bool CanAcceptItem(ItemSO item)
        {
            return item != null && item.itemType == allowedType;
        }

        public void EquipItem(ItemSO item)
        {
            if (item == null || !CanAcceptItem(item))
            {
                Debug.LogWarning($"Cannot equip item of type {item?.itemType}. Allowed type: {allowedType}");
                return;
            }

            if (equippedItem != null)
            {
                UnequipCurrentItem();
            }

            equippedItem = item;
            slotImage.sprite = item.ItemImage;
            slotImage.enabled = true;

            UpdatePlayerStats(equippedItem);
            OnEquipItem?.Invoke(equippedItem);

            Debug.Log($"Equipped {equippedItem.Name} in {allowedType} slot.");
        }

        public void UnequipCurrentItem()
        {
            if (equippedItem != null)
            {
                OnUnequipItem?.Invoke(equippedItem);
                UpdatePlayerStats(null); 
                ResetSlot();

                Debug.Log($"Unequipped {equippedItem.Name} from {allowedType} slot.");
            }
        }

        private void UpdatePlayerStats(ItemSO item)
        {
            if (player == null)
                return;

            if (allowedType == Itemtype.Weapon)
                player.damage = (equippedItem?.itemStat ?? 0) + (item?.itemStat ?? 0);
            else if (allowedType == Itemtype.Armor)
                player.evasion = (equippedItem?.itemStat ?? 0) + (item?.itemStat ?? 0);
            else if (allowedType == Itemtype.Accessory)
                player.maxHealth = (equippedItem?.itemStat ?? 0) + (item?.itemStat ?? 0);
        }
    }
    }

