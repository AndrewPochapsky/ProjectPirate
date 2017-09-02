using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandTile : Tile {

    public enum IslandSize
    {
        Regular,
        Long,
        Large
    }

    /// <summary>
    /// Only used if long island
    /// </summary>
    public enum IslandType
    {
        Horizontal,
        Vertical,
        None
    }

    public IslandSize Size { get; private set; }

    public IslandType type = IslandType.None;

    public int Seed {  get; private set; }

    const int maxSeed = 10000;

    private void Awake()
    {
        Size = DetermineIslandSize();
        Seed = Random.Range(0, maxSeed);
    }

    public override void Enable(bool value)
    {

    }

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

}
