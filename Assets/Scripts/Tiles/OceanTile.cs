using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanTile : Tile
{
    protected override void Awake()
    {
        base.Awake();
        //_renderer.materials[0].SetFloat("_StartTime", Time.time);
    }

    public override void Enable(bool value)
    {
        throw new NotImplementedException();
    }

   
    void Update()
    {
        
    }
}
