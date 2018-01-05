using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class WorldController : MonoBehaviour {

    public static WorldController Instance;


    private Player player;
    private LocalData localData;
    private BattleData battleData;

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

    [HideInInspector]
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
    /// <returns>current node</returns>
    //public Node currentNode { get; set; }

    Transform world;

    List<BaseNode> nodes;

    [HideInInspector]
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
        localData = Resources.Load<LocalData>("Data/LocalData");
        battleData = Resources.Load<BattleData>("Data/BattleData");
        if (battleData.enemyObject != null)
        {
            Destroy(battleData.enemyObject);
            battleData.enemyObject = null;
        }
       
    }

    private void Start()
    {
        World.Instance.gameObject.SetActive(true);
        player = GameObject.FindObjectOfType<Player>();
        player.transform.position = localData.playerShipPos;

        //TODO: Create a function which updates all of the player's stuff after a battle
        
        if(battleData.Friendlies.Count > 0)
            player.entityData.Tier = battleData.Friendlies[0].Tier;
            
        player.SetInfamy(battleData.InfamyReward);
        
        battleData.InfamyReward = 0;
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

    public List<BaseNode> GetNodesNearPlayer(Transform player, BaseNode node)
    {
        List<BaseNode> nodes = new List<BaseNode>();

        List<BaseNode> tempNodes = new List<BaseNode>();
        
        //Add nodes adjacent to the current node
        //Add those nodes to the list
        //Then loop through the adjacent nodes of the ones currently in list
        // and add them to the list if they are not duplicates
        foreach(BaseNode n in node.Adjacents)
        {
            IslandTile island = node.transform.GetChild(0).GetComponent<IslandTile>();
            if(island == null)
                nodes.Add(node);
        }

        foreach(BaseNode n in nodes)
        {
            foreach(BaseNode adjNode in n.Adjacents)
            {   
                IslandTile island = null;
                if(adjNode.transform.childCount > 0)
                {
                    island = adjNode.transform.GetChild(0).GetComponent<IslandTile>();
                }
                if(island == null && adjNode != node && !tempNodes.Contains(adjNode))
                {
                    //nodes.Add(adjNode);
                    tempNodes.Add(adjNode);
                }
            }
        }

        foreach(var n in tempNodes)
        {
            nodes.Add(n);
        }
        nodes.Remove(node);

        return nodes;
    }
}
