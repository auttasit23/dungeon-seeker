using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Searching
{

    public class OOPFireStormItem : Identity
    {
        public override void Hit()
        {
            mapGenerator.player.inventory.AddItem("FireStorm");
            mapGenerator.fireStorms[positionX, positionY] = null;
            mapGenerator.mapdata[positionX, positionY] = mapGenerator.empty;
            Destroy(gameObject);
        }
    }

}