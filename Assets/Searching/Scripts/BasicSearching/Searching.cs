using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Searching
{
    public class Searching : MonoBehaviour
    {
        [SerializeField]
        private int[] smallArray;

        private int[] mediumArray;
        private int[] largeArray;

        private int[,] small2DArray;

        public int target;

        void Awake()
        {
            smallArray = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            Debug.Log("created small array ...done");

            mediumArray = new int[1000];
            for (int i = 0; i < mediumArray.Length; i++)
            {
                mediumArray[i] = UnityEngine.Random.Range(0, 1000);
            }
            Debug.Log("created medium array ...done");

            largeArray = new int[1000000];
            for (int i = 0; i < largeArray.Length; i++)
            {
                largeArray[i] = UnityEngine.Random.Range(0, 1000000);
            }
            Debug.Log("created large array ...done");
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                int result = Search1DArray(largeArray, target);
                Debug.Log(result);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                var (i, j) = Search2DArray(small2DArray, target);
                Debug.Log(i + " " + j);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Array.Sort(largeArray);
                int result = BinarySearch(largeArray, target);
                Debug.Log(result);
            }
        }

        public int Search1DArray(int[] array, int target)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == target)
                {
                    return i;
                }
            }
            return -1;
        }

        public (int, int) Search2DArray(int[,] array, int target)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    if (array[i, j] == target)
                    {
                        return (i, j);
                    }
                }
            }
            return (-1, -1);
        }

        public int BinarySearch(int[] array, int target)
        {
            int left = 0;
            int right = array.Length - 1;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;

                if (array[mid] == target)
                {
                    return mid;
                }

                if (array[mid] < target)
                {
                    left = mid + 1;
                }
                else
                {
                    right = mid - 1;
                }
            }

            return -1;
        }
    }
}