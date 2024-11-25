using System;
using UnityEngine;
using UnityEngine.UI;
using Inventory.Model;
using Searching;

namespace Inventory.UI
{
    public class EquipmentSlotManager : MonoBehaviour
    {
        [SerializeField] public ItemSO[] equipmentSlots = new ItemSO[3];
    }
}