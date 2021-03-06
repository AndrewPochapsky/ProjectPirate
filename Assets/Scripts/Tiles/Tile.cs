﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tile : MonoBehaviour {

    protected Vector2 gridLocation;

    protected Color regularColour;

    protected Renderer _renderer;

    protected virtual void Awake()
    {
        _renderer = GetComponent<Renderer>();
        //TODO change this
        if(SceneManager.GetActiveScene().name == "Battle")
            regularColour = _renderer.sharedMaterial.color;
    }

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
