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
	public static List<Node> FindPath(Node startNode, Node targetNode)
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
                return path;
            }

            foreach(Node adjacentNode in currentNode.Adjacents)
            {
                if(!adjacentNode.isTraversable || closedSet.Contains(adjacentNode))
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
        return null;
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

        /*foreach(Node node in path)
        {
            node.transform.GetChild(0).GetComponent<Tile>().Select(Color.black);
        }*/

        return path;
    }

    /// <summary>
    /// Uses Manhattan Distance formula |x1 - x2| + |y1 - y2|
    /// </summary>
    /// <param name="nodeA">First node</param>
    /// <param name="nodeB">Second Node</param>
    /// <returns>The distance</returns>
    public static int GetDistance(Node nodeA, Node nodeB)
    {
        return (int)Mathf.Abs(nodeA.location.x - nodeB.location.x) + (int)Mathf.Abs(nodeA.location.y - nodeB.location.y);
    }

    /// <summary>
    /// Returns a list of nodes in range
    /// </summary>
    /// <param name="nodes">The list of nodes</param>
    /// <param name="originNode">The node to start from</param>
    /// <param name="range">The range</param>
    /// <returns>List of nodes in range</returns>
    public static List<Node> GetRange(List<Node> nodes, Node originNode, int range)
    {
        List<Node> rangeNodes = new List<Node>();

        foreach(Node node in nodes)
        {
            node.hCost = GetDistance(originNode, node);
            if(node.hCost <= range)
            {
                rangeNodes.Add(node);
            }
        }
        return rangeNodes;
    }

    public static Node GetClosestNode(Node startingNode, Node targetNode, int minDistance)
    {
        //TODO fix this
        Node node = null;

        int distance = GetDistance(startingNode, targetNode);
        if(targetNode.transform.childCount == 1 && distance <= minDistance)
        {
            return targetNode;
        }
        for(int i = 0; i < targetNode.Adjacents.Count; i++)
        {
            if (targetNode.Adjacents[i] != null)
            {
                node = GetClosestNode(startingNode, targetNode.Adjacents[i], minDistance);
            }
        }

        return node;
    }
}
