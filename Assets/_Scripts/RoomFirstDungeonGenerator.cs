﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField]
    private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField]
    private int dungeonWidth = 20, dungeonHeight = 20;
    [SerializeField]
    [Range(0,10)]
    private int offset = 1;
    [SerializeField]
    private bool randomWalkRooms = false;
    
    [Header("Set Transform")]
    public Transform nodesParent;

    Dictionary<Vector2Int, Node> nodes = new Dictionary<Vector2Int, Node>();
    
    
    protected override void RunProceduralGeneration()
    {
        CreateRooms();
    }
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            tilemapVisualizer.Clear();
            ClearNodes();
            CreateRooms();
        }
    }

    public void CreateRooms()
    {
        var roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(
            new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);

        HashSet<Vector2Int> floor = CreateSimpleRooms(roomsList);

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
}