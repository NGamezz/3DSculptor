using UnityEngine;

/// <summary>
/// Creates a two dimensional float array that stores the height values for the vertices, these height values are determined using a multitude of peramiters paired with perlin noise.
/// </summary>
public static class GenerateNoise
{
    public static float[,] CreateNoiseMap(int mapSize, int octaves, float scale, int seed, float persistance, float lacunarity, bool randomSeed)
    {
        float[,] noiseMap = new float[mapSize + 1, mapSize + 1];

        ///Used for randomizing the seed.
        int seedValue = randomSeed ? Random.Range(0, 100000) : seed;
        Debug.Log(seedValue);

        System.Random random = new(seedValue);

        float offSetX = random.Next(-100000, 100000);
        float offSetZ = random.Next(-100000, 100000);
        Vector2 octaveOffsets = new(offSetX, offSetZ);
        ///

        for (int z = 0; z <= mapSize; z++)
        {
            for (int x = 0; x <= mapSize; x++)
            {
                float frequency = 1;
                float amplitude = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = x / scale * frequency + octaveOffsets.x;
                    float sampleZ = z / scale * frequency + octaveOffsets.y;

                    float y = Mathf.PerlinNoise(sampleX, sampleZ);

                    noiseHeight += y * amplitude;

                    //Amplitude decreases each octave while the frequency increases.
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                noiseMap[x, z] = noiseHeight;
            }
        }
        return noiseMap;
    }
}
