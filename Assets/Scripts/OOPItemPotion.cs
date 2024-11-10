using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Searching
{

    public class OOPItemPotion : Identity
    {
        

        public override void Hit()
        {
            mapGenerator.player.Heal(25);
            Destroy(gameObject);
        }
    }
}