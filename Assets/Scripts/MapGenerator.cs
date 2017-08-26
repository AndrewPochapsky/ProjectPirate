using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    public enum DrawMode { NoiseMap, ColourMap, Mesh };
    public DrawMode drawMode;

    [SerializeField]
    private int mapWidth;
    [SerializeField]
    private int mapHeight;
    [SerializeField]
    private float noiseScale;

    [SerializeField]
    private int octaves;
    [SerializeField][Range(0,1)]
    private float persistance;
    [SerializeField]
    private float lacunarity;

    [SerializeField]
    private int seed;
    [SerializeField]
    private Vector2 offset;

    [SerializeField]
    private float meshHeightMultiplier;
    [SerializeField]
    private AnimationCurve meshHeightCurve;

    public TerrainType[] regions;
    
    [SerializeField]
    private bool autoUpdate;

    public bool AutoUpdate
    {
        get
        {
            return autoUpdate;
        }
    }

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

        Color[] colourMap = new Color[mapWidth * mapHeight];

        for(int y = 0; y < mapHeight; y++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];
                for(int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colourMap[y * mapWidth + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();

        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if(drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight));
        }
        else if(drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve), TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight));
        }

        
    }

    private void OnValidate()
    {
        if(mapWidth < 1)
        {
            mapWidth = 1;
        }
        if(mapHeight < 1)
        {
            mapHeight = 1;
        }
        if(lacunarity < 1)
        {
            lacunarity = 1;
        }
        if(octaves < 0)
        {
            octaves = 0;
        }

    }

}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}
