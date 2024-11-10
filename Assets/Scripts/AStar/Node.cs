using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public string onMe = "empty";
    
    public Node cameFrom;
    public List<Node> connections;
    public bool isWalkable = true;
    public bool ItemPlaced = false;
    public Node previous;

    public float gScore;
    public float hScore;
    public bool drawGizmos = true;

    public float FScore()
    {
        return gScore + hScore;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        if (this == null || gameObject == null)
        {
            return;
        }
        
        if (onMe == "player")
        {
            Gizmos.color = Color.cyan;
        }
        
        else if (onMe == "exit")
        {
            Gizmos.color = Color.green;
        }
        
        else if (onMe == "enemy")
        {
            Gizmos.color = Color.magenta;
        }
        else
        {
            Gizmos.color = Color.blue;
        }
        
        if (connections.Count > 0)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                Gizmos.DrawLine(transform.position, connections[i].transform.position);
            }
        }
    }

}