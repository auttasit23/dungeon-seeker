using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;

namespace Searching
{

    public class OOPMapGenerator : DungeonGenerator
    {
        [Header("Set Player")]
        public OOPPlayer player;
        public Vector2Int playerStartPos;
        public int maxEnemy = 1;

        [Header("Set Exit")]
        public OOPExit Exit;

        [Header("Set Prefab")]
        public GameObject[] itemsPrefab;
        public GameObject[] keysPrefab;
        public GameObject[] enemiesPrefab;

        [Header("Set Transform")]
        public Transform itemParent;
        public Transform enemyParent;

        [Header("Set object Count")]
        public int itemCount;
        public int itemKeyCount;
        public int enemyCount;
        
        
        public OOPItemPotion[,] potions;
        public OOPItemKey[,] keys;
        public Dictionary<Vector2, List<OOPEnemy>> enemies = new Dictionary<Vector2, List<OOPEnemy>>();

        // block types ...
        [Header("Block Types")]
        public int playerBlock = 99;
        public int empty = 0;
        public int potion = 2;
        public int exit = 4;
        public int key = 5;
        public int enemy = 6;
        Dictionary<Vector2, Node> nodes = new Dictionary<Vector2, Node>();

        // Start is called before the first frame update

        void Start()
        {
            CreateRooms();
            Vector3 randomPos = RandomNode();
            PlaceEnemy();
            PlacePlayer();
            PlaceExit();
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                StartCoroutine(ResetMap());
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                SceneManager.LoadScene("Map");
            }
        }
        
        public IEnumerator ResetMap()
        {
            tilemapVisualizer.Clear();
            ClearNodes();
            CreateRooms();
            RemoveAllEnemies();
            GameObject enemy = GameObject.Find("Enemy");

            if (enemy != null)
            {
                foreach (Transform child in enemy.transform)
                {
                    Destroy(child.gameObject);
                }
            }
            yield return new WaitForSeconds(0.3f);
            PlaceEnemy();
            //ย้ายผู้เล่นไปจุดเริ่มต้น
            PlacePlayer();
            //ย้ายทางออก
            PlaceExit();
        }
        
        public Vector3 FindClosestNodePosition(Vector3 targetPosition)
        {
            if (nodesParent == null)
            {
                return Vector3.negativeInfinity;
            }

            Transform closestNode = null;
            float closestDistance = float.MaxValue;

            foreach (Transform child in nodesParent)
            {
                if (child.name.StartsWith("Node_"))
                {
                    float distance = Vector3.Distance(targetPosition, child.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestNode = child;
                    }
                }
            }

            if (closestNode != null)
            {
                return closestNode.position;
            }
            else
            {
                return Vector3.negativeInfinity;
            }
        }


        public Vector3 FindLowestNodePosition()
        {
            if (nodesParent == null)
            {
                return Vector3.negativeInfinity;
            }

            Transform lowestNode = null;
            int lowestX = int.MaxValue;
            int lowestY = int.MaxValue;

            foreach (Transform child in nodesParent)
            {
                if (child.name.StartsWith("Node_"))
                {
                    string[] parts = child.name.Split('_');
                    if (parts.Length == 3 && int.TryParse(parts[1], out int x) && int.TryParse(parts[2], out int y))
                    {
                        if (x < lowestX || (x == lowestX && y < lowestY))
                        {
                            lowestX = x;
                            lowestY = y;
                            lowestNode = child;
                        }
                    }
                }
            }

            if (lowestNode != null)
            {
                return lowestNode.position;
            }
            else
            {
                return Vector3.negativeInfinity;
            }
        }



        public Vector3 FindHighestNodePosition()
        {
            if (nodesParent == null)
            {
                return Vector3.negativeInfinity;
            }

            Transform highestNode = null;
            int highestX = int.MinValue;
            int highestY = int.MinValue;

            foreach (Transform child in nodesParent)
            {
                if (child.name.StartsWith("Node_"))
                {
                    string[] parts = child.name.Split('_');
                    if (parts.Length == 3 && int.TryParse(parts[1], out int x) && int.TryParse(parts[2], out int y))
                    {
                        if (x > highestX || (x == highestX && y > highestY))
                        {
                            highestX = x;
                            highestY = y;
                            highestNode = child;
                        }
                    }
                }
            }

            if (highestNode != null)
            {
                return highestNode.position;
            }
            else
            {
                return Vector3.negativeInfinity;
            }
        }
        
