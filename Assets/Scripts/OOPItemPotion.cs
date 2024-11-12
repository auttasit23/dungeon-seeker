using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Searching
{

    public class OOPItemPotion : Identity
    {
        private void Update()
        {
            if (mapGenerator.potion.ContainsKey(transform.position))
            {
                mapGenerator.potion[transform.position].Remove(this);
                if (mapGenerator.potion[transform.position].Count == 0)
                {
                    mapGenerator.potion.Remove(transform.position);
                }
            }
            
            if (!mapGenerator.potion.ContainsKey(transform.position))
            {
                mapGenerator.potion[transform.position] = new List<OOPItemPotion>();
            }
            mapGenerator.potion[transform.position].Add(this);
        }

        public override void Hit()
        {
            if (this != null)
            {
                mapGenerator.player.Heal(25);
                Destroy(gameObject);
            }
        }
    }
}