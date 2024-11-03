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

        public void PrintInfo()
        {
            Debug.Log("tell me your " + Name);
        }

        public virtual void Hit()
        {

        }
    }
}