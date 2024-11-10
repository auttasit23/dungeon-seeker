using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

[System.Serializable]
public class ItemData
{
    public enum PlacementType
    {
        InOpenArea,
        NearWall
    }
    public int MinQuantity;
    public int MaxQuantity;
    public TileBase Item;
    public PlacementType Placement;
}



public class DungeonGenerator : MonoBehaviour
{
    [SerializeField]
    private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField]
    private int dungeonWidth = 20, dungeonHeight = 20;
    [SerializeField]
    [Range(0,10)]
    private int offset = 1;

    [SerializeField]
    protected SimpleRandomWalkSO randomWalkParameters;

    [Header("Set Transform")]
    public Transform nodesParent;

    Dictionary<Vector2Int, Node> nodes = new Dictionary<Vector2Int, Node>();
    
    [SerializeField]
    protected TilemapVisualizer tilemapVisualizer = null;
    [SerializeField]
    protected Vector2Int startPosition = Vector2Int.zero;
    [Header("Item Placement Data")]
    [SerializeField]
    private List<ItemData> itemDataList = new List<ItemData>();
    
    //Room Data
    public enum RoomType { PlayerRoom, EnemyRoom, TreasureRoom, ExitRoom, Other }
    
    [SerializeField] public int enemyRoomCount = 1;
    [SerializeField] public int treasureRoomCount = 1;

    public List<BoundsInt> roomsList;
    public List<RoomType> roomTypes;
    public HashSet<Vector2Int> floor;

    public void CreateRooms()
    {
        roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);
        Debug.Log($"Total rooms created: {roomsList.Count}");
        roomTypes = AssignRoomTypes(roomsList.Count);
        
        floor = CreateRoomsRandomly(roomsList);

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in roomsList)
        {
            roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
        }

        HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
        floor.UnionWith(corridors);
        
        foreach (var position in floor)
        {
            CreateNodeAtPosition(position);
        }

        ConnectNodes(floor);

        tilemapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tilemapVisualizer);
        foreach (var room in roomsList)
        {
            PlaceItemsInRoom(room);
        }
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }
        return corridors;
    }
    
    private void CreateNodeAtPosition(Vector2Int position)
    {
        if (!nodes.ContainsKey(position))
        {
            GameObject obj = new GameObject($"Node_{position.x}_{position.y}");
            obj.transform.position = new Vector3(position.x+0.5f, position.y+0.5f, 0);
            obj.transform.parent = nodesParent;

            Node node = obj.AddComponent<Node>();
            node.connections = new List<Node>();
            nodes[position] = node;
        }
    }

    
    private void ConnectNodes(HashSet<Vector2Int> floor)
    {
        foreach (var position in floor)
        {
            if (nodes.ContainsKey(position))
            {
                Node currentNode = nodes[position];
            
                Vector2Int[] directions = {
                    Vector2Int.up,  
                    Vector2Int.down,
                    Vector2Int.left,
                    Vector2Int.right
                };

                foreach (var direction in directions)
                {
                    Vector2Int neighborPos = position + direction;
                    if (floor.Contains(neighborPos) && nodes.ContainsKey(neighborPos))
                    {
                        Node neighborNode = nodes[neighborPos];
                        currentNode.connections.Add(neighborNode);
                    }
                }
            }
        }
    }
    
    private void PlaceItemsInRoom(BoundsInt room)
    {
        foreach (var item in itemDataList)
        {
            int quantity = Random.Range(item.MinQuantity, item.MaxQuantity + 1);
            for (int i = 0; i < quantity; i++)
            {
                bool itemPlaced = false;
                int attempts = 0;
                int maxAttempts = 100;

                while (!itemPlaced && attempts < maxAttempts)
                {
                    int x = Random.Range(room.xMin + 1, room.xMax - 1);
                    int y = Random.Range(room.yMin + 1, room.yMax - 1);
                    Vector2Int position = new Vector2Int(x, y);

                    attempts++;
                    if (nodes.ContainsKey(position) && nodes[position].ItemPlaced == false)
                    {
                        bool isSurrounded = CheckNodeSurroundings(position);

                        if ((item.Placement == ItemData.PlacementType.InOpenArea && isSurrounded) ||
                            (item.Placement == ItemData.PlacementType.NearWall && !isSurrounded))
                        {
                            tilemapVisualizer.PaintSingleTile(tilemapVisualizer.itemTileMap, item.Item, position);
                        
                            nodes[position].ItemPlaced = true;
                            itemPlaced = true;
                        }
                    }
                }
            }
        }
    }
    


    
    public bool CheckNodeSurroundings(Vector2Int position)
    {
        if (!nodes.ContainsKey(position)) return false;

        Node currentNode = nodes[position];
        Vector2Int[] directions = {
            Vector2Int.up,  
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        int connectedCount = 0;

        foreach (var direction in directions)
        {
            Vector2Int neighborPos = position + direction;
            if (nodes.ContainsKey(neighborPos) && currentNode.connections.Contains(nodes[neighborPos]))
            {
                connectedCount++;
            }
        }
        
        return connectedCount == directions.Length;
    }
    

    
    public void ClearNodes()
    {
        if (nodesParent != null)
        {
            foreach (Transform child in nodesParent.transform)
            {
                Destroy(child.gameObject);
            }
            nodes.Clear();
        }
    }
    
    public void ClearTile()
    {
        tilemapVisualizer.Clear();
    }


    
    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);
        while (position.y != destination.y)
        {
            if(destination.y > position.y)
            {
                position += Vector2Int.up;
            }
            else if(destination.y < position.y)
            {
                position += Vector2Int.down;
            }
            corridor.Add(position);
        }
        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector2Int.right;
            }else if(destination.x < position.x)
            {
                position += Vector2Int.left;
            }
            corridor.Add(position);
        }
        return corridor;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var position in roomCenters)
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);
            if(currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }
        return closest;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var room in roomsList)
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }
        return floor;
    }
    
    private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        for (int i = 0; i < roomsList.Count; i++)
        {
            var roomBounds = roomsList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            foreach (var position in roomFloor)
            {
                if(position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) && position.y >= (roomBounds.yMin - offset) && position.y <= (roomBounds.yMax - offset))
                {
                    floor.Add(position);
                }
            }
        }
        return floor;
    }
    
    protected HashSet<Vector2Int> RunRandomWalk(SimpleRandomWalkSO parameters, Vector2Int position)
    {
        var currentPosition = position;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for (int i = 0; i < parameters.iterations; i++)
        {
            var path = ProceduralGenerationAlgorithms.SimpleRandomWalk(currentPosition, parameters.walkLength);
            floorPositions.UnionWith(path);
            if (parameters.startRandomlyEachIteration)
                currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
        }
        return floorPositions;
    }
    
    private List<RoomType> AssignRoomTypes(int roomCount)
    {
        List<RoomType> assignedRoomTypes = new List<RoomType>();

        int remainingEnemyRooms = enemyRoomCount;
        int remainingTreasureRooms = treasureRoomCount;

        if (roomCount > 0)
        {
            assignedRoomTypes.Add(RoomType.PlayerRoom);

            for (int i = 1; i < roomCount - 1; i++)
            {
                if (remainingEnemyRooms > 0)
                {
                    assignedRoomTypes.Add(RoomType.EnemyRoom);
                    remainingEnemyRooms--;
                }
                else if (remainingTreasureRooms > 0)
                {
                    assignedRoomTypes.Add(RoomType.TreasureRoom);
                    remainingTreasureRooms--;
                }
                else
                {
                    assignedRoomTypes.Add(RoomType.Other);
                }
            }

            assignedRoomTypes.Add(RoomType.ExitRoom);
        }

        return assignedRoomTypes;
    }

    private void OnDrawGizmos()
    {
        if (roomsList == null || roomTypes == null || roomsList.Count != roomTypes.Count) return;
    
        for (int roomIndex = 0; roomIndex < roomsList.Count; roomIndex++)
        {
            Color roomColor = GetRoomColor(roomTypes[roomIndex]);
            roomColor.a = 0.1f;
        
            Gizmos.color = roomColor;

            BoundsInt room = roomsList[roomIndex];
            foreach (var position in floor)
            {
                if (position.x >= room.xMin && position.x < room.xMax &&
                    position.y >= room.yMin && position.y < room.yMax)
                {
                    Vector3 drawPos = new Vector3(position.x + 0.5f, position.y + 0.5f, 0);
                    Gizmos.DrawCube(drawPos, Vector3.one);
                }
            }
        }
    }


    private Color GetRoomColor(RoomType type)
    {
        switch (type)
        {
            case RoomType.PlayerRoom: return Color.green;
            case RoomType.EnemyRoom: return Color.red;
            case RoomType.TreasureRoom: return Color.yellow;
            case RoomType.ExitRoom: return Color.cyan;
            default: return Color.white;
        }
    }
}
