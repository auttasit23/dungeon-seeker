using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynTaxLinkedList : MonoBehaviour
{

    public void Start()
    {
        // 1. สร้าง LinkedList ของประเภท string
        LinkedList<string> linkedList = new LinkedList<string>();

        // 2. เพิ่มข้อมูลที่ท้ายของ LinkedList
        linkedList.AddLast("Node 1");
        linkedList.AddLast("Node 2");

        // 3. เพิ่มข้อมูลที่ต้นของ LinkedList
        linkedList.AddFirst("Node 0");

        // 4. แสดงเนื้อหาใน LinkedList
        PrintLinkedList(linkedList);

        // 5. เช้าถึงข้อมูลใน LinkedList
        LinkedListNode<string> firstNode = linkedList.First;
        Debug.Log($"first:{firstNode.Value}");
        LinkedListNode<string> lastNode = linkedList.Last;
        Debug.Log("last: {lastNode.Value}");
        LinkedListNode<string> node1 = linkedList.Find("Node 1");
        Debug.Log(node1.Previous.Value);
        Debug.Log(node1.Next.Value);
        if (firstNode.Previous == null)
        {
            Debug.Log("firstNode.Previous is null");
        }
        if (lastNode.Next == null)
        {
            Debug.Log("lastNode.Next is null");
        }

        // 6. add node ก่อน หรือ หลัง node ที่กำหนด
        linkedList.AddAfter(node1, "Node 1.5");
        linkedList.AddBefore(node1, "Node 0.5");
        PrintLinkedList(linkedList);

        // 6. ลบ Node แรก
        linkedList.RemoveFirst();
        PrintLinkedList(linkedList);
   
        // 7. ลบ Node ตามค่าที่กำหนด
        linkedList.Remove("Node 2");
        PrintLinkedList(linkedList);
    }

    // 4. แสดงเนื้อหาใน LinkedList
    public void PrintLinkedList(LinkedList<string> linkedList)
    {
        Debug.Log("LinkedList ...");
        foreach (var node in linkedList)
        {
            Debug.Log(node);
        }
    }
}
