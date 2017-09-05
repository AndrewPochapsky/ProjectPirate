using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {

    WorldGenerator worldGenerator;

    public const int worldWidth = 10;
    public const int worldHeight = 10;

    private void Awake()
    {
        worldGenerator = FindObjectOfType<WorldGenerator>();

        worldGenerator.GenerateWorld(worldWidth, worldHeight);
        worldGenerator.GenerateOceanTiles(addIslands: true, removeNodes: false);
    }
}
