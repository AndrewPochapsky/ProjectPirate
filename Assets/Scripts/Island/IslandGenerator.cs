using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandGenerator : MonoBehaviour {

    private float islandWidth;

    private float islandHeight;

    private int seed;

    [SerializeField]
    private PersistantIslandData islandData;

    private IslandTile islandTile;

    private float[,] falloffMap;

    const float constantValue = 20f;

    public bool AutoUpdate
    {
        get
        {
            return islandData.autoUpdate;
        }
    }

    public float MinHeight
    {
        get
        {
            return (constantValue/5) * islandData.meshHeightMultiplier * islandData.meshHeightCurve.Evaluate(0);
        }
    }

    public float MaxHeight
    {
        get
        {
            return constantValue * islandData.meshHeightMultiplier * islandData.meshHeightCurve.Evaluate(1);
        }
    }

    private void Awake()
    {
        islandTile = GetComponent<IslandTile>();

        islandWidth = WorldGenerator.tileSize / 2;
        islandHeight = WorldGenerator.tileSize / 2;
        seed = islandTile.UniqueIslandData.Seed;
    }

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(islandWidth, islandHeight, seed, islandTile.UniqueIslandData.NoiseScale, islandTile.UniqueIslandData.Octaves, islandTile.UniqueIslandData.Persistance, islandData.lacunarity, islandTile.UniqueIslandData.Offset);

        for(int y = 0; y < islandHeight; y++)
        {
            for(int x = 0; x < islandWidth; x++)
            {
                //makes map island-like
                noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x,y]);
            }
        }

        UpdateMeshHeights(islandData.terrainMaterial, MinHeight, MaxHeight);

        MapDisplay display = FindObjectOfType<MapDisplay>();

        display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, islandData.meshHeightMultiplier, islandData.meshHeightCurve));
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
        
        ApplyToMaterial(islandData.terrainMaterial);

        falloffMap = FallOffGenerator.GenerateFalloffMap(islandWidth, islandHeight);


    }

    private void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
    {
        material.SetFloat("minHeight", minHeight);
        material.SetFloat("maxHeight", maxHeight);
    }

    private void ApplyToMaterial(Material material)
    {
        material.SetColorArray("baseColours", islandData.baseColours);
        material.SetFloatArray("baseStartHeights", islandData.baseStartHeights);
        material.SetFloatArray("baseBlends", islandData.baseBlends);
        material.SetInt("baseColourCount", islandData.baseColours.Length);

        UpdateMeshHeights(material, MinHeight, MaxHeight);
    }

    public void DetermineSize()
    {
        switch (islandTile.Size)
        {
            case IslandTile.IslandSize.Long:
                islandWidth *= 2;
                break;

            case IslandTile.IslandSize.Tall:
                islandHeight *= 2;
                break;

            case IslandTile.IslandSize.Large:
                islandWidth *= 2;
                islandHeight *= 2;
                break;
        }
        falloffMap = FallOffGenerator.GenerateFalloffMap(islandWidth, islandHeight);
    }
    
}


