using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
                audioManager.PlaySFX(audioManager.victory);
                mapGenerator.player.FullHeal();
                if (GameManager.level >= 3)
                {
                    Debug.Log("Exit unlocked");
                    mapGenerator.player.enabled = false;
                    YouWin.SetActive(true);
                    Debug.Log("You win");
                }
                else
                {
                    if (mapGenerator.enemyRoomCount <= 2)
                    {
                        mapGenerator.enemyRoomCount += 1;
                    }
                    GameManager.level += 1;
                    mapGenerator.player.inventory.UseItem(unlockKey);
                    StartCoroutine(mapGenerator.ResetMap());
                    Debug.Log("Level " + GameManager.level);
                }
            }
            else
            {
                Debug.Log($"Exit locked, require key: {unlockKey}");
            }
        }
    }
}