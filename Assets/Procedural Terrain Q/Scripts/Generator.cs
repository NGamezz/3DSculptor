using TMPro;
using TreeEditor;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [Tooltip("Amount of iterations used when creating the noiseheight values. Has a pretty significant impact on performance.")]
    [SerializeField] private int octaves = 3;
    [Tooltip("Used to recreate a certain terrain.")]
    [SerializeField] private int seed;
    [Tooltip("The scale of the noise.")]
    [SerializeField] private float scale = .5f;
    [Range(0, 1), Tooltip("Determines how much the amplitude decreases with each octave.")]
    [SerializeField] private float persistance = .5f;

    private MeshCollider MeshCollider;

    [Range(1, 10), Tooltip("A higher lacunarity will result in a higher frequency increase with each octave.")]
    [SerializeField] private float lacunarity = 1f;

    [Tooltip("Multiplier of the y position of vertixes.")]
    [SerializeField] private float heightMultiplier = 4f;

    [Tooltip("If enabled the seed will be random, random seed will be printed in the console.")]
    [SerializeField] private bool randomSeed = false;

    [Tooltip("Enables Bilinear filtering, off means FilterMode Point.")]
    [SerializeField] private bool bilinear = true;

    [Range(0, 8), Tooltip("Level of anisotrophic filtering.")]
    [SerializeField] private int anisoLevel = 8;

    [SerializeField] private Material landscapeMaterial;

    [Tooltip("The gradient used for the colours of the landscape.")]
    [SerializeField] private Gradient gradient;

    private const int mapSize = 240;

    private Vector3[] vertices;

    private Vector2[] uvs;

    private Mesh mesh;

    private int[] triangles;

    void Start()
    {
        mesh = new();
        GetComponentInChildren<MeshFilter>().mesh = mesh;
        MeshCollider = GetComponentInChildren<MeshCollider>();

        CreateMesh();
        UpdateMesh();
    }

    /// <summary>
    /// Creates a grid of vertices, then creates triangles in between those vertices to fill the area. Displaces the vertices on the y-axis using the generated noiseMap.
    /// </summary>
    private void CreateMesh()
    {
        vertices = new Vector3[(mapSize + 1) * (mapSize + 1)];

        triangles = new int[mapSize * mapSize * 6];

        uvs = new Vector2[vertices.Length];

        float[,] noiseMap = GenerateNoise.CreateNoiseMap(mapSize, octaves, scale, seed, persistance, lacunarity, randomSeed);

        for (int i = 0, z = 0; z <= mapSize; z++)
        {
            for (int x = 0; x <= mapSize; x++, i++)
            {
                vertices[i] = new Vector3(x, noiseMap[x, z] * heightMultiplier, z);
            }
        }

        for (int vertice = 0, triangle = 0, x = 0; x < mapSize; x++, vertice++)
        {
            for (int z = 0; z < mapSize; z++)
            {
                triangles[triangle] = vertice;
                triangles[triangle + 1] = vertice + mapSize + 1;
                triangles[triangle + 2] = vertice + 1;
                triangles[triangle + 3] = vertice + 1;
                triangles[triangle + 4] = vertice + mapSize + 1;
                triangles[triangle + 5] = vertice + mapSize + 2;

                vertice++;
                triangle += 6;
            }
        }

        for (int i = 0, z = 0; z <= mapSize; z++)
        {
            for (int x = 0; x <= mapSize; x++)
            {
                uvs[i] = new Vector2((float)x / mapSize, (float)z / mapSize);
                i++;
            }
        }

        CreateTexture(noiseMap);
    }

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.SetUVs(0, uvs);

        mesh.RecalculateNormals();
        MeshCollider.sharedMesh = mesh;
    }

    //Creates a color texture using a gradient and the normalised height values of the noiseMap.
    private void CreateTexture(float[,] noiseMap)
    {
        float maxValue = float.MinValue;
        float minValue = float.MaxValue;

        for (int i = 0; i <= mapSize; i++)
        {
            for (int x = 0; x <= mapSize; x++)
            {
                if (noiseMap[x, i] < minValue)
                {
                    minValue = noiseMap[x, i];
                }
                if (noiseMap[x, i] > maxValue)
                {
                    maxValue = noiseMap[x, i];
                }
            }
        }

        Texture2D textureMap = new(mapSize, mapSize);

        for (int z = 0; z < mapSize; z++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                textureMap.SetPixel(x, z, gradient.Evaluate(Mathf.InverseLerp(minValue, maxValue, noiseMap[x, z])));
            }
        }

        textureMap.anisoLevel = anisoLevel;
        textureMap.filterMode = bilinear ? FilterMode.Bilinear : FilterMode.Point;
        textureMap.wrapMode = TextureWrapMode.Clamp;

        textureMap.Apply();
        landscapeMaterial.mainTexture = textureMap;
    }
}