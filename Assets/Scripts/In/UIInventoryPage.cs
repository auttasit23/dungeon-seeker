using Inventory.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

namespace Inventory.UI
{
    public class UIInventoryPage : MonoBehaviour
    {
        [SerializeField]
        private UIInventoryItem itemPrefab;

        [SerializeField]
        private RectTransform contentPanel;

        [SerializeField]
        private UIInventoryDescription itemDescription;

        [SerializeField]
        private MouseFollower mouseFollower;

        
        List<UIInventoryItem> listOfUIItems = new List<UIInventoryItem>();
        
        private UIInventoryItem currentlySelectedItem = null;

        public event Action<int> OnDescriptionRequested,
                        OnItemActionRequested,
                        OnStartDragging;
        public event Action<int, int> OnSwapItems;

        private int currentlyDraggedItemIndex = -1;
        private void Awake()
        {
            Hide();
            mouseFollower.Toggle(false);
            itemDescription.ResetDescription();
        }
        public void InitializeInventoryUI(int inventorysize)
        {
            for (int i = 0; i < inventorysize; i++)
            {
                UIInventoryItem uiItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
                uiItem.transform.SetParent(contentPanel);
                listOfUIItems.Add(uiItem);

                uiItem.OnItemClicked += HandleItemSelection;
                uiItem.OnItemBeginDrag += HandleBeginDrag;
                uiItem.OnItemDroppedOn += HandleSwap;
                uiItem.OnItemEndDrag += HandleEndDrag;
                uiItem.OnRightMouseBtnClick += HandleShowItemActions;
                uiItem.OnItemDestroyed += HandleItemDestroyed; // เพิ่ม Listener
            }
        }

        public void UpdateData(int itemIndex, ItemSO itemData, int itemQuantity)
        {
            if (listOfUIItems.Count > itemIndex)
            {
                listOfUIItems[itemIndex].SetData(itemData, itemQuantity);  // อัปเดต UI
            }
        }

        private void HandleShowItemActions(UIInventoryItem inventoryItemUI)
        {
            //inventoryItemUI.ShowContextMenu();
        }

        private void HandleEndDrag(UIInventoryItem inventoryItemUI)
        {
            ResetDraggedItem();
        }

        private void HandleSwap(UIInventoryItem inventoryItemUI)
        {
            int index = listOfUIItems.IndexOf(inventoryItemUI);
            if (index == -1)
            {
                Debug.LogWarning("Invalid index in HandleSwap: item not found in list.");
                return;
            }

            OnSwapItems?.Invoke(currentlyDraggedItemIndex, index);
            HandleItemSelection(inventoryItemUI);
        }



        private void ResetDraggedItem()
        {
            mouseFollower.Toggle(false);
            currentlyDraggedItemIndex = -1;
        }
        private void HandleBeginDrag(UIInventoryItem inventoryItemUI)
        {
            int index = listOfUIItems.IndexOf(inventoryItemUI);
            if (index == -1)
            {
                Debug.LogWarning("Invalid index in HandleBeginDrag: item not found in list.");
                return;
            }

            currentlyDraggedItemIndex = index;
            HandleItemSelection(inventoryItemUI);
            OnStartDragging?.Invoke(index);
        }

        
        private void HandleItemDestroyed(UIInventoryItem inventoryItemUI)
        {
            int index = listOfUIItems.IndexOf(inventoryItemUI);
            if (index != -1 && currentlySelectedItem == inventoryItemUI)
            {
                ResetSelection();
            }
        }
        
        public void CreateDraggedItem(Sprite sprite, int quantity)
        {
            mouseFollower.Toggle(true);
            mouseFollower.SetData(sprite, quantity);
        }
        
        private void HandleItemSelection(UIInventoryItem inventoryItemUI)
        {
            if (inventoryItemUI == null || inventoryItemUI.ItemSO == null) return;

            int index = listOfUIItems.IndexOf(inventoryItemUI);
            if (index == -1)
                return;

            currentlySelectedItem = inventoryItemUI;
            OnDescriptionRequested?.Invoke(index);

        }



        public void Show()
        {
            gameObject.SetActive(true);
            itemDescription.ResetDescription();
            ResetDraggedItem();

        }

        public void ResetSelection()
        {
            itemDescription.ResetDescription();
            DeselectAllItems();
            currentlySelectedItem = null;
        }


        private void DeselectAllItems()
        {
            foreach (UIInventoryItem item in listOfUIItems)
            {
                item.Deselect();
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            ResetDraggedItem();
        }

        internal void UpdateDescription(int itemIndex, Sprite itemImage, string name, string description)
        {
            itemDescription.SetDescription(itemImage, name, description, currentlySelectedItem);
            DeselectAllItems();
            listOfUIItems[itemIndex].Select();
        }

        internal void ResetAllItem()
        {
            foreach (var item in listOfUIItems)
            {
                item.ResetData();
                item.Deselect();
            }
        }
    }
}