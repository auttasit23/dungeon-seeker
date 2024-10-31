using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Searching
{

    public class OOPWall : Identity
    {
        public int Damage;
        public bool IsIceWall;

        private void Start()
        {
            IsIceWall = Random.Range(0, 100) < 20 ? true : false;
            if (IsIceWall)
            {
                GetComponent<SpriteRenderer>().color = Color.blue;
            }
        }
        public override void Hit()
        {
            if (IsIceWall)
            {
                mapGenerator.player.TakeDamage(Damage, IsIceWall);
            }
            else
            {
                mapGenerator.player.TakeDamage(Damage);
            }
            mapGenerator.mapdata[positionX, positionY] = 0;
            Destroy(gameObject);
        }
    }
}