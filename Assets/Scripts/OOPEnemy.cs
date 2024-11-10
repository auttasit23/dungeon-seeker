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
        
        public CameraFollow camera;
        public GameObject floatingPoints;
        private bool isPathGenerated = false;

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
            camera = FindObjectOfType<CameraFollow>();
            if (camera == null)
            {
                Debug.LogError("CameraFollow not found in the scene!");
            }
            if (AStarManager.instance != null)
            {
                GeneratePathToPlayer();
                Node startNode = AStarManager.instance.FindNearestNode(transform.position);
                currentNode = startNode;
            }
            GetRemainEnergy();
            GeneratePathToPlayer();
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
        
        public void Hit(Vector3 targetPosition)
        {
            if (this != null && transform.position == targetPosition)
            {
                camera.ShakeCamera(0.2f, 0.3f);
                int randomValue = Random.Range(0, 100);
                if (randomValue < mapScript.player.hitchance)
                {
                    GameObject points = Instantiate(floatingPoints, targetPosition, Quaternion.identity);
                    points.transform.GetChild(0).GetComponent<TextMesh>().text = mapScript.player.damage.ToString();
                    maxHealth -= mapScript.player.damage;
                    Debug.Log("Enemy Health: " + maxHealth);
                }
                else
                {
                    GameObject points = Instantiate(floatingPoints, targetPosition, Quaternion.identity);
                    TextMesh textMesh = points.transform.GetChild(0).GetComponent<TextMesh>();
                    textMesh.text = "Miss";
                    textMesh.color = Color.green;
                    Debug.Log("Enemy dodge");
                }

                if (mapScript.player.evasion > randomValue)
                {
                    GameObject points = Instantiate(floatingPoints, mapScript.player.transform.position, Quaternion.identity);
                    TextMesh textMesh = points.transform.GetChild(0).GetComponent<TextMesh>();
                    textMesh.text = "Miss";
                    textMesh.color = Color.green;
                    Debug.Log("Enemy Attack Miss");
                }
                else
                {
                    GameObject points = Instantiate(floatingPoints, mapScript.player.transform.position, Quaternion.identity);
                    points.transform.GetChild(0).GetComponent<TextMesh>().text = damage.ToString();
                    this.Attack(mapScript.player);
                }
                
                if (this != null)
                {
                    CheckDead();
                }
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
                Debug.Log("Enemy killed");
                Destroy(gameObject);
            }
        }
        
        public void CreatePath()
        {
            if (path.Count == 0 || path.Count == null)
            {
                isPathGenerated = false;
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
            if (player != null && !isPathGenerated)
            {
                Node startNode = AStarManager.instance.FindNearestNode(transform.position);
                Node endNode = AStarManager.instance.FindNearestNode(player.position);

                path = AStarManager.instance.GeneratePath(startNode, endNode);

                if (path == null || path.Count == 0)
                {
                    Debug.LogError("Failed to generate path to player.");
                    return;
                }

                currentPathIndex = 0;
                isPathGenerated = true;

                CreatePath();
                MoveOneStepTowardsPlayer();
            }
        }


        public void MoveOneStepTowardsPlayer()
        {
            if (currentPathIndex >= path.Count)
            {
                isPathGenerated = false;
                return;
            }
            shouldMove = true;
        }

        private void MoveToPosition(float x, float y)
        {
            Vector2 oldPosition = new Vector2(transform.position.x, transform.position.y);
            Vector2 newPosition = new Vector2(x, y);
            
            if (IsPlayer(newPosition))
            {
                return;
            }
            
            if (HasPlacement(oldPosition))
            {
                if (IsEnemy(oldPosition))
                {
                    if (IsEnemy(newPosition) && oldPosition == newPosition)
                    {
                        path.RemoveAt(0);
                    }
                }
            }
            
            if (HasPlacement(newPosition))
            {
                if (IsEnemy(newPosition) && newPosition != oldPosition)
                {
                    Node startNode = AStarManager.instance.FindNearestNode(transform.position);
                    Node endNode = AStarManager.instance.FindNearestNode(player.position);
        
                    path = AStarManager.instance.GeneratePath(startNode, endNode);
                    StartCoroutine(MoveSmoothly(path[0].transform.position));
                    return;
                }
                if (!IsEnemy(newPosition))
                {
                    Node startNode = AStarManager.instance.FindNearestNode(transform.position);
                    Node endNode = AStarManager.instance.FindNearestNode(player.position);
        
                    path = AStarManager.instance.GeneratePath(startNode, endNode);
                    StartCoroutine(MoveSmoothly(path[0].transform.position));
                    return;
                }
            }
            else
            {
                path.RemoveAt(0);
                StartCoroutine(MoveSmoothly(newPosition));
            }

            /*if (!HasPlacement(oldPosition))
            {
                if (IsEnemy(oldPosition))
                {
                    if (IsEnemy(newPosition))
                    {
                        return; 
                    }
                    else
                    {
                        path.RemoveAt(0);
                        SetNode(newPosition, "enemy");
                        SetNode(transform.position, "empty");
                        StartCoroutine(MoveSmoothly(newPosition));
                    }
                }
            }
            else
            {
                path.RemoveAt(0);
                SetNode(newPosition, "enemy");
                SetNode(transform.position, "empty");
                StartCoroutine(MoveSmoothly(newPosition));
            }*/
            
            
            if (mapScript.enemies.ContainsKey(oldPosition))
            {
                mapScript.enemies[oldPosition].Remove(this);
                if (mapScript.enemies[oldPosition].Count == 0)
                {
                    mapScript.enemies.Remove(oldPosition);
                }
            }
            
            if (!mapScript.enemies.ContainsKey(newPosition))
            {
                mapScript.enemies[newPosition] = new List<OOPEnemy>();
            }
            mapScript.enemies[newPosition].Add(this);
        }
    }
}