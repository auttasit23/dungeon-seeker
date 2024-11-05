using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Searching
{
    public class SyntaxDictionary : MonoBehaviour
    {
        public void Start()
        {
            // 1. สร้าง Dictionary โดยระบุชนิดของ Key และ Value
            // กำหนดให้ Key เป็น int และ Value เป็น string
            Dictionary<int, string> dictionary = new Dictionary<int, string>();

            // 2. การเพิ่มข้อมูลลงใน Dictionary
            dictionary.Add(1, "Apple");
            dictionary.Add(2, "Banana");
            dictionary[3] = "Cherry";

            // 3. แสดงข้อมูลใน Dictionary
            PrintDictionary(dictionary);

            // 4. การตรวจสอบการมีอยู่ของคีย์ และการดึงค่าออกมาโดยใช้
            int keyToCheck = 1;
            bool hasKey = dictionary.ContainsKey(keyToCheck);
            Debug.Log($"has key {keyToCheck} : {hasKey}");
            if (hasKey)
            {
                string value = dictionary[keyToCheck];
                Debug.Log($"value of key {keyToCheck} : {value}");
            }

            // 5. การดึง key ออกมาทั้งหมด
            Debug.Log("All keys in dictionary:");
            foreach (int key in dictionary.Keys)
            {
                Debug.Log(key);
            }

            // 6. ลบข้อมูลออกจาก Dictionary
            int keyToRemove = 1;
            dictionary.Remove(keyToRemove);
            PrintDictionary(dictionary);

            // 7. clear ข้อมูลใน Dictionary
            dictionary.Clear();
        }

        // 3. แสดงข้อมูลใน Dictionary
        public void PrintDictionary(Dictionary<int, string> dictionary)
        {
            Debug.Log($"Dictionary has {dictionary.Count} keys");
            foreach (KeyValuePair<int, string> entry in dictionary)
            {
                Debug.Log($"Key: {entry.Key}, Value: {entry.Value}");
            }
        }
    }
}