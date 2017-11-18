﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandTile : Tile {

    public enum IslandSize
    {               //Sizes:
        Regular,    // 1x1
        Long,       // 2x1
        Tall,       // 1x2
        Large       // 2x2
    }

    public IslandSize Size { get; set; }

    public UniqueIslandData UniqueIslandData { get; private set; }

    private IslandGenerator generator;

    private void Awake()
    {
        generator = GetComponent<IslandGenerator>();
        UniqueIslandData = new UniqueIslandData();

        UniqueIslandData.Initialize();
    }

    public override void Enable(bool value)
    {

    }

    /// <summary>
    /// Calls functions which generate island
    /// </summary>
    public void GenerateIsland()
    {
        generator.DetermineSize();
        generator.GenerateIsland();
    }


    /// <summary>
    /// Determine the size of which to generate the island
    /// </summary>
    /// <returns>The determined size</returns>
    public static IslandSize DetermineIslandSize()
    {
        float randomValue = Random.Range(0f, 1f);

        if (randomValue <= 0.5f)
        {
            return IslandSize.Regular;
        }
        else if (randomValue <= 0.7f)
        {
            return IslandSize.Long;
        }
        else if (randomValue <= 0.9f)
        {
            return IslandSize.Tall;
        }
        return IslandSize.Large;
    }

    /// <summary>
    /// Gets the correct island offset given its size
    /// </summary>
    /// <param name="islandTile">The island tile</param>
    /// <param name="tileSize">The tileSize</param>
    /// <returns>The offset</returns>
    public static Vector3 GetIslandOffset(IslandTile islandTile, int tileSize)
    {
        switch (islandTile.Size)
        {
            case IslandSize.Long:
                return new Vector3(tileSize / 2, 0, 0);

            case IslandSize.Tall:
                return new Vector3(0, 0, tileSize / 2);

            case IslandSize.Large:
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
    public static Vector3 GetChildOceanTileOffset(IslandTile islandTile, int tileSize)
    {
        switch (islandTile.Size)
        {
            case IslandSize.Regular:
                return new Vector3(tileSize, 1, tileSize);

            case IslandSize.Long:
                return new Vector3(tileSize * 2, 1, tileSize);

            case IslandSize.Tall:
                return new Vector3(tileSize, 1, tileSize * 2);

            case IslandSize.Large:
                return new Vector3(tileSize * 2, 1, tileSize * 2);
        }
        return Vector3.zero;
    }

}
