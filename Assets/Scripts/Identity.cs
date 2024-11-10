using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Searching
{

    public class Identity : MonoBehaviour
    {
        [Header("Identity")]
        public string Name;
        public float positionX;
        public float positionY;

        public OOPMapGenerator mapGenerator;
        public OOPEnemy enemy;

        public void PrintInfo()
        {
            Debug.Log("tell me your " + Name);
        }

        private void Start()
        {
            mapGenerator = FindObjectOfType<OOPMapGenerator>();
            if (mapGenerator == null)
            {
                Debug.LogError("OOPMapGenerator not found in the scene!");
            }
        }

        public virtual void Hit()
        {

        }
    }
}