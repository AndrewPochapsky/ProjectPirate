using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise  {

    public static float [,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        //create noisemap array
        float[,] noiseMap = new float[mapWidth, mapHeight];

        //pseudo random number generator
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for(int i = 0; i < octaves; i++)
        {
            //offsets allow scrolling
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        //prevent scale from being 0
        if(scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        //zoom in on centre instead of top right corner
        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for(int y = 0; y < mapHeight; y++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequencey = 1;
                float noiseHeight = 0;

                for(int i = 0; i < octaves; i++)
                {

                    float sampleX = (x - halfWidth) / scale * frequencey + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / scale * frequencey + octaveOffsets[i].y;

                    //range is -1 to 1
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    //amplitude decreases each octave
                    amplitude *= persistance;

                    //frequency increases each octave
                    frequencey *= lacunarity;
                }

                if(noiseHeight > maxNoiseHeight)
                {
                    maxNoiseHeight = noiseHeight;
                }
                else if(noiseHeight < minNoiseHeight)
                {
                    minNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }

        //Normalize the map(back to values between 0 and 1
        for(int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}
