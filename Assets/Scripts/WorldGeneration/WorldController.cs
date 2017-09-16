using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public const int numberOfChunks = 5;

    public const int mapTileSize = 32;

    /// <summary>
    /// This is required in order for the island tiles to match with the ocean tiles
    /// </summary>
    int newSize = mapTileSize * 24;

    List<Node> nodes;

    private void Awake()
    {
        nodes = new List<Node>();
        tileGenerator = FindObjectOfType<TileGenerator>();
        chunkGenerator = FindObjectOfType<ChunkGenerator>();

        GenerateWorld();
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

                Chunk chunk = chunkGenerator.GenerateChunk(location, chunkLocation);
                chunkLocation = chunkGenerator.GetNextChunkLocation(chunk, newSize, chunkSize, chunkLocation);
                nodes = tileGenerator.AddNodes(chunkSize, chunkSize, newSize, chunk.transform);

                foreach (Node node in nodes)
                {
                    node.SetAdjacents(nodes);
                }
                tileGenerator.GenerateOceanTiles(nodes, addIslands: true, removeNodes: true, tileSize: newSize, parent: chunk.transform);
                tileGenerator.ResetTileLocation();
            }
        }
    }




}
