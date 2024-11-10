using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private UIInventoryPage inventoryUI;

    [SerializeField] private Sprite appleSprite;
    [SerializeField] private Sprite potionSprite;

    private void Start()
    {
        // Initialize the inventory UI with 10 slots (for example)
        inventoryUI.InitializeInventory(10);

        // Add test items to the inventory
        AddTestItems();
    }

    private void Update()
    {
        // Toggle inventory UI on 'I' key press
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!inventoryUI.isActiveAndEnabled)
            {
                inventoryUI.Show();
            }
            else
            {
                inventoryUI.Hide();
            }
        }
    }

    private void AddTestItems()
    {
        // Add apple to the first slot
        inventoryUI.AddItemToInventory(appleSprite, 5); // Adds 5 apples

        // Add potion to the second slot
        inventoryUI.AddItemToInventory(potionSprite, 3); // Adds 3 potions
    }
}
