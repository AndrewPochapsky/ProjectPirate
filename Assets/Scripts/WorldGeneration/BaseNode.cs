using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BaseNode : MonoBehaviour {

    public List<BaseNode> Adjacents { get; private set; }

    //TODO remove these and just use the above list
    public BaseNode TopEmpty { get; private set; }
    public BaseNode BottomEmpty { get; private set; }
    public BaseNode LeftEmpty { get; private set; }
    public BaseNode RightEmpty { get; private set; }

    /// <summary>
    /// Used for generation
    /// </summary>
    [HideInInspector]
    public bool isAvailable = true;

    public Vector2 location;

    private void Awake()
    {
        Adjacents = new List<BaseNode>();
    }

    /// <summary>
    /// Sets the EmptyTile's adjacent tiles
    /// </summary>
    public void SetAdjacents(List<BaseNode> nodes, int worldLimit)
    {
        //if tile is not on left edge
        if (location.x != 0)
        {
            LeftEmpty = nodes.Single(t => t.location.x == location.x - 1 && t.location.y == location.y);
            Adjacents.Add(LeftEmpty);
        }

        //if tile is not on right edge
        if (location.x + 1 != worldLimit)
        {
            RightEmpty = nodes.Single(t => t.location.x == location.x + 1 && t.location.y == location.y);
            Adjacents.Add(RightEmpty);
        }

        //if tile is not on bottom edge
        if (location.y != 0)
        {
            BottomEmpty = nodes.Single(t => t.location.x == location.x && t.location.y == location.y - 1);
            Adjacents.Add(BottomEmpty);
        }

        //if tile is not on top edge
        if (location.y + 1 != worldLimit)
        {
            TopEmpty = nodes.Single(t => t.location.x == location.x && t.location.y == location.y + 1);
            Adjacents.Add(TopEmpty);
        }
    }
}
