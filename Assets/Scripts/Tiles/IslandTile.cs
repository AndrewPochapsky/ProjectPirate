using System.Collections;
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

    //TODO remove this and check if the island can be created based on adjacent tiles
    private IslandSize DetermineIslandSize()
    {
        float randomValue = Random.Range(0f, 1f);

        if(randomValue <= 0.5f)
        {
            return IslandSize.Regular;
        }
        else if(randomValue <= 0.9f)
        {
            return IslandSize.Long;
        }
        return IslandSize.Large;
    }

    /// <summary>
    /// Calls functions which generate island
    /// </summary>
    public void GenerateIsland()
    {
        generator.DetermineSize();
        generator.GenerateIsland();
    }

}
