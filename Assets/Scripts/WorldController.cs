using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {

    WorldGenerator worldGenerator;

    Vector3 chunkLocation = Vector3.zero;

    public const int chunkSize = 2;

    public const int numberOfChunks = 4;

    public const int mapTileSize = 50;

    /// <summary>
    /// This is required in order for the island tiles to match with the ocean tiles
    /// </summary>
    int newSize = mapTileSize * 24;

    List<Node> nodes;

    private void Awake()
    {
        nodes = new List<Node>();
        worldGenerator = FindObjectOfType<WorldGenerator>();

        for(int y = 0; y < numberOfChunks/2; y++)
        {
            for(int x = 0; x < numberOfChunks/2; x++)
            {
                Vector2 location = new Vector2(x, y);
                Chunk chunk = worldGenerator.GenerateChunk(location, chunkLocation);
                chunkLocation = worldGenerator.GetNextChunkLocation(chunk, newSize);
                nodes = worldGenerator.AddNodes(chunkSize, chunkSize, newSize, chunk.transform);
                foreach(Node node in nodes)
                {
                    node.SetAdjacents(nodes);
                }
                worldGenerator.GenerateOceanTiles(nodes, addIslands: true, removeNodes: true, tileSize: newSize, parent: chunk.transform);
                //worldGenerator.GenerateOceanTiles()
            }
        }

        //worldGenerator.AddNodes(chunkSize, chunkSize, newSize);
       ;
    }



}
