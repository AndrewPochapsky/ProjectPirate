using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BaseNode : MonoBehaviour {

    public List<Node> Adjacents { get; private set; }

    //TODO remove these and just use the above list
    public Node TopEmpty { get; private set; }
    public Node BottomEmpty { get; private set; }
    public Node LeftEmpty { get; private set; }
    public Node RightEmpty { get; private set; }

    public Vector2 location;

    private void Awake()
    {
        Adjacents = new List<Node>();
    }

    /// <summary>
    /// Sets the EmptyTile's adjacent tiles
    /// </summary>
    public virtual void SetAdjacents(List<Node> nodes)
    {
        //if tile is not on left edge
        if (location.x != 0)
        {
            LeftEmpty = nodes.Single(t => t.location.x == location.x - 1 && t.location.y == location.y);
            Adjacents.Add(LeftEmpty);
        }

        //if tile is not on right edge
        if (location.x + 1 != WorldController.chunkSize)
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
        if (location.y + 1 != WorldController.chunkSize)
        {
            TopEmpty = nodes.Single(t => t.location.x == location.x && t.location.y == location.y + 1);
            Adjacents.Add(TopEmpty);
        }
    }
}
