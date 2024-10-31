using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Searching
{
    public class SyntaxHashTaable : MonoBehaviour
    {
        public void Start()
        {
            // 1. สร้าง Hashtable
            Hashtable hashtable = new Hashtable();

            // 2. การเพิ่มข้อมูลลงใน Hashtable
            hashtable.Add(1, "Apple");
            hashtable.Add(2, "Banana");
            hashtable.Add("bad-fruit", "Rotten Tomato");

            // 3. เข้าถึงข้อมูลใน Hashtable ด้วย Key
            string fruit1 = (string)hashtable[1];
            string fruit2 = (string)hashtable[2];
            string badFruit = (string)hashtable["bad-fruit"];
            Debug.Log($"fruit1: {fruit1}");
            Debug.Log($"fruit2: {fruit2}");
            Debug.Log($"badFruit: {badFruit}");

            // 4. แสดงข้อมูลใน Hashtable
            PrintHashTable(hashtable);

            // 5. ตรวจสอบการมีอยู่ของ Key
            int key = 2;
            if (hashtable.ContainsKey(key))
            {
                Debug.Log($"found {key}");
            }
            else
            {
                Debug.Log($"not found {key}");
            }

            // 6. ลบข้อมูลออกจาก Hashtable
            int keyToRemove = 1;
            hashtable.Remove(keyToRemove);
            PrintHashTable(hashtable);
        }

        // 4. แสดงเนื้อหาใน LinkedList
        public void PrintHashTable(Hashtable hashtable)
        {
            Debug.Log("table ...");
            foreach (DictionaryEntry entry in hashtable)
            {
                Debug.Log($"Key: {entry.Key}, Value: {entry.Value}");
            }
        }
    }
}