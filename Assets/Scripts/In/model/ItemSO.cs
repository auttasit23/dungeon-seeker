using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class ItemSO : ScriptableObject
    {
        [field: SerializeField]
        public bool IsStackable { get; set; }

        public int ID => GetInstanceID();

        [field: SerializeField]
        public int MaxStackSize { get; set; } = 1;

        [field: SerializeField]
        public string itemName { get; set; }

        [field: SerializeField]
        [field: TextArea]
        public string Description { get; set; }

        [field: SerializeField]
        public Sprite ItemImage { get; set; }
        
        [field: SerializeField]
        public Itemtype itemType { get; set; }

        [field: SerializeField]
        public int itemStat {get; set; }
    }

   
}
 public enum Itemtype
    {
        Potion,
        Weapon,
        Armor,
        Accessory
    }