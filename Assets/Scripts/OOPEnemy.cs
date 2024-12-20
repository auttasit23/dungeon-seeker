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
        public IEnumerator EnemyAttackAnimator()
        {
            currentState = EnemyState.Attack;
            yield return new WaitForSeconds(0.5f);
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
                    GameObject points = Instantiate(floatingPoints, new Vector3(targetPosition.x, targetPosition.y, -1), Quaternion.identity);
                    points.transform.GetChild(0).GetComponent<TextMesh>().text = mapScript.player.damage.ToString("F0");
                    health -= mapScript.player.damage;
                    Debug.Log("Enemy Health: " + health);
                }
                else
                {
                    GameObject points = Instantiate(floatingPoints, new Vector3(targetPosition.x, targetPosition.y, -1), Quaternion.identity);
                    TextMesh textMesh = points.transform.GetChild(0).GetComponent<TextMesh>();
                    textMesh.text = "Miss";
                    textMesh.color = Color.green;
                    Debug.Log("Enemy dodge");
                }
                if (mapScript.player.evasion > randomValue)
                {
                    GameObject points = Instantiate(floatingPoints, new Vector3(mapScript.player.transform.position.x, mapScript.player.transform.position.y, -1), Quaternion.identity);
                    TextMesh textMesh = points.transform.GetChild(0).GetComponent<TextMesh>();
                    textMesh.text = "Miss";
                    textMesh.color = Color.green;
                    Debug.Log("Enemy Attack Miss");
                }
                else
                {
                    GameObject points = Instantiate(floatingPoints, new Vector3(mapScript.player.transform.position.x, mapScript.player.transform.position.y, -1), Quaternion.identity);
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
            if (Random.Range(0, 100) >= 50) // 50% ไม่เดิน
            {
                return;
            }

            if (path == null)
            {
                Debug.LogWarning("Path is null. Retrying to generate path.");
                GeneratePathToPlayer();
                return;
            }

            if (path.Count == 0)
            {
                Debug.LogWarning("Path is empty. Retrying to generate path.");
                isPathGenerated = false;
                GeneratePathToPlayer();
                return;
            }

            Node nextNode = path[0];
            if (nextNode != null)
            {
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
            if (player == null)
            {
                Debug.LogError("Player transform is null. Cannot generate path.");
                return;
            }

            if (AStarManager.instance == null)
            {
                Debug.LogError("AStarManager instance is null. Cannot generate path.");
                return;
            }

            Node startNode = AStarManager.instance.FindNearestNode(transform.position);
            Node endNode = AStarManager.instance.FindNearestNode(player.position);

            if (startNode == null || endNode == null)
            {
                Debug.LogError("Start or End Node is null. Cannot generate path.");
                return;
            }

            path = AStarManager.instance.GeneratePath(startNode, endNode);

            if (path == null || path.Count == 0)
            {
                Debug.LogWarning("Path generation failed. Retrying...");
                StartCoroutine(RetryGeneratePath());
                return;
            }

            currentPathIndex = 0;
            isPathGenerated = true;
            Debug.Log("Path generated successfully.");

            CreatePath();
            MoveOneStepTowardsPlayer();
        }
        private IEnumerator RetryGeneratePath()
        {
            yield return new WaitForSeconds(0.5f);
            GeneratePathToPlayer();
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

            if (player == null)
            {
                return;
            }
            
            if (IsPlayer(newPosition))
            {
                return;
            }
            
            if (HasPlacement(oldPosition))
            {
                if (IsEnemy(oldPosition) && oldPosition == newPosition)
                {
                    path.RemoveAt(0);
                    return;
                }

                if (IsPlayer(oldPosition))
                {
                    return;
                }
            }
            
            if (HasPlacement(newPosition))
            {
                if (HandleObstacleInNewPosition(newPosition))
                {
                    return;
                }
            }
            else
            {
                if (path.Count > 0)
                {
                    path.RemoveAt(0);
                }
                StartCoroutine(MoveSmoothly(newPosition));
            }
            
            UpdateEnemyPositionInMap(oldPosition, newPosition);
        }
        private void UpdateEnemyPositionInMap(Vector2 oldPosition, Vector2 newPosition)
        {
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
        private bool HandleObstacleInNewPosition(Vector2 newPosition)
        {
            if (IsPlayer(newPosition))
            {
                return true;
            }
            if (IsTreasure(newPosition))
            {
                return true;
            }
            if (IsExit(newPosition))
            {
                return true;
            }
            if (IsKey(newPosition))
            {
                return true;
            }
            if (IsEnemy(newPosition))
            {
                return true;
            }

            return false;
        }
    }
}