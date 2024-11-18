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
        public int maxEnemy = 1;

        [Header("Set Exit")]
        public OOPExit Exit;

        [Header("Set Prefab")]
        public GameObject[] itemsPrefab;
        public GameObject[] keysPrefab;
        public GameObject[] treasurePrefab;
        public GameObject[] enemiesPrefab;

        [Header("Set Transform")]
        public Transform itemParent;
        public Transform enemyParent;
        public Transform potionParent;
        
        public bool selectingTreasure = false;
        public GameObject choose;
        
        public OOPItemKey keys;
        public Dictionary<Vector2, List<OOPTreasure>> treasure = new Dictionary<Vector2, List<OOPTreasure>>();
        public Dictionary<Vector2, List<OOPItemPotion>> potion = new Dictionary<Vector2, List<OOPItemPotion>>();
        public Dictionary<Vector2, List<OOPEnemy>> enemies = new Dictionary<Vector2, List<OOPEnemy>>();
        Dictionary<Vector2, Node> nodes = new Dictionary<Vector2, Node>();

        // Start is called before the first frame update

        void Start()
        {
            CreateRooms();
            PlaceEnemy();
            PlacePlayer();
            PlaceExit();
            PlaceKey();
            PlacePotion();
            PlaceTreasure();
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
            
            foreach (Transform childnode in nodesParent)
            {
                Node nodeS = childnode.GetComponent<Node>();
                bool isOccupied = false;
                
                foreach (Transform childenemy in enemyParent)
                {
                    if (childnode.transform.position == childenemy.transform.position)
                    {
                        nodeS.onMe = "enemy";
                        isOccupied = true;
                        break;
                    }
                }

                if (player != null)
                {
                    if (childnode.transform.position == player.transform.position)
                    {
                        nodeS.onMe = "player";
                        isOccupied = true;
                    }
                }

                if (Exit != null)
                {
                    if (childnode.transform.position == Exit.transform.position)
                    {
                        nodeS.onMe = "exit";
                        isOccupied = true;
                    }
                }
                
                if (keys != null)
                {
                    if (childnode.transform.position == keys.transform.position)
                    {
                        nodeS.onMe = "key";
                        isOccupied = true;
                    }
                }
                if (!isOccupied)
                {
                    nodeS.onMe = "empty";
                }
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
            yield return new WaitForSeconds(0.5f);
            PlaceEnemy();
            PlacePlayer();
            PlaceExit();
            PlaceKey();
            PlacePotion();
            PlaceTreasure();
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
        
        public void PlacePlayer()
        {
            for (int i = 0; i < roomsList.Count; i++)
            {
                if (roomTypes[i] == RoomType.PlayerRoom)
                {
                    Vector3 roomCenter = (Vector3)roomsList[i].center;
                    Vector2 closestNodePosition = FindClosestNodePosition(roomCenter);
                    player.transform.position = closestNodePosition;
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
                    Exit.positionX = closestNodePosition.x;
                    Exit.positionY = closestNodePosition.y;
                    break;
                }
            }
        }

        public void PlaceKey()
        {
            for (int i = 0; i < roomsList.Count; i++)
            {
                if (roomTypes[i] == RoomType.KeyRoom)
                {
                    Vector3 roomCenter = (Vector3)roomsList[i].center;

                    Vector2 closestNodePosition = FindClosestNodePosition(roomCenter);
                    GameObject obj = Instantiate(keysPrefab[0], closestNodePosition, Quaternion.identity);
                    obj.transform.parent = itemParent;
                    keys = obj.GetComponent<OOPItemKey>();
                    SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        spriteRenderer.sortingOrder = 1;
                    }
                }
            }
        }
        
        public void PlaceTreasure()
        {
            for (int i = 0; i < roomsList.Count; i++)
            {
                if (roomTypes[i] == RoomType.TreasureRoom)
                {
                    Vector3 randomPo = (Vector3)roomsList[i].center;
                    for (int j = 0; j < 1; j++)
                    {
                        Vector2 closestNodePosition = FindClosestNodePosition(randomPo);
                        Vector3 position = new Vector3(closestNodePosition.x, closestNodePosition.y, 0);

                        GameObject obj = Instantiate(treasurePrefab[0], position, Quaternion.identity);
                        obj.transform.parent = itemParent;

                        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
                        if (spriteRenderer != null)
                        {
                            spriteRenderer.sortingOrder = 1;
                        }
                        OOPTreasure treasureObj = obj.GetComponent<OOPTreasure>();
                        Vector2 key = new Vector2(closestNodePosition.x, closestNodePosition.y);
                        if (!treasure.ContainsKey(key))
                        {
                            treasure[key] = new List<OOPTreasure>();
                        }
                        treasure[key].Add(treasureObj);
                    }
                }
            }
        }
        
        public void PlacePotion()
        {
            for (int i = 0; i < roomsList.Count; i++)
            {
                if (roomTypes[i] == RoomType.Other)
                {
                    Vector3 randomPo = (Vector3)roomsList[i].center;
                    for (int j = 0; j < 1; j++)
                    {
                        Vector2 closestNodePosition = FindClosestNodePosition(randomPo);
                        Vector3 position = new Vector3(closestNodePosition.x, closestNodePosition.y, 0);

                        GameObject obj = Instantiate(itemsPrefab[1], position, Quaternion.identity);
                        obj.transform.parent = potionParent;

                        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
                        if (spriteRenderer != null)
                        {
                            spriteRenderer.sortingOrder = 1;
                        }
                        OOPItemPotion itemPotion = obj.GetComponent<OOPItemPotion>();
                        Vector2 key = new Vector2(closestNodePosition.x, closestNodePosition.y);
                        if (!potion.ContainsKey(key))
                        {
                            potion[key] = new List<OOPItemPotion>();
                        }
                        potion[key].Add(itemPotion);
                    }
                }
            }
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
                        Vector3 position = new Vector3(closestNodePosition.x, closestNodePosition.y, 0);

                        int r = Random.Range(0, enemiesPrefab.Length);
                        SetNode(closestNodePosition, "enemy");
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
            
            foreach (var potionList in potion.Values)
            {
                foreach (var potion in potionList)
                {
                    if (potion != null)
                    {
                        Destroy(potion.gameObject);
                    }
                }
            }
            enemies.Clear();
        }
    }
}