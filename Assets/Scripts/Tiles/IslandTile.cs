using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandTile : Tile {



    public int Seed {  get; private set; }

    const int maxSeed = 10000;

   

    private void Awake()
    {
        Seed = UnityEngine.Random.Range(0, maxSeed);
    }

    public override void Enable(bool value)
    {
        throw new NotImplementedException();
    }
}
