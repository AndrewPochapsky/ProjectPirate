using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//TODO consider making this static class to be called from WorldController
public class WorldGenerator : MonoBehaviour {

    enum TileType { OceanTile, IslandTile }

    /// <summary>
    /// Holds all of the tiles
    /// </summary>
    Transform container;

    public const int worldWidth = 10;
    public const int worldHeight = 10;

    public const int tileSize = 50;
    private Vector3 tileLocation;

    /// <summary>
    /// Value which increases the ocean tile's scale slightly to prevent weird gaps
    /// </summary>
    private Vector3 oceanTileScaleAddition;

    float islandSpawnChance = 0.1f;

    /// <summary>
    /// This is required in order for the island tiles to match with the ocean tiles
    /// </summary>
    int newSize = tileSize * 24;

    static List<EmptyTile> emptyTiles;

    public static List<EmptyTile> EmptyTiles
    {
        get
        {
            return emptyTiles;
        }
    }

    private void Awake()
    {
        oceanTileScaleAddition = new Vector3(50, 50, 0);

        container = GameObject.FindGameObjectWithTag("World").transform;

        emptyTiles = new List<EmptyTile>();
        tileLocation = Vector3.zero;

        //Create the grid
        for(int y = 0; y < worldHeight; y++)
        {
            for(int x = 0; x < worldWidth; x++)
            {
                Vector2 location = new Vector2(x, y);
                EmptyTiles.Add(AddEmptyTile(location));
            }
        }

        //Add the placeholder empty tiles
        foreach(var emptyTile in EmptyTiles)
        {
            emptyTile.SetAdjacents();
        }

        //Add actual tiles
        foreach (EmptyTile emptyTile in EmptyTiles)
        {
            if (emptyTile.isAvailable)
            {
                AddTile(emptyTile, GetNextTileType());
            }
            
            Destroy(emptyTile.gameObject);
        }

    }

    /// <summary>
    /// Adds either an ocean or island tile at a specified location
    /// </summary>
    /// <param name="emptyTile">The EmptyTile</param>
    /// <returns>The created tile</returns>
    private Tile AddTile(EmptyTile emptyTile, TileType type)
    {
        Vector3 position = new Vector3(emptyTile.transform.position.x, emptyTile.transform.position.y, emptyTile.transform.position.z);

        GameObject obj = Instantiate(Resources.Load("Tiles/"+type), position, Quaternion.identity) as GameObject;

        if (obj.GetComponent<IslandTile>())
        {
            SetUpIsland(emptyTile, obj);
        }

        if (obj.GetComponent<OceanTile>())
        {
            obj.transform.localScale = new Vector3(newSize, newSize, 1);
            obj.transform.localEulerAngles = new Vector3(90, 0, 0);
        }
       
        obj.transform.SetParent(container);

        return obj.GetComponent<Tile>();
    }

    /// <summary>
    /// Adjust island based on its size
    /// </summary>
    /// <param name="emptyTile">Empty tile used as a location reference</param>
    /// <param name="obj">The instantiated island</param>
    private void SetUpIsland(EmptyTile emptyTile, GameObject obj)
    {
        IslandTile islandTile = obj.GetComponent<IslandTile>();
        Transform oceanTileChild = obj.transform.GetChild(1);

        oceanTileChild.transform.localEulerAngles = new Vector3(90, 0, 0);

        //If vertical rotate the oceanTile so it fits
        if(islandTile.type == IslandTile.IslandType.Vertical)
        {
            oceanTileChild.localEulerAngles = new Vector3(
                            oceanTileChild.localEulerAngles.x,
                            oceanTileChild.localEulerAngles.y,
                            90);
        }

        DisableRedundantEmptyTiles(islandTile, emptyTile);

        islandTile.transform.position += GetIslandOffset(islandTile);

        oceanTileChild.localScale = GetOceanTileSize(islandTile);
        oceanTileChild.localScale += oceanTileScaleAddition;
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
            nextLocation = new Vector3(0, 0, tileLocation.z + newSize);
        }
        else
        {
            nextLocation = new Vector3(tileLocation.x + newSize, 0, tileLocation.z);
        }

        return nextLocation;
    }

    /// <summary>
    /// Gets the next tile type randomly
    /// </summary>
    /// <returns>the tile type</returns>
    private TileType GetNextTileType()
    {
        float num = Random.Range(0f, 1f);

        if(num <= islandSpawnChance)
        {
            return TileType.IslandTile;
        }

        return TileType.OceanTile;
    }
    
    /// <summary>
    /// Gets the correct island offset given its size
    /// </summary>
    /// <param name="islandTile">The island tile</param>
    /// <returns>The offset</returns>
    private Vector3 GetIslandOffset(IslandTile islandTile)
    {
        switch (islandTile.Size)
        {
            case IslandTile.IslandSize.Long:

                switch (islandTile.type)
                {
                    case IslandTile.IslandType.Horizontal:
                        return new Vector3(newSize / 2, 0, 0);

                    case IslandTile.IslandType.Vertical:
                        return new Vector3(0, 0, newSize / 2);
                }
                break;

            case IslandTile.IslandSize.Large:

                return new Vector3(newSize / 2, 0, newSize / 2);
        }
        return Vector3.zero;
    }

    /// <summary>
    /// Gets the correct oceanTileSize given the island's size
    /// </summary>
    /// <param name="islandTile">The island tile</param>
    /// <returns>The offset</returns>
    private Vector3 GetOceanTileSize(IslandTile islandTile)
    {
        switch (islandTile.Size)
        {
            case IslandTile.IslandSize.Regular:
                return new Vector3(newSize, newSize, 1);

            case IslandTile.IslandSize.Long:
                return new Vector3(newSize * 2, newSize, 1);

            case IslandTile.IslandSize.Large:
                return new Vector3(newSize * 2, newSize * 2, 1);
        }
        return Vector3.zero;
    }

    /// <summary>
    /// Disables empty tiles which are now taken up by larger island
    /// </summary>
    /// <param name="islandTile">The islandTile</param>
    /// <param name="emptyTile">The emptyTile</param>
    private void DisableRedundantEmptyTiles(IslandTile islandTile, EmptyTile emptyTile)
    {
        switch (islandTile.Size)
        {
            case IslandTile.IslandSize.Long:

                switch (islandTile.type)
                {
                    case IslandTile.IslandType.Horizontal:

                        if (emptyTile.RightEmpty != null)
                        {
                            emptyTile.RightEmpty.isAvailable = false;
                        }
                        break;

                    case IslandTile.IslandType.Vertical:

                        if (emptyTile.TopEmpty != null)
                        {
                            emptyTile.TopEmpty.isAvailable = false;
                        }
                        break;
                }
                break;

            case IslandTile.IslandSize.Large:

                if (emptyTile.RightEmpty != null)
                {
                    emptyTile.RightEmpty.isAvailable = false;
                }
                if (emptyTile.TopEmpty != null)
                {
                    if (emptyTile.TopEmpty.RightEmpty != null)
                    {
                        emptyTile.TopEmpty.RightEmpty.isAvailable = false;
                    }

                    emptyTile.TopEmpty.isAvailable = false;
                }
                break;
        }
    }

}
