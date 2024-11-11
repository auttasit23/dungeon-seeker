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