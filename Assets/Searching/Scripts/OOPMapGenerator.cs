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

        public int[,] mapdata;
        
        public OOPItemPotion[,] potions;
        public OOPItemKey[,] keys;
        public OOPEnemy[,] enemies;

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
            int exitPosX = Mathf.RoundToInt(exitPosition.x);
            int exitPosY = Mathf.RoundToInt(exitPosition.y);
            Exit.transform.position = new Vector3(exitPosition.x, exitPosition.y, 0);
            /*mapdata[exitPosX, exitPosY] = exit;*/
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
        
 

        public void PlaceItem(int x, int y)
        {
            int r = Random.Range(0, itemsPrefab.Length);
            GameObject obj = Instantiate(itemsPrefab[r], new Vector3(x, y, 0), Quaternion.identity);
            obj.transform.parent = itemParent;
            mapdata[x, y] = potion;
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
            mapdata[x, y] = key;
            keys[x, y] = obj.GetComponent<OOPItemKey>();
            keys[x, y].positionX = x;
            keys[x, y].positionY = y;
            keys[x, y].mapGenerator = this;
            obj.name = $"Item_{keys[x, y].Name} {x}, {y}";
        }

        public void PlaceEnemy(int x, int y)
        {
            int r = Random.Range(0, enemiesPrefab.Length);
            GameObject obj = Instantiate(enemiesPrefab[r], new Vector3(x, y, 0), Quaternion.identity);
            obj.transform.parent = enemyParent;
            mapdata[x, y] = enemy;
            enemies[x, y] = obj.GetComponent<OOPEnemy>();
            enemies[x, y].positionX = x;
            enemies[x, y].positionY = y;
            enemies[x, y].mapGenerator = this;
            obj.name = $"Enemy_{enemies[x, y].Name} {x}, {y}";
        }
        

        public OOPEnemy[] GetEnemies()
        {
            List<OOPEnemy> list = new List<OOPEnemy>();
            foreach (var enemy in enemies)
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
            foreach (var enemy in enemies)
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