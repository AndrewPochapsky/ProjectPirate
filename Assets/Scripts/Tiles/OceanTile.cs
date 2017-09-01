using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanTile : Tile
{
    Color32 color;

    Renderer _renderer;

    private void Awake()
    {
        //Colour of island's borders
        //color = new Color32(38, 114, 114, 255);

        //_renderer = GetComponent<Renderer>();

        //_renderer.sharedMaterial.color = color;
    }

    public override void Enable(bool value)
    {
        throw new NotImplementedException();
    }
}
