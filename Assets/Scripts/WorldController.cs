using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {

    WorldGenerator worldGenerator;

    public const int worldWidth = 10;
    public const int worldHeight = 10;

    public const int mapTileSize = 50;

    /// <summary>
    /// This is required in order for the island tiles to match with the ocean tiles
    /// </summary>
    int newSize = mapTileSize * 24;

    private void Awake()
    {
        worldGenerator = FindObjectOfType<WorldGenerator>();

        worldGenerator.AddNodes(worldWidth, worldHeight, newSize);
        worldGenerator.GenerateOceanTiles(addIslands: true, removeNodes: false, tileSize: newSize);
    }
}
