using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class IslandGenerator : MonoBehaviour {

    private float islandWidth;

    private float islandHeight;

    private int seed;

    [SerializeField]
    private PersistantIslandData islandData;

    private IslandTile islandTile;

    private float[,] falloffMap;

    const float constantValue = 20f;

    private float minHeight;
    private float maxHeight;

    private void Awake()
    {
        islandTile = GetComponent<IslandTile>();

        minHeight = (constantValue / 5) * islandData.meshHeightMultiplier * islandData.meshHeightCurve.Evaluate(0);
        maxHeight = constantValue * islandData.meshHeightMultiplier * islandData.meshHeightCurve.Evaluate(1);

        islandWidth = WorldController.mapTileSize / 2;
        islandHeight = WorldController.mapTileSize / 2;
        seed = islandTile.UniqueIslandData.Seed;
    }

    /// <summary>
    /// Generates the island
    /// </summary>
    public void GenerateIsland()
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

        UpdateMeshHeights(islandData.terrainMaterial, minHeight, maxHeight);

        MapDisplay display = FindObjectOfType<MapDisplay>();

        display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, islandData.meshHeightMultiplier, islandData.meshHeightCurve));

        ApplyToMaterial(islandData.terrainMaterial);
    }

    /// <summary>
    /// Sets the height variables in material(shader)
    /// </summary>
    /// <param name="material">The material</param>
    /// <param name="minHeight">The minHeight</param>
    /// <param name="maxHeight">The maxHeight</param>
    private void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
    {
        material.SetFloat("minHeight", minHeight);
        material.SetFloat("maxHeight", maxHeight);
    }

    /// <summary>
    /// Applies various properties to the material(shader)
    /// </summary>
    /// <param name="material"></param>
    private void ApplyToMaterial(Material material)
    {
        material.SetColorArray("baseColours", islandData.baseColours);
        material.SetFloatArray("baseStartHeights", islandData.baseStartHeights);
        material.SetFloatArray("baseBlends", islandData.baseBlends);
        material.SetInt("baseColourCount", islandData.baseColours.Length);

        UpdateMeshHeights(material, minHeight, maxHeight);
    }

    /// <summary>
    /// Determines the size of the island based on its IslandSize value
    /// </summary>
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


