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
                emptyTiles.Add(AddEmptyTile(location));
            }
        }

        //Add the placeholder empty tiles
        foreach(var emptyTile in emptyTiles)
        {
            emptyTile.SetAdjacents();
        }

        //Add actual tiles
        foreach (EmptyTile emptyTile in emptyTiles)
        {
            if (emptyTile.isAvailable)
            {
                float num = Random.Range(0f, 1f);

                if (num <= islandSpawnChance)
                {
                    IslandTile.IslandSize size = DetermineIslandSize();

                    if (CanGenerate(emptyTile, size))
                    {
                        AddIslandTile(emptyTile, size);
                    }
                    else
                    {
                        AddOceanTile(emptyTile);
                    }
                }
                else
                {
                    AddOceanTile(emptyTile);
                }
            }
            
            Destroy(emptyTile.gameObject);
        }
    }

    /// <summary>
    /// Adds either an ocean or island tile at a specified location
    /// </summary>
    /// <param name="emptyTile">The EmptyTile</param>
    /// <param name="islandSize">The islandSize</param>
    /// <returns>The created tile</returns>
    private Tile AddIslandTile(EmptyTile emptyTile, IslandTile.IslandSize islandSize)
    {
        Vector3 position = new Vector3(emptyTile.transform.position.x, emptyTile.transform.position.y, emptyTile.transform.position.z);
        GameObject obj = Instantiate(Resources.Load("Tiles/IslandTile"), position, Quaternion.identity) as GameObject;

        IslandTile islandTile = obj.GetComponent<IslandTile>();
        islandTile.Size = islandSize;

        islandTile.GenerateIsland();

        SetUpIsland(emptyTile, obj);

        obj.transform.SetParent(container);

        return obj.GetComponent<Tile>();
    }

    /// <summary>
    /// Adds an ocean tile at emptyTile location
    /// </summary>
    /// <param name="emptyTile">The emptyTile</param>
    /// <returns>The created tile</returns>
    private Tile AddOceanTile(EmptyTile emptyTile)
    {
        Vector3 position = new Vector3(emptyTile.transform.position.x, emptyTile.transform.position.y, emptyTile.transform.position.z);

        GameObject obj = Instantiate(Resources.Load("Tiles/OceanTile"), position, Quaternion.identity) as GameObject;

        obj.transform.localScale = new Vector3(newSize + oceanTileScaleAddition.x, newSize + oceanTileScaleAddition.y, 1);


        obj.transform.localEulerAngles = new Vector3(90, 0, 0);

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
    /// Gets the correct island offset given its size
    /// </summary>
    /// <param name="islandTile">The island tile</param>
    /// <returns>The offset</returns>
    private Vector3 GetIslandOffset(IslandTile islandTile)
    {
        switch (islandTile.Size)
        {
            case IslandTile.IslandSize.Long:
                return new Vector3(newSize / 2, 0, 0);

            case IslandTile.IslandSize.Tall:
                return new Vector3(0, 0, newSize / 2);

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

            case IslandTile.IslandSize.Tall:
                return new Vector3(newSize, newSize * 2, 1);

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
                emptyTile.RightEmpty.isAvailable = false;
                break;

            case IslandTile.IslandSize.Tall:
                emptyTile.TopEmpty.isAvailable = false;
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

    /// <summary>
    /// Determine the size of which to generate the island
    /// </summary>
    /// <returns>The determined size</returns>
    private IslandTile.IslandSize DetermineIslandSize()
    {
        float randomValue = Random.Range(0f, 1f);

        if (randomValue <= 0.5f)
        {
            return IslandTile.IslandSize.Regular;
        }
        else if (randomValue <= 0.7f)
        {
            return IslandTile.IslandSize.Long;
        }
        else if(randomValue <= 0.9f)
        {
            return IslandTile.IslandSize.Tall;
        }
        return IslandTile.IslandSize.Large;
    }

    /// <summary>
    /// Returns true if there are no conflicts with generating the specifed size of island
    /// </summary>
    /// <param name="emptyTile">The emptyTile</param>
    /// <param name="size">The specified size</param>
    /// <returns>Whether or not the specified size can be generated at emptyTile location</returns>
    private bool CanGenerate(EmptyTile emptyTile, IslandTile.IslandSize size)
    {
        switch (size)
        {
            case IslandTile.IslandSize.Regular:
                return true;

            case IslandTile.IslandSize.Long:
                if(emptyTile.RightEmpty != null)
                {
                    if (emptyTile.RightEmpty.isAvailable)
                    {
                        return true;
                    }
                }
                
                return false;

            case IslandTile.IslandSize.Tall:
                if(emptyTile.TopEmpty != null)
                {
                    if (emptyTile.TopEmpty.isAvailable)
                    {
                        return true;
                    }
                }
                
                return false;

            case IslandTile.IslandSize.Large:
                if(emptyTile.RightEmpty != null && emptyTile.TopEmpty != null && emptyTile.TopEmpty.RightEmpty != null)
                {
                    if (emptyTile.RightEmpty.isAvailable && emptyTile.TopEmpty.isAvailable && emptyTile.TopEmpty.RightEmpty.isAvailable)
                    {
                        return true;
                    }
                }
                
                return false;
        }

        Debug.Log("Something went wrong with island generation");
        return true;
    }
}

       

