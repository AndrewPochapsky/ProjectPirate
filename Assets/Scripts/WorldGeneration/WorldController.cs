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

    /// <summary>
    /// The current node the player is in
    /// </summary>
    /// <returns></returns>
    public Node currentNode { get; set; }

    Transform world;

    List<BaseNode> nodes;

    public List<Chunk> Chunks;

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
            nodes = new List<BaseNode>();

            Chunks = GenerateWorld();
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
    private List<Chunk> GenerateWorld()//TODO make this return a list of chunks
    {
        List<Chunk> chunks = new List<Chunk>();
        for (int y = 0; y < numberOfChunks; y++)
        {
            for (int x = 0; x < numberOfChunks; x++)
            {
                Vector2 location = new Vector2(x, y);

                Chunk chunk = ChunkGenerator.Instance.GenerateChunk(location, chunkLocation, world);
                chunkLocation = ChunkGenerator.Instance.GetNextChunkLocation(chunk, newSize, chunkSize, chunkLocation);
                nodes = TileGenerator.Instance.AddNodes(chunkSize, chunkSize, newSize, offset: 8, parent: chunk.transform);
                
                TileGenerator.Instance.GenerateOceanTiles(nodes, addIslands: true, removeNodes: false, tileSize: newSize, parent: chunk.transform);
                TileGenerator.Instance.ResetTileLocation();

                chunks.Add(chunk);
            }
        }
        return chunks;
    }

    private void Update()
    {
        //TODO: remove this
        if (Input.GetKeyDown(KeyCode.B))
        {
            SceneManager.LoadScene(1);
            World.Instance.gameObject.SetActive(false);
        }
    }

    public List<Tile> GetTilesNearPlayer(Transform player)
    {
        List<Tile> tiles = new List<Tile>();

        return tiles;
    }
}
