﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour {

    public Chunk GenerateChunk(Vector2 gridLocation, Vector3 worldLocation)
    {
        GameObject obj = Instantiate(Resources.Load(nameof(Chunk)), worldLocation, Quaternion.identity) as GameObject;
        Chunk chunk = obj.GetComponent<Chunk>();
        chunk.location = gridLocation;

        return chunk;
    }

    public Vector3 GetNextChunkLocation(Chunk chunk, int tileSize, int numberOfTiles, Vector3 currentChunkLocation)
    {
        if (chunk.location.x + 1 == WorldController.numberOfChunks)
        {
            return new Vector3(0, 0, currentChunkLocation.z + (tileSize * numberOfTiles));
        }

        return new Vector3(currentChunkLocation.x + (tileSize * numberOfTiles), 0, currentChunkLocation.z);
    }
}
