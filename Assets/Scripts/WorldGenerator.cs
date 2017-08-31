using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO consider making this static class to be called from WorldController
public class WorldGenerator : MonoBehaviour {

    Transform container;

    public const int worldWidth = 10;
    public const int worldHeight = 10;

    public const int tileSize = 50;
    private Vector3 tileLocation;

    int currentTileIndex;

    static EmptyTile[] emptyTiles;

    public static EmptyTile[] EmptyTiles
    {
        get
        {
            return emptyTiles;
        }
    }

    private void Awake()
    {
        container = GameObject.FindGameObjectWithTag("World").transform;

        emptyTiles = new EmptyTile[worldHeight * worldWidth];
        tileLocation = Vector3.zero;

        currentTileIndex = 0;

        //Create the grid
        for(int y = 0; y < worldHeight; y++)
        {
            for(int x = 0; x < worldWidth; x++)
            {
                Vector2 location = new Vector2(x, y);
                EmptyTiles[currentTileIndex] = AddEmptyTile(location);
            }
        }

        foreach(var emptyTile in EmptyTiles)
        {
            emptyTile.SetAdjacents();
        }

    }

    /// <summary>
    /// Add an empty tile to the grid
    /// </summary>
    /// <param name="location">The grid location to give to the instantiated tile(row, column)</param>
    /// <returns>The created tile</returns>
    private EmptyTile AddEmptyTile(Vector2 location)
    {
        GameObject obj = Instantiate(Resources.Load("Tiles/EmptyTile"), tileLocation, Quaternion.identity) as GameObject;
        EmptyTile tile = obj.GetComponent<EmptyTile>();

        tile.transform.SetParent(container);
        tile.location = location;

        tileLocation = GetNextLocation(tile);

        currentTileIndex++;

        return tile;
    }

    /// <summary>
    /// Get the location to add the next empty tile
    /// </summary>
    /// <param name="tile">The tile whose location is used</param>
    /// <returns>A Vector3 of the next location</returns>
    private Vector3 GetNextLocation(EmptyTile tile)
    {
        Vector3 nextLocation = Vector3.zero;

        //The tile is currently on the edge so the next one should start new row
        if(tile.location.x + 1 == worldWidth)
        {
            nextLocation = new Vector3(0, 0, tileLocation.z + tileSize);
        }
        else
        {
            nextLocation = new Vector3(tileLocation.x + tileSize, 0, tileLocation.z);
        }

        return nextLocation;
    }

    
    
}
