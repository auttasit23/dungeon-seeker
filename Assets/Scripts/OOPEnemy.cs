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
        private bool isPathGenerated = false;
        private bool isFacingRight = true;
        
        public enum EnemyState
        {
            Idle,
            Walk,
            Attack,
        }
        private EnemyState currentState = EnemyState.Idle;

        
        IEnumerator StartAfterSceneLoad()
        {
            yield return new WaitForSeconds(0.1f);
            CreatePath();
        }

        void OnEnable()
        {
            StartCoroutine(StartAfterSceneLoad());
        }

        public void Start()
        {
            damage = damage * Mathf.Pow(1.5f, GameManager.level - 1);
            health = health * Mathf.Pow(1.5f, GameManager.level - 1);
            animator = gameObject.GetComponent<Animator>();
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
        }
        

        private void Update()
        {
            MoveAlongPath();
            EnemyUpdateState();
            if (player != null)
            {
                FlipTowardsPlayer();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                path.Clear();
                GeneratePathToPlayer();
            }
        }
        private void FlipTowardsPlayer()
        {
            if (player.position.x > transform.position.x && !isFacingRight)
            {
                FlipCharacter();
            }
            else if (player.position.x < transform.position.x && isFacingRight)
            {
                FlipCharacter();
            }
        }

        private void FlipCharacter()
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
        
        public void EnemyUpdateState()
        {
            if (currentState == EnemyState.Idle)
            {
                animator.SetBool("idle", true);
                animator.SetBool("walk", false);
                animator.SetBool("attack", false);
            }
            if (currentState == EnemyState.Walk)
            {
                animator.SetBool("idle", false);
                animator.SetBool("walk", true);
                animator.SetBool("attack", false);
            }
            if (currentState == EnemyState.Attack)
            {
                animator.SetBool("idle", false);
                animator.SetBool("walk", false);
                animator.SetBool("attack", true);
            }
        }
        
        public IEnumerator EnemyMoveAnimator()
        {
            currentState = EnemyState.Walk;
            yield return new WaitForSeconds(0.3f);
            currentState = EnemyState.Idle;
        }
        
        public IEnumerator EnemyAttackAnimator()
        {
            currentState = EnemyState.Attack;
            yield return new WaitForSeconds(0.3f);
            currentState = EnemyState.Idle;
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
                    points.transform.GetChild(0).GetComponent<TextMesh>().text = mapScript.player.damage.ToString("F0");
                    health -= mapScript.player.damage;
                    Debug.Log("Enemy Health: " + health);
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
                    points.transform.GetChild(0).GetComponent<TextMesh>().text = damage.ToString("F0");
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
            StartCoroutine(EnemyAttackAnimator());
            _player.TakeDamage(damage);
        }

        protected override void CheckDead()
        {
            if (health <= 0)
            {
                Debug.Log("Enemy killed");
                Destroy(gameObject);
            }
        }
        
        public void CreatePath()
        {
            if (path != null && path.Count == 0)
            {
                isPathGenerated = false;
                GeneratePathToPlayer();
            }
            else if (path == null)
            {
                Debug.LogWarning("Path is null. No path could be generated.");
            }
            
            if (path.Count > 0 || path == null)
            {
                int randomValue = Random.Range(0, 100);
                if (randomValue < 50)
                {
                    Node nextNode = path[0];
                    currentNode = nextNode;
                    MoveToPosition(nextNode.transform.position.x, nextNode.transform.position.y);
                }

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
                if (IsPlayer(oldPosition))
                {
                    return;
                }
            }
            
            if (HasPlacement(newPosition))
            {
                if (IsPlayer(newPosition))
                {
                    return;
                }
                if (IsPotion(newPosition))
                {
                    Node startNode = AStarManager.instance.FindNearestNode(transform.position);
                    Node endNode = AStarManager.instance.FindNearestNode(player.position);
        
                    path = AStarManager.instance.GeneratePath(startNode, endNode);
                    path.RemoveAt(0);
                    StartCoroutine(EnemyMoveAnimator());
                    StartCoroutine(MoveSmoothly(path[0].transform.position));
                    return;
                }
                if (IsExit(newPosition))
                {
                    Node startNode = AStarManager.instance.FindNearestNode(transform.position);
                    Node endNode = AStarManager.instance.FindNearestNode(player.position);
        
                    path = AStarManager.instance.GeneratePath(startNode, endNode);
                    path.RemoveAt(0);
                    StartCoroutine(MoveSmoothly(path[0].transform.position));
                    return;
                }
                if (IsKey(newPosition))
                {
                    Node startNode = AStarManager.instance.FindNearestNode(transform.position);
                    Node endNode = AStarManager.instance.FindNearestNode(player.position);
        
                    path = AStarManager.instance.GeneratePath(startNode, endNode);
                    path.RemoveAt(0);
                    StartCoroutine(MoveSmoothly(path[0].transform.position));
                    return;
                }
                if (IsEnemy(newPosition) && newPosition != oldPosition)
                {
                    Node startNode = AStarManager.instance.FindNearestNode(transform.position);
                    Node endNode = AStarManager.instance.FindNearestNode(player.position);
        
                    path = AStarManager.instance.GeneratePath(startNode, endNode);
                    path.RemoveAt(0);
                    StartCoroutine(MoveSmoothly(path[0].transform.position));
                    return;
                }
                if (!IsEnemy(newPosition))
                {
                    path.RemoveAt(0);
                    StartCoroutine(MoveSmoothly(path[0].transform.position));
                    return;
                }
            }
            else if (IsPlayer(newPosition))
            {
                return;
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