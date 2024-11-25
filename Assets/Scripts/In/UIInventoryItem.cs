using Inventory.Model;
using Searching;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory.UI
{
    public class UIInventoryItem : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDropHandler, IDragHandler
    {
        private OOPPlayer player;
        [SerializeField]
        private Image itemImage;
        [SerializeField]
        private TMP_Text quantityTxt;

        [SerializeField]
        private Image borderImage;
        [SerializeField]
        private TMP_Text equipUI;
        [SerializeField]
        private GameObject contextMenuPrefab; // Prefab for the right-click context menu
        private GameObject contextMenuInstance;

        public event Action<UIInventoryItem> OnItemClicked, OnItemDroppedOn, OnItemBeginDrag, OnItemEndDrag, OnRightMouseBtnClick, OnItemDestroyed;

        private bool empty = true;
        private bool isEquipped = false;
        public ItemSO ItemSO { get; private set; }
        public EquipmentSlotManager equipment;

        public void Awake()
        {
            ResetData();
            Deselect();
        }

        private void Start()
        {
            equipment = FindAnyObjectByType<EquipmentSlotManager>();
            player = FindObjectOfType<OOPPlayer>();
        }

        public void ResetData()
        {
            itemImage.gameObject.SetActive(false);
            empty = true;
            ItemSO = null;
            isEquipped = false;
            equipUI.gameObject.SetActive(false);
        }
        

        public void SetData(ItemSO item, int quantity)
        {
            if (item == null)
            {
                ResetData();
                return;
            }

            ItemSO = item; // Assign the item
            itemImage.gameObject.SetActive(true);
            itemImage.sprite = item.ItemImage; // Use the item's sprite
            quantityTxt.text = quantity.ToString();
            empty = false;
        }

        public void Select()
        {
            borderImage.enabled = true;
        }
        public void Deselect()
        {
            borderImage.enabled = false;
        }

        public void OnPointerClick(PointerEventData pointerData)
        {
            if (pointerData.button == PointerEventData.InputButton.Right)
            {
                // กรณีคลิกขวา
                OnRightMouseBtnClick?.Invoke(this);
            }
            else if (!empty)
            {
                // กรณีคลิกซ้ายบนไอเท็ม
                OnItemClicked?.Invoke(this);
            }
            else
            {
                // กรณีคลิกช่องว่าง
                var inventoryPage = GetComponentInParent<UIInventoryPage>();
                inventoryPage?.ResetSelection(); // รีเซ็ตการเลือก
            }
        }



        /*public void ShowContextMenu(PointerEventData pointerData)
        {
            if (contextMenuInstance != null)
                Destroy(contextMenuInstance);

            // Instantiate context menu at the item's position
            contextMenuInstance = Instantiate(contextMenuPrefab, pointerData.position, Quaternion.identity, transform.parent);

            // Optionally, adjust the position so it doesn't overlap with the item
            Vector3 menuPosition = transform.position;
            contextMenuInstance.transform.position = new Vector3(menuPosition.x + 50, menuPosition.y, menuPosition.z); // Adjust as needed

            // Link button actions
            var buttons = contextMenuInstance.GetComponentsInChildren<Button>();
            buttons[0].onClick.AddListener(() => ToggleEquip());
            buttons[1].onClick.AddListener(() => DestroyItem());
        }*/

        public void ToggleEquip()
        {
            if (ItemSO == null)
            {
                Debug.LogWarning("Cannot equip item: ItemSO is null.");
                return;
            }

            int slotIndex = GetSlotIndex(ItemSO);

            // Ensure the slot index is valid and within the 3-slot limit
            if (slotIndex < 0 || slotIndex >= equipment.equipmentSlots.Length)
            {
                Debug.LogWarning("Invalid slot index. Cannot toggle equip.");
                return;
            }

            // Check if the item is already equipped in the same slot
            if (isEquipped && equipment.equipmentSlots[slotIndex] == ItemSO)
            {
                UnequipItemFromSlot(slotIndex); // Unequip the item
                Debug.Log($"{ItemSO.itemName} unequipped from slot {slotIndex}.");
                CloseContextMenu();
                return;
            }

            // Check if the slot is occupied by another item
            if (equipment.equipmentSlots[slotIndex] != null)
            {
                Debug.LogWarning($"Cannot equip {ItemSO.itemName}: Slot {slotIndex} is already occupied by {equipment.equipmentSlots[slotIndex].itemName}.");
                return;
            }

            // Equip the new item
            EquipItemToSlot(slotIndex);
            Debug.Log($"{ItemSO.itemName} equipped in slot {slotIndex}.");
            CloseContextMenu();
        }



        private void EquipItemToSlot(int slotIndex)
        {
            equipment.equipmentSlots[slotIndex] = ItemSO;
            ApplyItemStats(ItemSO, true);
            isEquipped = true;
            if (equipUI != null)
            {
                equipUI.gameObject.SetActive(true);
                equipUI.text = "E";
            }
        }


        private void UnequipItemFromSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= equipment.equipmentSlots.Length)
            {
                Debug.LogWarning("Invalid slot index. Cannot unequip item.");
                return;
            }

            ItemSO itemToUnequip = equipment.equipmentSlots[slotIndex];
            if (itemToUnequip != null)
            {
                ApplyItemStats(itemToUnequip, false);
                equipment.equipmentSlots[slotIndex] = null;
                Debug.Log($"{itemToUnequip.itemName} unequipped from slot {slotIndex}");

                isEquipped = false;
                
                if (equipUI != null)
                {
                    equipUI.gameObject.SetActive(false);
                }
            }
        }


        private void ApplyItemStats(ItemSO item, bool isAdding)
        {
            int modifier = isAdding ? 1 : -1;

            switch (item.itemType)
            {
                case Itemtype.Weapon:
                    player.damage += item.itemStat * modifier;
                    break;
                case Itemtype.Armor:
                    player.maxHealth += item.itemStat * modifier;
                    break;
                case Itemtype.Accessory:
                    player.evasion += item.itemStat * modifier;
                    break;
                default:
                    Debug.LogWarning("Unknown item type for stats application.");
                    break;
            }
        }

        private int GetSlotIndex(ItemSO item)
        {
            if (item == null)
            {
                Debug.LogWarning("Cannot get slot index: ItemSO is null.");
                return -1;
            }

            switch (item.itemType)
            {
                case Itemtype.Weapon:
                    return 0;
                case Itemtype.Armor:
                    return 1;
                case Itemtype.Accessory:
                    return 2;
                default:
                    Debug.LogWarning("Invalid item type for equipping.");
                    return -1;
            }
        }

        public void DestroyItem()
        {
            if (empty)
            {
                Debug.LogWarning("Cannot destroy an empty slot.");
                return;
            }

            Debug.Log($"{ItemSO?.itemName ?? "Unknown item"} destroyed");
    
            // อัปเดตข้อมูลใน Inventory Data
            FindObjectOfType<InventoryController>().RemoveItemFromInventory(this.ItemSO);

            ResetData();
            CloseContextMenu();

            OnItemDestroyed?.Invoke(this);
        }



        private void CloseContextMenu()
        {
            if (contextMenuInstance != null)
                Destroy(contextMenuInstance);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (empty)
                return;
            OnItemBeginDrag?.Invoke(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnItemEndDrag?.Invoke(this);
        }

        public void OnDrop(PointerEventData eventData)
        {
            OnItemDroppedOn?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData)
        {

        }
    }
}
