using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldController : MonoBehaviour {

    public static WorldController Instance;

    /// <summary>
    /// Location at which to generate the chunk
    /// </summary>
    Vector3 chunkLocation = Vector3.zero;

    /// <summary>
    /// Both the width and height of a chunk, number of tiles is chunkSize^2
    /// </summary>
    public const int chunkSize = 5;

    /// <summary>
    /// In a specific direction, actual number is numberOfChunks^2
    /// </summary>
    public const int numberOfChunks = 3;

    public const int mapTileSize = 32;

    public int oceanTileOffset = 30;

    /// <summary>
    /// This is required in order for the island tiles to match with the ocean tiles
    /// </summary>
    int newSize = mapTileSize * 24;

    /// <summary>
    /// The current island which the player is near/interacting with
    /// </summary>
    public IslandInfo currentIsland;

    Transform world;

    List<Node> nodes;

    static bool hasGenerated = false;
    
    private void Awake()
    {
        if(Instance != null && Instance != this){
			Destroy(gameObject);
		}else{
			Instance = this;
		}

        world = FindObjectOfType<World>().transform;
        if (!hasGenerated)
        {
            nodes = new List<Node>();

            GenerateWorld();
            hasGenerated = true;
        }
       
    }

    private void Start()
    {
        
        World.Instance.gameObject.SetActive(true);
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

                Chunk chunk = ChunkGenerator.Instance.GenerateChunk(location, chunkLocation, world);
                chunkLocation = ChunkGenerator.Instance.GetNextChunkLocation(chunk, newSize, chunkSize, chunkLocation);
                nodes = TileGenerator.Instance.AddNodes(chunkSize, chunkSize, newSize, offset: 8, parent: chunk.transform);
                
                TileGenerator.Instance.GenerateOceanTiles(nodes, addIslands: true, removeNodes: true, tileSize: newSize, parent: chunk.transform);
                TileGenerator.Instance.ResetTileLocation();
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            SceneManager.LoadScene(1);
            World.Instance.gameObject.SetActive(false);
        }
    }
}
