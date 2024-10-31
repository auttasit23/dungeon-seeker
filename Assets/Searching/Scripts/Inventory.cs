using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Searching
{
    public class Inventory : MonoBehaviour
    {
        // สร้าง Dictionary เพื่อเก็บข้อมูลของไอเทมในคลังของผู้เล่น
        private Dictionary<string, int> inventory = new Dictionary<string, int>();

        // ฟังก์ชันสำหรับเพิ่มไอเทม
        public void AddItem(string itemName)
        {
            if (inventory.ContainsKey(itemName))
            {
                inventory[itemName] += 1;  // เพิ่มจำนวนหากไอเทมมีอยู่แล้ว
            }
            else
            {
                inventory.Add(itemName, 1);  // เพิ่มไอเทมใหม่ถ้ายังไม่มี
            }

            Debug.Log($"add item {itemName} => total: {inventory[itemName]}");
        }

        // [In-class Assignment] addItem ที่รับค่า amount และเพิ่มจำนวนไอเทมในคลังตามจำนวนที่ระบุ
        public void AddItem(string itemName, int amount)
        {
            // ...
        }

        // ฟังก์ชันสำหรับลบไอเทม
        public void UseItem(string itemName)
        {
            if (inventory.ContainsKey(itemName))
            {
                int remaining = inventory[itemName] - 1;
                if (remaining <= 0)
                {
                    inventory.Remove(itemName);  // ถ้าจำนวนเป็น 0 หรือ น้อยกว่า ให้ลบออกจากคลัง
                }

                Debug.Log($"remove {itemName} remaining: {remaining}");
            }
            else
            {
                Debug.LogWarning($"no item {itemName}");
            }
        }

        // [In-class Assignment] UseItem ที่รับค่า amount และลดจำนวนไอเทมในคลังตามจำนวนที่ระบุ
        public void UseItem(string itemName, int amount)
        {
            // ...
        }

        // [In-class Assignment] ฟังก์ชันสำหรับตรวจสอบจำนวนไอเทมในคลัง
        public int numberOfItem(string itemName)
        {
            if (inventory.ContainsKey(itemName))
            {
                return inventory[itemName];
            }
            else
            {
                return 0;
            }
        }

        // ฟังก์ชันสำหรับแสดงจำนวนไอเทมในคลัง
        public void ShowInventory()
        {
            Debug.Log("Inventory ...");
            foreach (KeyValuePair<string, int> item in inventory)
            {
                Debug.Log($"{item.Key}: {item.Value}");
            }
        }
    }
}