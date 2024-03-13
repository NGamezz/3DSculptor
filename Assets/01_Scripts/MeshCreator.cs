using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

//Original Version Was Created By Sebastian Lague, and can be found in the "External" Folder under the name of "GenTest".
public class MeshCreator : MonoBehaviour
{
    [Header("Init Settings")]
    public int numChunks = 4;

    public int numPointsPerAxis = 10;
    public float boundsSize = 10;
    public float isoLevel = 0f;
    public bool useFlatShading;

    [Header("References")]
    public ComputeShader meshCompute;
    public ComputeShader densityCompute;
    public ComputeShader editCompute;
    public Material material;

    private List<GameObject> meshHolders = new();

    private ComputeBuffer triangleBuffer;
    private ComputeBuffer triCountBuffer;
    private Chunk[] chunks;
    private RenderTexture rawDensityTexture;

    private VertexData[] vertexDataArray;

    void Start ()
    {
        Run();
    }

    public RenderTexture GetRenderTexture ()
    {
        return rawDensityTexture;
    }

    private void Run ( bool load = false )
    {
        InitTextures(load);

        if ( (triangleBuffer == null || triCountBuffer == null || vertexDataArray == null) || (triangleBuffer.IsValid() == false || triangleBuffer.IsValid() == false) )
        {
            Debug.Log("Creating Buffers.");
            CreateBuffers();
        }

        if ( chunks == null || chunks.Length < math.pow(numChunks, 3) )
        {
            Debug.Log("Creating Chunks.");
            CreateChunks();
        }

        if ( !load )
        {
            ComputeDensity();
        }

        GenerateAllChunks();
    }

    void InitTextures ( bool load = false )
    {
        if ( !load )
        {
            Debug.Log("Creating Texture.");
            int size = numChunks * (numPointsPerAxis - 1) + 1;
            Create3DTexture(ref rawDensityTexture, size, "Raw Density Texture");
        }

        densityCompute.SetTexture(0, "DensityTexture", rawDensityTexture);
        editCompute.SetTexture(0, "EditTexture", rawDensityTexture);
        meshCompute.SetTexture(0, "DensityTexture", rawDensityTexture);
    }

    void GenerateAllChunks ()
    {
        for ( int i = 0; i < chunks.Length; i++ )
        {
            GenerateChunk(chunks[i]);
        }
    }

    private void DeleteChunks ()
    {
        if ( meshHolders.Count < 1 )
            return;

        int amount = numChunks * numChunks * numChunks;

        for ( int i = amount - 1; i > 0; i-- )
        {
            if ( i < chunks.Length )
            {
                var chunk = chunks[i];
                chunk.Release();
            }

            if ( i < meshHolders.Count )
            {
                var meshHolder = meshHolders[i];
                if ( meshHolder == null )
                {
                    meshHolders.RemoveAt(i);
                    continue;
                }

                meshHolders.Remove(meshHolder);
                Destroy(meshHolder);
            }
        }
    }

    public Mesh GetMesh ()
    {
        return default;
    }

    public void LoadVertices ( SaveData<float[], int3> saveData )
    {
        var dimensions = saveData.dataB;
        Texture3D depthTexture = new(dimensions.x, dimensions.y, dimensions.z, TextureFormat.RFloat, false);

        depthTexture.SetPixelData(saveData.data, 0);
        depthTexture.Apply();

        Graphics.CopyTexture(depthTexture, rawDensityTexture);

        Destroy(depthTexture);

        Debug.Log("Loaded.");

        Run(true);
    }

    void ComputeDensity ()
    {
        Debug.Log("Compute Density.");

        // Get points (each point is a vector4: xyz = position, w = density)
        int textureSize = rawDensityTexture.width;

        densityCompute.SetInt("textureSize", textureSize);
        densityCompute.SetFloat("planetSize", boundsSize);

        ComputeHelper.Dispatch(densityCompute, textureSize, textureSize, textureSize);
    }

    void GenerateChunk ( Chunk chunk )
    {
        // Marching cubes
        int numVoxelsPerAxis = numPointsPerAxis - 1;
        int marchKernel = 0;

        meshCompute.SetInt("textureSize", rawDensityTexture.width);
        meshCompute.SetInt("numPointsPerAxis", numPointsPerAxis);
        meshCompute.SetFloat("isoLevel", isoLevel);
        meshCompute.SetFloat("planetSize", boundsSize);
        triangleBuffer.SetCounterValue(0);
        meshCompute.SetBuffer(marchKernel, "triangles", triangleBuffer);

        Vector3 chunkCoord = (Vector3)chunk.id * (numPointsPerAxis - 1);
        meshCompute.SetVector("chunkCoord", chunkCoord);

        ComputeHelper.Dispatch(meshCompute, numVoxelsPerAxis, numVoxelsPerAxis, numVoxelsPerAxis, marchKernel);

        // Create mesh
        int[] vertexCountData = new int[1];
        triCountBuffer.SetData(vertexCountData);
        ComputeBuffer.CopyCount(triangleBuffer, triCountBuffer, 0);
        triCountBuffer.GetData(vertexCountData);

        int numVertices = vertexCountData[0] * 3;

        // Fetch vertex data from GPU
        triangleBuffer.GetData(vertexDataArray, 0, 0, numVertices);

        chunk.CreateMesh(vertexDataArray, numVertices, useFlatShading);
    }

