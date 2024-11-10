using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItemDescription : MonoBehaviour
{
    //[SerializeField] private UIInventoryPage inventoryPage;
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;

    public void Awake()
    {
        ResetDescription();
    }
    public void ResetDescription()
    {
        this.itemImage.gameObject.SetActive(false);
        this.title.text = string.Empty;
        this.description.text = string.Empty;
    }

    public void SetDescription(Sprite sprite, string itemName, string itemDescription)
    {
        this.itemImage.gameObject.SetActive(true);
        this.itemImage.sprite = sprite;
        this.title.text = itemName;
        this.description.text = itemDescription;
    }

}
