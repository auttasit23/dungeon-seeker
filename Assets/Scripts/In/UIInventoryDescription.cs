using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.UI
{
    public class UIInventoryDescription : MonoBehaviour
    {
        [SerializeField]
        private Image itemImage;
        [SerializeField]
        private TMP_Text title;
        [SerializeField]
        private TMP_Text description;
        [SerializeField]
        private GameObject equipbutton;
        [SerializeField]
        private GameObject destroybutton;


        public void Awake()
        {
            ResetDescription();
        }

        public void ResetDescription()
        {
            itemImage.gameObject.SetActive(false);
            title.text = "";
            description.text = "";
            equipbutton.SetActive(false);
            destroybutton.SetActive(false);
        }

        public void SetDescription(Sprite sprite, string itemName,
            string itemDescription)
        {
            itemImage.gameObject.SetActive(true);
            itemImage.sprite = sprite;
            title.text = itemName;
            description.text = itemDescription;
            equipbutton.SetActive(true);
            destroybutton.SetActive(true);
        }
    }
}