using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//TODO consider using as battle squares, if so should probably be renamed
public class EmptyTile : MonoBehaviour {

    public Vector2 location;

    [SerializeField]
    EmptyTile leftEmpty, rightEmpty, topEmpty, bottomEmpty;

    /// <summary>
    /// Sets the EmptyTile's adjacent tiles
    /// </summary>
    public void SetAdjacents()
    {
        //if tile is not on left edge
        if(location.x != 0)
        {
            leftEmpty = WorldGenerator.EmptyTiles.Single(t => t.location.x == location.x - 1 && t.location.y == location.y);
        }

        //if tile is not on right edge
        if(location.x + 1 != WorldGenerator.worldWidth)
        {
            rightEmpty = WorldGenerator.EmptyTiles.Single(t => t.location.x == location.x + 1 && t.location.y == location.y);
        }

        //if tile is not on bottom edge
        if(location.y != 0)
        {
            bottomEmpty = WorldGenerator.EmptyTiles.Single(t => t.location.x == location.x && t.location.y == location.y - 1);
        }

        //if tile is not on top edge
        if(location.y + 1 != WorldGenerator.worldHeight)
        {
            topEmpty = WorldGenerator.EmptyTiles.Single(t => t.location.x == location.x && t.location.y == location.y + 1);
        }
    }

    

}
