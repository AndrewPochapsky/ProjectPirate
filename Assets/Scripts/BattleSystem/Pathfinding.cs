using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinding {

    private static List<Node> path;

    public delegate void OnPathUpdated(List<Node> path);
    public static event OnPathUpdated OnPathUpdatedEvent;

    /// <summary>
    /// Finds the path between a start and target node using A*
    /// </summary>
    /// <param name="startNode">The start node</param>
    /// <param name="targetNode">The target(end) node</param>
	public static void FindPath(Node startNode, Node targetNode)
    {
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while(openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for(int i = 1; i < openSet.Count; i++)
            {
                if(openSet[i].FCost < currentNode.FCost || openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if(currentNode == targetNode)
            {
                path = RetracePath(startNode, targetNode);
                OnPathUpdatedEvent(path);
                return;
            }

            foreach(Node adjacentNode in currentNode.Adjacents)
            {
                if(!adjacentNode.traversable || closedSet.Contains(adjacentNode))
                {
                    continue;
                }

                int newMovementCostToAdjacent = currentNode.gCost + GetDistance(currentNode, adjacentNode);

                if (newMovementCostToAdjacent < adjacentNode.gCost || !openSet.Contains(adjacentNode))
                {
                    adjacentNode.gCost = newMovementCostToAdjacent;
                    adjacentNode.hCost = GetDistance(adjacentNode, targetNode);
                    adjacentNode.parent = currentNode;

                    if (!openSet.Contains(adjacentNode))
                    {
                        openSet.Add(adjacentNode);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="startNode"></param>
    /// <param name="endNode"></param>
    /// <returns></returns>
    private static List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        foreach(Node node in path)
        {
            node.transform.GetChild(0).GetComponent<Tile>().Select();
        }

        return path;
    }

    /// <summary>
    /// Uses Manhattan Distance formula |x1 - x2| + |y1 - y2|
    /// </summary>
    /// <param name="nodeA">First node</param>
    /// <param name="nodeB">Second Node</param>
    /// <returns>The distance</returns>
    private static int GetDistance(Node nodeA, Node nodeB)
    {
        return (int)Mathf.Abs(nodeA.location.x - nodeB.location.x) + (int)Mathf.Abs(nodeA.location.y - nodeB.location.y);
    }

    public static List<Node> GetRange(List<Node> nodes, Node currentNode, int range)
    {
        List<Node> rangeNodes = new List<Node>();

        foreach(Node node in nodes)
        {
            node.hCost = GetDistance(currentNode, node);
            if(node.hCost <= range)
            {
                rangeNodes.Add(node);
            }
        }
        return rangeNodes;
    }
}
