using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    public enum DrawMode { NoiseMap, ColourMap, Mesh };
    public DrawMode drawMode;

    [SerializeField]
    private TerrainData terrainData;
    [SerializeField]
    private NoiseData noiseData;

    [SerializeField]
    private int mapWidth;
    [SerializeField]
    private int mapHeight;
    
    

    public TerrainType[] regions;
    
    [SerializeField]
    private bool autoUpdate;

    private float[,] falloffMap;

    public bool AutoUpdate
    {
        get
        {
            return autoUpdate;
        }
    }

    private void Awake()
    {
        falloffMap = FallOffGenerator.GenerateFalloffMap(mapWidth, mapHeight);
    }

    private void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            GenerateMap();
        }
    }

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity, noiseData.offset);

        Color[] colourMap = new Color[mapWidth * mapHeight];

        for(int y = 0; y < mapHeight; y++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                //makes map island-like
                noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x,y]);

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
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve), TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight));
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
        
        if(terrainData != null)
        {
            terrainData.OnValuesUpdated -= OnValuesUpdated;
            terrainData.OnValuesUpdated += OnValuesUpdated;
        }

        if (noiseData != null)
        {
            noiseData.OnValuesUpdated -= OnValuesUpdated;
            noiseData.OnValuesUpdated += OnValuesUpdated;
        }


        falloffMap = FallOffGenerator.GenerateFalloffMap(mapWidth, mapHeight);

    }

}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}
