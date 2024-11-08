using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

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
            mapScript = FindObjectOfType<OOPMapGenerator>();
            if (mapScript == null)
            {
                Debug.LogError("OOPMapGenerator not found in the scene!");
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
            if (gameObject != null)
            {
                int randomValue = Random.Range(0,100);
                if (randomValue < mapScript.player.hitchance)
                {
                    maxHealth -= mapScript.player.damage;
                    Debug.Log("Enemy Health: " +maxHealth);
                }
                else
                {
                    Debug.Log("Enemy dodge");
                }

                if (mapScript.player.evasion > randomValue)
                {
                    this.Attack(mapScript.player);
                    Debug.Log("Enemy Attack Miss");
                }
                CheckDead();
            }
        }

        public void Attack(OOPPlayer _player)
        {
            damage = 5;
            _player.TakeDamage(damage);
        }

        protected override void CheckDead()
        {
            if (maxHealth <= 0)
            {
                Debug.Log("Enemy kill");
                SetNode(transform.position, "empty");
                Destroy(gameObject);
            }
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
                MoveToPosition(nextNode.transform.position.x, nextNode.transform.position.y);

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

        private void MoveToPosition(float x, float y)
        {
            Vector3 nextPo = new Vector3(x, y);
            if (!HasPlacement(nextPo))
            {
                if (IsEnemy(nextPo))
                {
                    path.RemoveAt(0);
                    Vector3 targetPosition = new Vector3(x, y, 0);
                    SetNode(targetPosition, "enemy");
                    SetNode(transform.position, "empty");
                    StartCoroutine(MoveSmoothly(targetPosition));
                }
                else
                {
                    return;
                }
            }
            else
            {
                path.RemoveAt(0);
                Vector3 targetPosition = new Vector3(x, y, 0);
                SetNode(targetPosition, "enemy");
                SetNode(transform.position, "empty");
                StartCoroutine(MoveSmoothly(targetPosition));
            }
        }
    }
}