    void CreateBuffers ()
    {
        int numVoxelsPerAxis = numPointsPerAxis - 1;
        int numVoxels = numVoxelsPerAxis * numVoxelsPerAxis * numVoxelsPerAxis;
        int maxTriangleCount = numVoxels * 5;
        int maxVertexCount = maxTriangleCount * 3;

        triCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
        triangleBuffer = new ComputeBuffer(maxVertexCount, ComputeHelper.GetStride<VertexData>(), ComputeBufferType.Append);
        vertexDataArray = new VertexData[maxVertexCount];
    }

    void ReleaseBuffers ()
    {
        ComputeHelper.Release(triangleBuffer, triCountBuffer);
    }

    void OnDestroy ()
    {
        ReleaseBuffers();
        DeleteChunks();
    }

    void CreateChunks ()
    {
        chunks = new Chunk[numChunks * numChunks * numChunks];
        float chunkSize = (boundsSize) / numChunks;
        int i = 0;

        for ( int y = 0; y < numChunks; y++ )
        {
            for ( int x = 0; x < numChunks; x++ )
            {
                for ( int z = 0; z < numChunks; z++ )
                {
                    float posX = (-(numChunks - 1f) / 2 + x) * chunkSize;
                    float posY = (-(numChunks - 1f) / 2 + y) * chunkSize;
                    float posZ = (-(numChunks - 1f) / 2 + z) * chunkSize;
                    Vector3 centre = new(posX, posY, posZ);

                    GameObject meshHolder = new($"Chunk ({x}, {y}, {z})");
                    meshHolder.transform.parent = transform;
                    meshHolder.layer = gameObject.layer;

                    meshHolders.Add(meshHolder);

                    Chunk chunk = new(new Vector3Int(x, y, z), centre, chunkSize, numPointsPerAxis, meshHolder);
                    chunk.SetMaterial(material);
                    chunks[i] = chunk;
                    i++;
                }
            }
        }
    }

    private Dictionary<Vector3, Chunk> chunkDictionary = new();

    //Todo: Potentially optimize it so it doesn't have to check every chunk for a sphere intersection.
    public void Terraform ( Vector3 point, float weight, float radius )
    {
        int editTextureSize = rawDensityTexture.width;
        float editPixelWorldSize = boundsSize / editTextureSize;
        int editRadius = Mathf.CeilToInt(radius / editPixelWorldSize);

        float tx = Mathf.Clamp01((point.x + boundsSize / 2) / boundsSize);
        float ty = Mathf.Clamp01((point.y + boundsSize / 2) / boundsSize);
        float tz = Mathf.Clamp01((point.z + boundsSize / 2) / boundsSize);

        int editX = Mathf.RoundToInt(tx * (editTextureSize - 1));
        int editY = Mathf.RoundToInt(ty * (editTextureSize - 1));
        int editZ = Mathf.RoundToInt(tz * (editTextureSize - 1));

        editCompute.SetFloat("weight", weight);
        editCompute.SetFloat("deltaTime", Time.deltaTime);
        editCompute.SetInts("brushCentre", editX, editY, editZ);
        editCompute.SetInt("brushRadius", editRadius);

        editCompute.SetInt("size", editTextureSize);
        ComputeHelper.Dispatch(editCompute, editTextureSize, editTextureSize, editTextureSize);

        float worldRadius = (editRadius + 1) * editPixelWorldSize;
        for ( int i = 0; i < chunks.Length; i++ )
        {
            var chunk = chunks[i];
            if ( MathUtility.SphereIntersectsBox(point, worldRadius, chunk.centre, Vector3.one * chunk.size) )
            {
                chunk.terra = true;
                GenerateChunk(chunk);
            }
        }
    }

    void Create3DTexture ( ref RenderTexture texture, int size, string name )
    {
        var format = UnityEngine.Experimental.Rendering.GraphicsFormat.R32_SFloat;
        if ( texture == null || !texture.IsCreated() || texture.width != size || texture.height != size || texture.volumeDepth != size || texture.graphicsFormat != format )
        {
            if ( texture != null )
            {
                texture.Release();
            }

            texture = new(size, size, 0)
            {
                volumeDepth = size,
                graphicsFormat = format,
                enableRandomWrite = true,
                dimension = TextureDimension.Tex3D
            };

            texture.Create();
        }
        texture.wrapMode = TextureWrapMode.Repeat;
        texture.filterMode = FilterMode.Bilinear;
        texture.name = name;
    }
}