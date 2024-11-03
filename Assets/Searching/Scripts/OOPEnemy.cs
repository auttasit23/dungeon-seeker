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

        public Node currentNode;
        public List<Node> path = new List<Node>();
        
        private int currentPathIndex = 0;
        private bool shouldMove = false;

        public void Start()
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            GetRemainEnergy();
            GeneratePathToPlayer();
            
            Node startNode = AStarManager.instance.FindNearestNode(transform.position);
            currentNode = startNode;
        }

        private void Update()
        {
            MoveAlongPath();

            if (Input.GetKeyDown(KeyCode.R))
            {
                path.Clear();
                GeneratePathToPlayer();
            }
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
            /*if (energy <= 0)
            {
                mapGenerator.enemies[positionX, positionY] = null;
                mapGenerator.mapdata[positionX, positionY] = mapGenerator.empty;
            }*/
        }
        public void CreatePath()
        {
            if (path.Count == 0 || path.Count == null)
            {
                GeneratePathToPlayer();
            }
            
            if (path.Count > 0 || path == null)
            {
                Node nextNode = path[0];
                currentNode = nextNode;
                path.RemoveAt(0);
                    
                MoveToPosition((int)nextNode.transform.position.x, (int)nextNode.transform.position.y);
            }
        }
        
        private void MoveAlongPath()
        {
            if (!shouldMove || path == null || currentPathIndex >= path.Count)
                return;

            Node nextNode = path[currentPathIndex];

            if (Vector2.Distance(transform.position, nextNode.transform.position) < 0.1f)
            {
                currentPathIndex++;
                shouldMove = false;
            }
        }


        private void GeneratePathToPlayer()
        {
            Node startNode = AStarManager.instance.FindNearestNode(transform.position);
            Node endNode = AStarManager.instance.FindNearestNode(player.position);
            path = AStarManager.instance.GeneratePath(startNode, endNode);
            currentPathIndex = 0;

            CreatePath();
            MoveOneStepTowardsPlayer();
        }

        
        public void MoveOneStepTowardsPlayer()
        {
            if (currentPathIndex >= path.Count)
            {
                GeneratePathToPlayer();
            }
            shouldMove = true;
        }

        private void MoveToPosition(int x, int y)
        {
            /*if (!HasPlacement(x, y))
            {
                mapGenerator.mapdata[positionX, positionY] = mapGenerator.empty;
                mapGenerator.enemies[positionX, positionY] = null;
                positionX = x;
                positionY = y;
                mapGenerator.mapdata[positionX, positionY] = mapGenerator.enemy;
                mapGenerator.enemies[positionX, positionY] = this;
                
                Vector3 targetPosition = new Vector3(positionX, positionY, 0);
                StartCoroutine(MoveSmoothly(targetPosition));
            }*/
            positionX = x;
            positionY = y;
            Vector3 targetPosition = new Vector3(positionX, positionY, 0);
            StartCoroutine(MoveSmoothly(targetPosition));
        }
    }
}