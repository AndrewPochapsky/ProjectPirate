using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node : BaseNode {

    public Tile Child { get; private set; }


    /// <summary>
    /// Used for generation
    /// </summary>
    public bool isAvailable = true;

    //TODO make this protected and set depending on the node type
    public bool traversable = true;

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

    private void Start()
    {
        if(transform.childCount > 0)
            Child = transform.GetChild(0).GetComponent<Tile>();

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
            Child.Deselect();
        }
    }

   
}
