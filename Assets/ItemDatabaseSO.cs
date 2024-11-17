using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/ItemDatabase")]

public class ItemDatabaseSO : MonoBehaviour
{
    [SerializeField]
    private List<ItemSO> items = new List<ItemSO>();

    public List<ItemSO> Items => items;

    public List<ItemSO> GetItemsByType(Itemtype type)
    {
        return items.FindAll(item => item.itemType == type);
    }
}
