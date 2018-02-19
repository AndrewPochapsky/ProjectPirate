using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PersistantIslandData : ScriptableObject {

    public float lacunarity;

    public float meshHeightMultiplier;

    public AnimationCurve meshHeightCurve;

    public bool autoUpdate;

    public Material terrainMaterial;

    public Color[] baseColours;

    [Range(0, 1)]
    public float[] baseStartHeights;

    [Range(0, 1)]
    public float[] baseBlends;
}
