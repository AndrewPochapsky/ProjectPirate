using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class IslandData : ScriptableObject {

    public float noiseScale;

    public int octaves;

    [Range(0, 1)]
    public float persistance;

    public float lacunarity;

    public int seed;

    public Vector2 offset;

    public float meshHeightMultiplier;

    public AnimationCurve meshHeightCurve;

    public bool autoUpdate;

    public Material terrainMaterial;

    [SerializeField]
    public Color[] baseColours;

    [Range(0, 1)]
    public float[] baseStartHeights;

    [Range(0, 1)]
    public float[] baseBlends;
}
