using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace Searching
{

    public class OOPMapGenerator : RoomFirstDungeonGenerator
    {
        [Header("Set Player")]
        public OOPPlayer player;
        public Vector2Int playerStartPos;

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

        public Dictionary<Vector2, float> mapdata = new Dictionary<Vector2, float>();
        
        public OOPItemPotion[,] potions;
        public OOPItemKey[,] keys;
        public Dictionary<Vector2, OOPEnemy> enemies = new Dictionary<Vector2, OOPEnemy>();

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
            PlaceEnemy(randomPos.x,randomPos.y);
            Vector3 startPosition = FindLowestNodePosition();
            if (startPosition != Vector3.negativeInfinity)
            {
                player.mapGenerator = this;
                player.positionX = startPosition.x;
                player.positionY = startPosition.y;
                player.transform.position = new Vector3(startPosition.x, startPosition.y, -0.1f);
            }

            int count = 0;
            /*potions = new OOPItemPotion[X, Y];
            count = 0;
            while (count < itemCount)
            {
                int x = Random.Range(0, X);
                int y = Random.Range(0, Y);
                if (mapdata[x, y] == empty)
                {
                    PlaceItem(x, y);
                    count++;
                }
            }

            keys = new OOPItemKey[X, Y];
            count = 0;
            while (count < itemKeyCount)
            {
                int x = Random.Range(0, X);
                int y = Random.Range(0, Y);
                if (mapdata[x, y] == empty)
                {
                    PlaceKey(x, y);
                    count++;
                }
            }

            enemies = new OOPEnemy[X, Y];
            count = 0;
            while (count < enemyCount)
            {
                int x = Random.Range(0, X);
                int y = Random.Range(0, Y);
                if (mapdata[x, y] == empty)
                {
                    PlaceEnemy(x, y);
                    AStarManager.instance.FindNearestNode(player.transform.position);
                    count++;
                }
            }*/
            
            Vector3 exitPosition = FindHighestNodePosition();
            Exit.transform.position = new Vector3(exitPosition.x, exitPosition.y, 0);
            /*mapdata[exitPosX, exitPosY] = exit;*/
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                StartCoroutine(ResetMap());
            }
        }
        
        public IEnumerator ResetMap()
        {
            tilemapVisualizer.Clear();
            ClearNodes();
            CreateRooms();
            yield return new WaitForSeconds(0.3f);
            //ย้ายผู้เล่นไปจุดเริ่มต้น
            Vector3 startPosition = FindLowestNodePosition();
            if (startPosition != Vector3.negativeInfinity)
            {
                player.mapGenerator = this;
                player.positionX = startPosition.x;
                player.positionY = startPosition.y;
                player.transform.position = new Vector3(startPosition.x, startPosition.y, -0.1f);
            }
                
            //ย้ายทางออก
            Vector3 exitPosition = FindHighestNodePosition();
            int exitPosX = Mathf.RoundToInt(exitPosition.x);
            int exitPosY = Mathf.RoundToInt(exitPosition.y);
            Exit.transform.position = new Vector3(exitPosition.x, exitPosition.y, 0);
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
                        // Compare based on X and Y values
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



 

        public void PlaceItem(int x, int y)
        {
            int r = Random.Range(0, itemsPrefab.Length);
            GameObject obj = Instantiate(itemsPrefab[r], new Vector3(x, y, 0), Quaternion.identity);
            obj.transform.parent = itemParent;
            Vector2 position = new Vector2(x, y);
            mapdata[position] = potion;
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


        public void PlaceKey(int x, int y)
        {
            int r = Random.Range(0, keysPrefab.Length);
            GameObject obj = Instantiate(keysPrefab[r], new Vector3(x, y, 0), Quaternion.identity);
            obj.transform.parent = itemParent;
            Vector2 position = new Vector2(x, y);
            mapdata[position] = key;
            keys[x, y] = obj.GetComponent<OOPItemKey>();
            keys[x, y].positionX = x;
            keys[x, y].positionY = y;
            keys[x, y].mapGenerator = this;
            obj.name = $"Item_{keys[x, y].Name} {x}, {y}";
        }

        public void PlaceEnemy(float x, float y)
        {
            int r = Random.Range(0, enemiesPrefab.Length);
            GameObject obj = Instantiate(enemiesPrefab[r], new Vector3(x, y, -1), Quaternion.identity);
            obj.transform.parent = enemyParent;
            SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sortingOrder = 1;
            }
            Vector2 position = new Vector2(x, y);

            mapdata[position] = enemy;
            OOPEnemy enemyComponent = obj.GetComponent<OOPEnemy>();
            enemies[position] = enemyComponent;
            enemyComponent.positionX = x;
            enemyComponent.positionY = y;
            enemyComponent.mapGenerator = this;

            obj.name = $"Enemy_{enemyComponent.Name} {x}, {y}";
        }



        
        public OOPEnemy[] GetEnemies()
        {
            List<OOPEnemy> list = new List<OOPEnemy>();
            foreach (var enemy in enemies.Values)
            {
                if (enemy != null)
                {
                    list.Add(enemy);
                }
            }
            return list.ToArray();
        }
        public void MoveEnemies()
        {
            if (enemies == null) return;
            List<OOPEnemy> list = new List<OOPEnemy>();
            foreach (var enemy in enemies.Values)
            {
                if (enemy != null)
                {
                    list.Add(enemy);
                }
            }
            foreach (var enemy in list)
            {
                enemy.CreatePath();
            }
        }
    }
}