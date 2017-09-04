using UnityEngine;

public sealed class UniqueIslandData {

    public int Seed { get; private set; }
    public float Persistance { get; private set; }
    public int NoiseScale { get; private set; }
    public int Octaves { get; private set; }
    public Vector2 Offset { get; private set; }

    const int maxSeed = 10000;

    const float minPersistance = 0.4f;
    const float maxPersistance = 0.6f;

    const int minNoiseScale = 8;
    const int maxNoiseScale = 11;

    const int minOctaves = 5;
    const int maxOctaves = 7;

    const int minOffset = -1000;
    const int maxOffset = 1000;

    /// <summary>
    /// Sets all of the island's data
    /// </summary>
    public void Initialize()
    {
        Seed = Random.Range(0, maxSeed);
        Persistance = Random.Range(minPersistance, maxPersistance);
        NoiseScale = Random.Range(minNoiseScale, maxNoiseScale + 1);
        Octaves = Random.Range(minOctaves, maxOctaves + 1);
        Offset = new Vector2(Random.Range(minOffset, maxOffset + 1), Random.Range(minOffset, maxOffset + 1));
    }
}
