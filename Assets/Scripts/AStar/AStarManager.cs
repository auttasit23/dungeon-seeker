using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarManager : MonoBehaviour
{
    public static AStarManager instance;

    private void Awake()
    {
        instance = this;
    }

    public List<Node> GeneratePath(Node start, Node end)
    {
        List<Node> openSet = new List<Node>();

        foreach (Node n in FindObjectsOfType<Node>())
        {
            n.gScore = float.MaxValue;
            n.previous = null;
        }

        start.gScore = 0;
        start.hScore = Vector2.Distance(start.transform.position, end.transform.position);
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            int lowestF = 0;
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FScore() < openSet[lowestF].FScore())
                {
                    lowestF = i;
                }
            }

            Node currentNode = openSet[lowestF];
            openSet.Remove(currentNode);

            if (currentNode == end)
            {
                return ReconstructPath(start, end);
            }
            foreach (Node neighbor in currentNode.connections)
            {

                float tentativeGScore = currentNode.gScore + Vector2.Distance(currentNode.transform.position, neighbor.transform.position);
                if (tentativeGScore < neighbor.gScore)
                {
                    neighbor.gScore = tentativeGScore;
                    neighbor.hScore = Vector2.Distance(neighbor.transform.position, end.transform.position);
                    neighbor.previous = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }
        
        return null;
    }

    

    private List<Node> ReconstructPath(Node start, Node end)
    {
        List<Node> path = new List<Node>();
        Node currentNode = end;
        
        while (currentNode != start)
        {
            path.Add(currentNode);
            currentNode = currentNode.previous;
        }

        path.Add(start);
        path.Reverse();

        return path;
    }


    public Node FindNearestNode(Vector2 pos)
    {
        Node foundNode = null;
        float minDistance = float.MaxValue;
        foreach (Node node in FindObjectsOfType<Node>())
        {
            float currentDistance = Vector2.Distance(pos, node.transform.position);

            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
                foundNode = node;
            }
        }
        return foundNode;
    }

    public Node FindFurthestNode(Vector2 pos)
    {
        Node foundNode = null;
        float maxDistance = default;

        foreach (Node node in FindObjectsOfType<Node>())
        {
            float currentDistance = Vector2.Distance(pos, node.transform.position);
            if (currentDistance > maxDistance)
            {
                maxDistance = currentDistance;
                foundNode = node;
            }
        }

        return foundNode;
    }
    public Node FindNearestWalkableNode(Vector2 pos)
    {
        Node nearestWalkableNode = null;
        float minDistance = float.MaxValue;
        
        foreach (Node node in FindObjectsOfType<Node>())
        {
            if (node.isWalkable)
            {
                float currentDistance = Vector2.Distance(pos, node.transform.position);
                if (currentDistance < minDistance)
                {
                    minDistance = currentDistance;
                    nearestWalkableNode = node;
                }
            }
        }

        return nearestWalkableNode;
    }

    public Node[] AllNodes()
    {
        return FindObjectsOfType<Node>();
    }
}