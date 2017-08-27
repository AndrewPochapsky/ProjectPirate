using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    public enum DrawMode { NoiseMap, Mesh };
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

    
    [SerializeField]
    private bool autoUpdate;

    [SerializeField]
    private Material terrainMaterial;

    [SerializeField]
    private Color[] baseColours;

    [Range(0,1)][SerializeField]
    private float[] baseStartHeights;

    [SerializeField][Range(0,1)]
    private float[] baseBlends;

    private float[,] falloffMap;

    const float constantValue = 20f;

    public bool AutoUpdate
    {
        get
        {
            return autoUpdate;
        }
    }

    public float MinHeight
    {
        get
        {
            return (constantValue/5) * meshHeightMultiplier * meshHeightCurve.Evaluate(0);
        }
    }

    public float MaxHeight
    {
        get
        {
            return constantValue * meshHeightMultiplier * meshHeightCurve.Evaluate(1);
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
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

        for(int y = 0; y < mapHeight; y++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                //makes map island-like
                noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x,y]);
            }
        }

        UpdateMeshHeights(terrainMaterial, MinHeight, MaxHeight);

        MapDisplay display = FindObjectOfType<MapDisplay>();

        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if(drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve));

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
        
        ApplyToMaterial(terrainMaterial);

        falloffMap = FallOffGenerator.GenerateFalloffMap(mapWidth, mapHeight);


    }

    private void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
    {
        material.SetFloat("minHeight", minHeight);
        material.SetFloat("maxHeight", maxHeight);
    }

    private void ApplyToMaterial(Material material)
    {
        material.SetColorArray("baseColours", baseColours);
        material.SetFloatArray("baseStartHeights", baseStartHeights);
        material.SetFloatArray("baseBlends", baseBlends);
        material.SetInt("baseColourCount", baseColours.Length);

        UpdateMeshHeights(material, MinHeight, MaxHeight);
    }

}