        public Vector3 RandomNode()
        {
            if (nodesParent == null)
            {
                return Vector3.negativeInfinity;
            }

            Vector3 lowestPosition = FindLowestNodePosition();
            Vector3 highestPosition = FindHighestNodePosition();

            if (lowestPosition == Vector3.negativeInfinity || highestPosition == Vector3.negativeInfinity)
            {
                return Vector3.negativeInfinity;
            }

            List<Transform> eligibleNodes = new List<Transform>();

            foreach (Transform child in nodesParent)
            {
                if (child.name.StartsWith("Node_"))
                {
                    string[] parts = child.name.Split('_');
                    if (parts.Length == 3 && int.TryParse(parts[1], out int x) && int.TryParse(parts[2], out int y))
                    {
                        Vector3 childPos = child.position;
                        if (childPos.x >= lowestPosition.x && childPos.x <= highestPosition.x &&
                            childPos.y >= lowestPosition.y && childPos.y <= highestPosition.y)
                        {
                            eligibleNodes.Add(child);
                        }
                    }
                }
            }

            if (eligibleNodes.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, eligibleNodes.Count);
                return eligibleNodes[randomIndex].position;
            }
            else
            {
                return Vector3.negativeInfinity;
            }
        }
        
        public GameObject GetNode(Vector3 position)
        {
            if (nodesParent == null)
            {
                return null;
            }

            foreach (Transform child in nodesParent)
            {
                if (child.name.StartsWith("Node_") && child.position == position)
                {
                    return child.gameObject;
                }
            }

            return null;
        }



        public void PlaceItem(int x, int y)
        {
            int r = Random.Range(0, itemsPrefab.Length);
            GameObject obj = Instantiate(itemsPrefab[r], new Vector3(x, y, 0), Quaternion.identity);
            obj.transform.parent = itemParent;
            Vector2 position = new Vector2(x, y);
            potions[x, y] = obj.GetComponent<OOPItemPotion>();
            potions[x, y].positionX = x;
            potions[x, y].positionY = y;
            potions[x, y].mapGenerator = this;
            obj.name = $"Item_{potions[x, y].Name} {x}, {y}";
            
            /*Vector2 position = new Vector2(x, y);
            if (nodes.ContainsKey(position))
            {
                Node node = nodes[position];
                if (node != null)
                {
                    node.isWalkable = false; // Mark the node as unwalkable
                }
            }*/
            /*Vector2 position = new Vector2(x, y);
            if (nodes.ContainsKey(position))
            {
                Node node = nodes[position];
                if (node != null)
                {
                    Destroy(node);
                    nodes.Remove(position);
                }
            }*/
        }
        public void PlacePlayer()
        {
            for (int i = 0; i < roomsList.Count; i++)
            {
                if (roomTypes[i] == RoomType.PlayerRoom)
                {
                    Vector3 roomCenter = (Vector3)roomsList[i].center;
                    Vector2 closestNodePosition = FindClosestNodePosition(roomCenter);
                    player.transform.position = closestNodePosition;
                    SetNode(closestNodePosition, "player");
                    player.positionX = closestNodePosition.x;
                    player.positionY = closestNodePosition.y;
                    break;
                }
            }
        }
        
        public void PlaceExit()
        {
            for (int i = 0; i < roomsList.Count; i++)
            {
                if (roomTypes[i] == RoomType.ExitRoom)
                {
                    Vector3 roomCenter = (Vector3)roomsList[i].center;
                    Vector2 closestNodePosition = FindClosestNodePosition(roomCenter);
                    Exit.transform.position = new Vector3(closestNodePosition.x, closestNodePosition.y, 0);
                    SetNode(closestNodePosition, "exit");
                    player.positionX = closestNodePosition.x;
                    player.positionY = closestNodePosition.y;
                    break;
                }
            }
        }

        public void PlaceKey(int x, int y)
        {
            int r = Random.Range(0, keysPrefab.Length);
            GameObject obj = Instantiate(keysPrefab[r], new Vector3(x, y, 0), Quaternion.identity);
            obj.transform.parent = itemParent;
            Vector2 position = new Vector2(x, y);
            keys[x, y] = obj.GetComponent<OOPItemKey>();
            keys[x, y].positionX = x;
            keys[x, y].positionY = y;
            keys[x, y].mapGenerator = this;
            obj.name = $"Item_{keys[x, y].Name} {x}, {y}";
        }

        public void PlaceEnemy()
        {
            for (int i = 0; i < roomsList.Count; i++)
            {
                if (roomTypes[i] == RoomType.EnemyRoom)
                {
                    Vector3 roomCenter = (Vector3)roomsList[i].center;

                    for (int j = 0; j < maxEnemy; j++)
                    {
                        Vector2 closestNodePosition = FindClosestNodePosition(roomCenter);
                        Vector3 position = new Vector3(closestNodePosition.x, closestNodePosition.y, -1);

                        int r = Random.Range(0, enemiesPrefab.Length);
                        GameObject obj = Instantiate(enemiesPrefab[r], position, Quaternion.identity);
                        obj.transform.parent = enemyParent;

                        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
                        if (spriteRenderer != null)
                        {
                            spriteRenderer.sortingOrder = 1;
                        }

                        OOPEnemy enemyComponent = obj.GetComponent<OOPEnemy>();
                        obj.name = $"Enemy_{enemyComponent.Name}_{j + 1}";

                        Vector2 key = new Vector2(position.x, position.y);
                        
                        if (!enemies.ContainsKey(key))
                        {
                            enemies[key] = new List<OOPEnemy>();
                        }
                        enemies[key].Add(enemyComponent);

                        SetNode(position, "enemy");
                    }
                }
            }
        }
        
        public void SetNode(Vector3 position, string name)
        {
            GameObject nodeObject = GetNode(position);
    
            if (nodeObject != null)
            {
                Node nodeS = nodeObject.GetComponent<Node>();
        
                if (nodeS != null)
                {
                    nodeS.onMe = name;
                }
            }
        }
        
        public void MoveEnemies()
        {
            StartCoroutine(MoveEnemy());
        }
        
        public IEnumerator MoveEnemy()
        {
            yield return new WaitForSeconds(0.5f);
            
            List<OOPEnemy> list = new List<OOPEnemy>();
            foreach (var enemyList in enemies.Values)
            {
                list.AddRange(enemyList);
            }
            foreach (var enemy in list)
            {
                if (enemy != null)
                {
                    enemy.CreatePath();
                }
            }
        }

        public void RemoveAllEnemies()
        {
            foreach (var enemyList in enemies.Values)
            {
                foreach (var enemy in enemyList)
                {
                    if (enemy != null)
                    {
                        Destroy(enemy.gameObject);
                    }
                }
            }
            enemies.Clear();
        }
    }
}