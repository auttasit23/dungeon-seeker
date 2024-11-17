using Inventory.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

namespace Inventory.UI
{
    public class UIInventoryItem : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDropHandler, IDragHandler
    {
        [SerializeField]
        private Image itemImage;
        [SerializeField]
        private TMP_Text quantityTxt;

        [SerializeField]
        private Image borderImage;

        public event Action<UIInventoryItem> OnItemClicked, OnItemDroppedOn, OnItemBeginDrag, OnItemEndDrag, OnRightMouseBtnClick;


        private bool empty = true;
        private Transform originalParent;

        [SerializeField] private Canvas canvas;

        public ItemSO ItemSO { get; private set; }
        public Action<ItemSO> OnItemDropped;
        [SerializeField]
        private UIInventoryItem inventoryItem; 
        [SerializeField]
        private EquipmentSlotManager equipmentSlot; 

        private void Start()
        {
            if (inventoryItem != null && equipmentSlot != null)
            {
                inventoryItem.OnItemDropped += equipmentSlot.EquipItem;
            }
        }

        public void Awake()
        {
            ResetData();
            Deselect();
        }

        public void SetItem(ItemSO item)
        {
            ItemSO = item;
        }
        public void ResetData()
        {
            itemImage.gameObject.SetActive(false);
            empty = true;
        }
        public void Deselect()
        {
            borderImage.enabled = false;
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

        public void OnPointerClick(PointerEventData pointerData)
        {
            if (pointerData.button == PointerEventData.InputButton.Right)
            {
                OnRightMouseBtnClick?.Invoke(this);
            }
            else
            {
                OnItemClicked?.Invoke(this);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (empty)
                return;
            originalParent = transform.parent;
            transform.SetParent(canvas.transform);
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            OnItemBeginDrag?.Invoke(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.pointerEnter != null && eventData.pointerEnter.TryGetComponent(out EquipmentSlotManager equipmentSlot))
            {
                if (equipmentSlot.CanAcceptItem(ItemSO))
                {
                    equipmentSlot.EquipItem(this); 
                }
                else
                {
                    Debug.Log("Cannot equip item here.");
                }
            }

            OnItemEndDrag?.Invoke(this);
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (ItemSO != null)
            {
                OnItemDropped?.Invoke(ItemSO); 
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position; 
        }
    }
}