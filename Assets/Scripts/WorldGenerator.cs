using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//TODO consider making this static class to be called from WorldController
public class WorldGenerator : MonoBehaviour {

    /// <summary>
    /// Holds all of the tiles
    /// </summary>
    Transform container;

    public const int worldWidth = 5;
    public const int worldHeight = 5;

    public const int tileSize = 100;
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

        //Add the placeholder empty tiles
        foreach(var emptyTile in EmptyTiles)
        {
            emptyTile.SetAdjacents();
        }

        //Add actual tiles
        foreach (Transform location in EmptyTiles.Select(t => t.transform).ToList())
        {
            AddTile(location);

            Destroy(location.gameObject);
        }

    }

    /// <summary>
    /// Adds either an ocean or island tile at a specified location
    /// </summary>
    /// <param name="location">The location at which to add the tile</param>
    /// <returns>The created tile</returns>
    private Tile AddTile(Transform location)
    {
        //for now just creates ocean tiles:
        Vector3 position = new Vector3(location.position.x + tileSize / 2, location.position.y, location.position.z + tileSize / 2);

        GameObject obj = Instantiate(Resources.Load("Tiles/OceanTile"), position, Quaternion.Euler(new Vector3(90, 0, 0))) as GameObject;
        obj.transform.localScale = new Vector3(tileSize, tileSize, 1);
        obj.transform.SetParent(container);

        return obj.GetComponent<Tile>();
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
