using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Searching
{

    public class OOPExit : Identity
    {
        public string unlockKey;
        public GameObject YouWin;

        public override void Hit()
        {
            if (mapGenerator.player.inventory.numberOfItem(unlockKey) > 0)
            {
                Debug.Log("Exit unlocked");
                mapGenerator.player.enabled = false;
                YouWin.SetActive(true);
                Debug.Log("You win");
            }
            else
            {
                Debug.Log($"Exit locked, require key: {unlockKey}");
            }
        }
    }
}