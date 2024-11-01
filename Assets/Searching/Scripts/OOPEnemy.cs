using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Searching
{
    public class OOPEnemy : Character
    {
        public Transform player;
        public float moveInterval = 1.0f;
        private List<Vector3> path;
        private int pathIndex = 0;

        [SerializeField] private List<string> obstacleTags;

        public void Start()
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }

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

        public void MoveToPlayer()
        {
            int directionX = 0;
            int directionY = 0;
            
            if (Mathf.Abs(player.position.x - positionX) > Mathf.Abs(player.position.y - positionY))
            {
                directionX = player.position.x > positionX ? 1 : -1;
            }
            else
            {
                directionY = player.position.y > positionY ? 1 : -1;
            }

            int toX = positionX + directionX;
            int toY = positionY + directionY;
            
            if (IsNearPlayer(toX, toY))
            {
                return;
            }
            
            if (!IsObstacle(toX, toY))
            {
                MoveToPosition(toX, toY);
                return;
            }
            
            List<Vector2Int> alternativeDirections = new List<Vector2Int>
            {
                new Vector2Int(directionX, 0),
                new Vector2Int(0, directionY),
                new Vector2Int(-directionX, 0),
                new Vector2Int(0, -directionY)
            };

            foreach (var dir in alternativeDirections)
            {
                int altX = positionX + dir.x;
                int altY = positionY + dir.y;
                if (!IsObstacle(altX, altY))
                {
                    MoveToPosition(altX, altY);
                    return;
                }
            }
        }
        
        private bool IsNearPlayer(int x, int y)
        {
            Vector2 position = new Vector2(x, y);
            Vector2 playerPosition = new Vector2(player.position.x, player.position.y);

            float distance = Vector2.Distance(position, playerPosition);
            
            float thresholdDistance = 0.5f;
            if (distance <= thresholdDistance)
            {
                return true;
            }

            return false;
        }


        private bool IsObstacle(int x, int y)
        {
            Vector2 position = new Vector2(x, y);
            Collider2D collider = Physics2D.OverlapPoint(position);

            if (collider != null)
            {
                Debug.Log($"Checking obstacle at ({x}, {y}) vs Player at ({player.position.x}, {player.position.y})");
                foreach (string tag in obstacleTags)
                {
                    if (collider.CompareTag(tag))
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        private void MoveToPosition(int x, int y)
        {
            mapGenerator.mapdata[positionX, positionY] = mapGenerator.empty;
            mapGenerator.enemies[positionX, positionY] = null;
            positionX = x;
            positionY = y;
            mapGenerator.mapdata[positionX, positionY] = mapGenerator.enemy;
            mapGenerator.enemies[positionX, positionY] = this;
            transform.position = new Vector3(positionX, positionY, 0);
        }
    }
}