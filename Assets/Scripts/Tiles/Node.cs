using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node : MonoBehaviour {

    Tile child;

    public Vector2 location;

    /// <summary>
    /// Used for generation
    /// </summary>
    public bool isAvailable = true;

    //TODO make this protected and set depending on the node type
    public bool traversable = true;

    public List<Node> Adjacents { get; private set; }

    //TODO remove these and just use the above list
    public Node TopEmpty { get; private set; }
    public Node BottomEmpty { get; private set; }
    public Node LeftEmpty { get; private set; }
    public Node RightEmpty { get; private set; }

    /// <summary>
    /// Movement cost of moving to node
    /// </summary>
    public int gCost;

    /// <summary>
    /// Movement distance from this node to targetNode
    /// </summary>
    public int hCost;

    /// <summary>
    /// Used for pathfinding
    /// </summary>
    public Node parent;

    public int FCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    private void Awake()
    {
        Adjacents = new List<Node>();
    }

    private void Start()
    {
        child = transform.GetChild(0).GetComponent<Tile>();
        Pathfinding.OnPathUpdatedEvent += OnPathUpdated;
    }

    /// <summary>
    /// Deselect any node which is not in the current path
    /// </summary>
    /// <param name="path">The path of nodes</param>
    private void OnPathUpdated(List<Node> path)
    {
        if (!path.Contains(this))
        {
            child.Deselect();
        }
    }

    /// <summary>
    /// Sets the EmptyTile's adjacent tiles
    /// </summary>
    public void SetAdjacents()
    {
        //if tile is not on left edge
        if(location.x != 0)
        {
            LeftEmpty = WorldGenerator.Nodes.Single(t => t.location.x == location.x - 1 && t.location.y == location.y);
            Adjacents.Add(LeftEmpty);
        }

        //if tile is not on right edge
        if(location.x + 1 != WorldController.worldWidth)
        {
            RightEmpty = WorldGenerator.Nodes.Single(t => t.location.x == location.x + 1 && t.location.y == location.y);
            Adjacents.Add(RightEmpty);
        }

        //if tile is not on bottom edge
        if(location.y != 0)
        {
            BottomEmpty = WorldGenerator.Nodes.Single(t => t.location.x == location.x && t.location.y == location.y - 1);
            Adjacents.Add(BottomEmpty);
        }

        //if tile is not on top edge
        if(location.y + 1 != WorldController.worldHeight)
        {
            TopEmpty = WorldGenerator.Nodes.Single(t => t.location.x == location.x && t.location.y == location.y + 1);
            Adjacents.Add(TopEmpty);
        }
    }
}
