using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Searching
{
    public class OOPEnemy : Character
    {
        public Transform player;
        public GameObject playerObject;
        public float moveInterval = 1.0f;

        public Node currentNode;
        public List<Node> path = new List<Node>();

        public void Start()
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            GetRemainEnergy();
            GeneratePathToPlayer();
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
        public void CreatePath()
        {
            if (path.Count == 0)
            {
                GeneratePathToPlayer();
            }

            if (path.Count > 0)
            {
                int x = 0;
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(path[x].transform.position.x, path[x].transform.position.y, -2), 1f);

                if (Vector2.Distance(transform.position, path[x].transform.position) < 0.1f)
                {
                    currentNode = path[x];
                    path.RemoveAt(x);
                }
            }
        }
        public void GeneratePathToPlayer()
        {
            Node targetNode = AStarManager.instance.FindNearestNode(playerObject.transform.position);
            currentNode = targetNode;
            path = AStarManager.instance.GeneratePath(currentNode, targetNode);
        }
    }
}