using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldController : MonoBehaviour {

    TileGenerator tileGenerator;
    ChunkGenerator chunkGenerator;

    /// <summary>
    /// Location at which to generate the chunk
    /// </summary>
    Vector3 chunkLocation = Vector3.zero;

    /// <summary>
    /// Both the width and height of a chunk, number of tiles is chunkSize^2
    /// </summary>
    public const int chunkSize = 4;

    /// <summary>
    /// In a specific direction, actual number is numberOfChunks^2
    /// </summary>
    public const int numberOfChunks = 1;

    public const int mapTileSize = 32;

    /// <summary>
    /// This is required in order for the island tiles to match with the ocean tiles
    /// </summary>
    int newSize = mapTileSize * 24;

    Transform world;

    List<Node> nodes;

    static bool hasGenerated = false;

    private void Awake()
    {
        world = FindObjectOfType<World>().transform;
        if (!hasGenerated)
        {
            nodes = new List<Node>();
            tileGenerator = FindObjectOfType<TileGenerator>();
            chunkGenerator = FindObjectOfType<ChunkGenerator>();

            GenerateWorld();
            hasGenerated = true;
        }
       
    }

    private void Start()
    {
        World.worldInstance.gameObject.SetActive(true);
    }

    /// <summary>
    /// Generates the world
    /// </summary>
    private void GenerateWorld()//TODO make this return a list of chunks
    {
        for (int y = 0; y < numberOfChunks; y++)
        {
            for (int x = 0; x < numberOfChunks; x++)
            {
                Vector2 location = new Vector2(x, y);

                Chunk chunk = chunkGenerator.GenerateChunk(location, chunkLocation, world);
                chunkLocation = chunkGenerator.GetNextChunkLocation(chunk, newSize, chunkSize, chunkLocation);
                nodes = tileGenerator.AddNodes(chunkSize, chunkSize, newSize, chunk.transform);
                
                tileGenerator.GenerateOceanTiles(nodes, addIslands: false, removeNodes: true, tileSize: newSize, parent: chunk.transform);
                tileGenerator.ResetTileLocation();
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            SceneManager.LoadScene(1);
            World.worldInstance.gameObject.SetActive(false);
        }
    }
}
