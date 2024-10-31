using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Searching
{

    public class OOPEnemy : Character
    {
        public void Start()
        {
            GetRemainEnergy();
        }

        public override void Hit()
        {
            mapGenerator.player.Attack(this);
            this.Attack(mapGenerator.player);
        }

        public void Attack(OOPPlayer _player)
        {
            _player.TakeDamage(AttackPoint);
        }

        protected override void CheckDead()
        {
            base.CheckDead();
            if (energy <= 0)
            {
                mapGenerator.enemies[positionX, positionY] = null;
                mapGenerator.mapdata[positionX, positionY] = mapGenerator.empty;
            }
        }

        public void RandomMove()
        {
            int toX = positionX;
            int toY = positionY;
            int random = Random.Range(0, 4);
            switch (random)
            {
                case 0:
                    // up
                    toY += 1;
                    break;
                case 1:
                    // down 
                    toY -= 1;
                    break;
                case 2:
                    // left
                    toX -= 1;
                    break;
                case 3:
                    // right
                    toX += 1;
                    break;
            }
            if (!HasPlacement(toX, toY))
            {
                mapGenerator.mapdata[positionX, positionY] = mapGenerator.empty;
                mapGenerator.enemies[positionX, positionY] = null;
                positionX = toX;
                positionY = toY;
                mapGenerator.mapdata[positionX, positionY] = mapGenerator.enemy;
                mapGenerator.enemies[positionX, positionY] = this;
                transform.position = new Vector3(positionX, positionY, 0);
            }
        }
    }
}