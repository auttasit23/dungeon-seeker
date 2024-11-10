using System;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
public class UIInventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] public Image itemImage;
    [SerializeField] public TMP_Text quantityText;
    [SerializeField] private Image borderImage;

    public event Action<UIInventoryItem> OnItemClicked, OnItemDropped, OnItemRightClicked;
    public bool isEmpty = true;
    private Transform originalParent;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        ResetData();
        Deselect();
    }

    public void ResetData()
    {
        itemImage.gameObject.SetActive(false);
        quantityText.text = "";
        isEmpty = true;
    }

    public void SetData(Sprite sprite, int quantity)
    {
        itemImage.sprite = sprite;
        itemImage.gameObject.SetActive(true);
        quantityText.text = quantity.ToString();
        isEmpty = false;
    }
    public void Select()
    {
        borderImage.enabled = true;
    }
    public void Deselect()
    {
        borderImage.enabled = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isEmpty) return;

        originalParent = transform.parent;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isEmpty)
            transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // Check if the dragged item was dropped on another UIInventoryItem slot
        if (eventData.pointerEnter != null && eventData.pointerEnter.TryGetComponent(out UIInventoryItem targetSlot) && targetSlot != this)
        {
            if (!targetSlot.isEmpty || targetSlot is EquipmentSlot) return;

            // Move item data to the target slot
            targetSlot.SetData(itemImage.sprite, int.Parse(quantityText.text));

            // Clear data in original slot
            ResetData();

            OnItemDropped?.Invoke(targetSlot);  // Trigger item drop event
        }

        // Return to original position if not dropped on a valid target slot
        transform.localPosition = Vector3.zero;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && !isEmpty)
        {
            OnItemRightClicked?.Invoke(this); // Trigger right-click action
        }
    }
}
