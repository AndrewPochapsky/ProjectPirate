﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldGenerator : MonoBehaviour {

    enum TileType { OceanTile, IslandTile }

    /// <summary>
    /// Holds all of the tiles
    /// </summary>
    Transform container;

    private Vector3 tileLocation;

    /// <summary>
    /// Value which increases the ocean tile's scale slightly to prevent weird gaps
    /// </summary>
    private Vector3 tileScaleAddition;

    float islandSpawnChance = 0.1f;
    static List<Node> nodes;

    //TODO change this
    public static List<Node> Nodes
    {
        get
        {
            return nodes;
        }
    }

    private void Awake()
    {
        nodes = new List<Node>();
        tileScaleAddition = new Vector3(50, 50, 0);

        container = GameObject.FindGameObjectWithTag("World").transform;

        tileLocation = Vector3.zero;
    }

    /// <summary>
    /// Adds the nodes to the map
    /// </summary>
    /// <param name="worldWidth">The width</param>
    /// <param name="worldHeight">The height</param>
    /// <param name="tileSize">The tileSize</param>
    public void AddNodes(int worldWidth, int worldHeight, int tileSize)
    {
        //Create the grid
        for (int y = 0; y < worldHeight; y++)
        {
            for (int x = 0; x < worldWidth; x++)
            {
                Vector2 location = new Vector2(x, y);
                nodes.Add(AddNode(location, worldWidth, tileSize));
            }
        }

        //Add the placeholder empty tiles
        foreach (var node in nodes)
        {
            node.SetAdjacents();
        }
    }

    /// <summary>
    /// Generates the ocean
    /// </summary>
    /// <param name="addIslands">True if want to include islands</param>
    /// <param name="removeNodes">True if nodes should be removed</param>
    /// <param name="tileSize">The tileSize</param>
    public void GenerateOceanTiles(bool addIslands, bool removeNodes, int tileSize)
    {
        Tile createdTile = null;

        foreach (Node node in nodes)
        {
            if (node.isAvailable)
            {
                float num = Random.Range(0f, 1f);

                if (addIslands && num <= islandSpawnChance)
                {
                    IslandTile.IslandSize size = DetermineIslandSize();

                    if (CanGenerate(node, size))
                    {
                        createdTile = AddIslandTile(node, size, tileSize);
                    }
                    else
                    {
                        createdTile = AddAnyTile(nameof(OceanTile), node, tileSize);
                    }
                }
                else
                {
                    createdTile = AddAnyTile(nameof(OceanTile), node, tileSize);
                }
            }

            if (removeNodes)
            {
                Destroy(node.gameObject);
            }
            else
            {
                createdTile.transform.SetParent(node.transform);
                node.transform.SetParent(container);
            }
        }
    }

    /// <summary>
    /// Adds either an ocean or island tile at a specified location
    /// </summary>
    /// <param name="node">The node</param>
    /// <param name="islandSize">The islandSize</param>
    /// <param name="tileSize">The tileSize</param>
    /// <returns>The created tile</returns>
    private Tile AddIslandTile(Node node, IslandTile.IslandSize islandSize, int tileSize)
    {
        Vector3 position = new Vector3(node.transform.position.x, node.transform.position.y, node.transform.position.z);
        GameObject obj = Instantiate(Resources.Load("Tiles/IslandTile"), position, Quaternion.identity) as GameObject;

        IslandTile islandTile = obj.GetComponent<IslandTile>();
        islandTile.Size = islandSize;

        islandTile.GenerateIsland();

        SetUpIsland(node, obj, tileSize);

        obj.transform.SetParent(container);

        return obj.GetComponent<Tile>();
    }

    /// <summary>
    /// Adds the specified tile at node's location
    /// </summary>
    /// <param name="tileName">The name of the tile to create</param>
    /// <param name="node">The node</param>
    /// <param name="tileSize">The tileSize</param>
    /// <returns>The created tile</returns>
    private Tile AddAnyTile(string tileName, Node node, int tileSize)
    {
        Vector3 position = new Vector3(node.transform.position.x, node.transform.position.y, node.transform.position.z);

        Quaternion rotation = Quaternion.Euler(new Vector3(90, 0, 0));

        GameObject obj = Instantiate(Resources.Load("Tiles/"+ tileName), position, rotation) as GameObject;

        obj.transform.localScale = new Vector3(tileSize, tileSize, 1);

        obj.transform.SetParent(container);

        return obj.GetComponent<Tile>();
    }

    /// <summary>
    /// Generates a map of only a specific tile
    /// </summary>
    /// <param name="tileName">Tile name to create</param>
    /// <param name="removeNodes">If true nodes will be removed</param>
    /// <param name="tileSize">The tileSize</param>
    public void GenerateTileMap(string tileName, bool removeNodes, int tileSize)
    {
        foreach(Node node in nodes)
        {
            Tile createdTile = AddAnyTile(tileName, node, tileSize);

            if (removeNodes)
            {
                Destroy(node.gameObject);
            }
            else
            {
                createdTile.transform.SetParent(node.transform);
                node.transform.SetParent(container);
            }
        }
    }

    /// <summary>
    /// Adjust island based on its size
    /// </summary>
    /// <param name="node">Node used as a location reference</param>
    /// <param name="obj">The instantiated island</param>
    /// <param name="tileSize">The tileSize</param>
    private void SetUpIsland(Node node, GameObject obj, int tileSize)
    {
        IslandTile islandTile = obj.GetComponent<IslandTile>();

        Transform oceanTileChild = obj.transform.GetChild(1);

        oceanTileChild.transform.localEulerAngles = new Vector3(90, 0, 0);

        DisableRedundantNodes(islandTile, node);

        islandTile.transform.position += GetIslandOffset(islandTile, tileSize);

        oceanTileChild.localScale = GetOceanTileSize(islandTile, tileSize);
        oceanTileChild.localScale += tileScaleAddition;
    }

    /// <summary>
    /// Add a node to the grid
    /// </summary>
    /// <param name="location">The grid location to give to the instantiated tile(row, column)</param>
    /// <param name="tileSize">The tileSize</param>
    /// <returns>The created tile</returns>
    private Node AddNode(Vector2 location, int worldWidth, int tileSize)
    {
        GameObject obj = Instantiate(Resources.Load("Tiles/"+ nameof(Node)), tileLocation, Quaternion.identity) as GameObject;
        Node tile = obj.GetComponent<Node>();

        tile.transform.SetParent(container);
        tile.location = location;

        tileLocation = GetNextLocation(tile, worldWidth, tileSize);

        return tile;
    }

    /// <summary>
    /// Get the location to add the next node
    /// </summary>
    /// <param name="node">The tile whose location is used</param>
    /// <param name="tileSize">The tileSize</param>
    /// <returns>A Vector3 of the next location</returns>
    private Vector3 GetNextLocation(Node node, int worldWidth, int tileSize)
    {
        Vector3 nextLocation = Vector3.zero;

        //The tile is currently on the edge so the next one should start new row
        if(node.location.x + 1 == worldWidth)
        {
            nextLocation = new Vector3(0, 0, tileLocation.z + tileSize);
        }
        else
        {
            nextLocation = new Vector3(tileLocation.x + tileSize, 0, tileLocation.z);
        }

        return nextLocation;
    }
    
    /// <summary>
    /// Gets the correct island offset given its size
    /// </summary>
    /// <param name="islandTile">The island tile</param>
    /// <param name="tileSize">The tileSize</param>
    /// <returns>The offset</returns>
    private Vector3 GetIslandOffset(IslandTile islandTile, int tileSize)
    {
        switch (islandTile.Size)
        {
            case IslandTile.IslandSize.Long:
                return new Vector3(tileSize / 2, 0, 0);

            case IslandTile.IslandSize.Tall:
                return new Vector3(0, 0, tileSize / 2);

            case IslandTile.IslandSize.Large:
                return new Vector3(tileSize / 2, 0, tileSize / 2);
        }
        return Vector3.zero;
    }

    /// <summary>
    /// Gets the correct oceanTileSize for the given island's oceanTile child
    /// </summary>
    /// <param name="islandTile">The island tile</param>
    /// <param name="tileSize">The tileSize</param>
    /// <returns>The size</returns>
    private Vector3 GetOceanTileSize(IslandTile islandTile, int tileSize)
    {
        switch (islandTile.Size)
        {
            case IslandTile.IslandSize.Regular:
                return new Vector3(tileSize, tileSize, 1);

            case IslandTile.IslandSize.Long:
                return new Vector3(tileSize * 2, tileSize, 1);

            case IslandTile.IslandSize.Tall:
                return new Vector3(tileSize, tileSize * 2, 1);

            case IslandTile.IslandSize.Large:
                return new Vector3(tileSize * 2, tileSize * 2, 1);
        }
        return Vector3.zero;
    }

    /// <summary>
    /// Disables nodes which are now taken up by larger island
    /// </summary>
    /// <param name="islandTile">The islandTile</param>
    /// <param name="node">The node</param>
    private void DisableRedundantNodes(IslandTile islandTile, Node node)
    {
        switch (islandTile.Size)
        {
            case IslandTile.IslandSize.Long:
                node.RightEmpty.isAvailable = false;
                break;

            case IslandTile.IslandSize.Tall:
                node.TopEmpty.isAvailable = false;
                break;

            case IslandTile.IslandSize.Large:

                if (node.RightEmpty != null)
                {
                    node.RightEmpty.isAvailable = false;
                }
                if (node.TopEmpty != null)
                {
                    if (node.TopEmpty.RightEmpty != null)
                    {
                        node.TopEmpty.RightEmpty.isAvailable = false;
                    }

                    node.TopEmpty.isAvailable = false;
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
    /// <param name="node">The node</param>
    /// <param name="size">The specified size</param>
    /// <returns>Whether or not the specified size can be generated at node's location</returns>
    private bool CanGenerate(Node node, IslandTile.IslandSize size)
    {
        switch (size)
        {
            case IslandTile.IslandSize.Regular:
                return true;

            case IslandTile.IslandSize.Long:
                if(node.RightEmpty != null)
                {
                    if (node.RightEmpty.isAvailable)
                    {
                        return true;
                    }
                }
                
                return false;

            case IslandTile.IslandSize.Tall:
                if(node.TopEmpty != null)
                {
                    if (node.TopEmpty.isAvailable)
                    {
                        return true;
                    }
                }
                
                return false;

            case IslandTile.IslandSize.Large:
                if(node.RightEmpty != null && node.TopEmpty != null && node.TopEmpty.RightEmpty != null)
                {
                    if (node.RightEmpty.isAvailable && node.TopEmpty.isAvailable && node.TopEmpty.RightEmpty.isAvailable)
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

       
