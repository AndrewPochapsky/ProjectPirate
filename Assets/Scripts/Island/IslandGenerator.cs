using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandGenerator : MonoBehaviour {

    [SerializeField]
    private int islandWidth;
    [SerializeField]
    private int islandHeight;

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
        falloffMap = FallOffGenerator.GenerateFalloffMap(islandWidth, islandHeight);
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
        float[,] noiseMap = Noise.GenerateNoiseMap(islandWidth, islandHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

        for(int y = 0; y < islandHeight; y++)
        {
            for(int x = 0; x < islandWidth; x++)
            {
                //makes map island-like
                noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x,y]);
            }
        }

        UpdateMeshHeights(terrainMaterial, MinHeight, MaxHeight);

        MapDisplay display = FindObjectOfType<MapDisplay>();

        display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve));
    }

    private void OnValidate()
    {
        if(islandWidth < 1)
        {
            islandWidth = 1;
        }
        if(islandHeight < 1)
        {
            islandHeight = 1;
        }
        
        ApplyToMaterial(terrainMaterial);

        falloffMap = FallOffGenerator.GenerateFalloffMap(islandWidth, islandHeight);


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


