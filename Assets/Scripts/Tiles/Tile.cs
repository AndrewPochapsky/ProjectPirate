﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : MonoBehaviour {

    protected Vector2 gridLocation;

    protected Color regularColour;

    private Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        regularColour = _renderer.sharedMaterial.color;
    }

    /// <summary>
    /// Enables/Disables the object according to value
    /// </summary>
    /// <param name="value">If true, enable, if false, disable</param>
    public abstract void Enable(bool value);

    public Vector2 GridLocation
    {
        get
        {
            return gridLocation;
        }
        set
        {
            gridLocation = value;
        }
    }

    public void Select(Color colour)
    {
        _renderer.material.color = colour;
    }

    public void Deselect()
    {
        if(_renderer != null)
            _renderer.material.color = regularColour;
    }
}
