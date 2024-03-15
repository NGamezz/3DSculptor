using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

//Original Version Was Created By Sebastian Lague, and can be found at the bottom of this file or on https://github.com/SebLague/Terraforming/tree/main.
public class MeshCreator : MonoBehaviour
{
    [Header("Init Settings")]
    public int numChunks = 4;

    public int numPointsPerAxis = 10;
    public float boundsSize = 10;
    public float isoLevel = 0f;
    public bool useFlatShading;

    [Header("References")]
    [SerializeField] private ComputeShader meshCompute;
    [SerializeField] private ComputeShader densityCompute;
    [SerializeField] private ComputeShader editCompute;
    [SerializeField] private ComputeShader setFloatsCompute;
    [SerializeField] private ComputeShader readFloatDataCompute;
    public Material material;

    private List<GameObject> meshHolders = new();

    private ComputeBuffer triangleBuffer;
    private ComputeBuffer triCountBuffer;
    private ComputeBuffer floatCountBuffer;
    private ComputeBuffer floatBuffer;
    private ComputeBuffer floatDataBuffer;
    private Chunk[] chunks;
    private RenderTexture rawDensityTexture;

    private VertexData[] vertexDataArray;

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

        DataHolder.TextPopupManager.QueuePopup(new(2, "Loaded Data. Reconstructing."));

        Run(true);
    }

    public RenderTexture GetRenderTexture ()
    {
        return rawDensityTexture;
    }

    public void CreateNew ()
    {
        Run(false);
    }

    private void Start ()
    {
        Run();
    }

    private void Run ( bool load = false )
    {
        InitTextures(load);

        if ( triangleBuffer == null || triCountBuffer == null || vertexDataArray == null || triangleBuffer.IsValid() == false || triangleBuffer.IsValid() == false )
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

        readFloatDataCompute.SetTexture(0, "EditTexture", rawDensityTexture);
        setFloatsCompute.SetTexture(0, "EditTexture", rawDensityTexture);
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

    private void ComputeDensity ()
    {
        Debug.Log("Compute Density.");

        // Get points (each point is a vector4: xyz = position, w = density)
        int textureSize = rawDensityTexture.width;

        densityCompute.SetInt("textureSize", textureSize);
        densityCompute.SetFloat("planetSize", boundsSize);

        ComputeHelper.Dispatch(densityCompute, textureSize, textureSize, textureSize);
    }

    private void GenerateChunk ( Chunk chunk )
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

    private void CreateBuffers ()
    {
        int numVoxelsPerAxis = numPointsPerAxis - 1;
        int numVoxels = numVoxelsPerAxis * numVoxelsPerAxis * numVoxelsPerAxis;
        int maxTriangleCount = numVoxels * 5;
        int maxVertexCount = maxTriangleCount * 3;

        triCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
        triangleBuffer = new ComputeBuffer(maxVertexCount, ComputeHelper.GetStride<VertexData>(), ComputeBufferType.Append);
        vertexDataArray = new VertexData[maxVertexCount];
        //floatCountBuffer = new(1, sizeof(int), ComputeBufferType.Raw);
        //floatBuffer = new(maxVertexCount, sizeof(float), ComputeBufferType.Append);
        //floatDataBuffer = new(maxVertexCount, sizeof(float), ComputeBufferType.Default);
    }

    private void ReleaseBuffers ()
    {
        ComputeHelper.Release(triangleBuffer, triCountBuffer, floatBuffer, floatCountBuffer, floatDataBuffer);
    }

    private void OnDestroy ()
    {
        ReleaseBuffers();
        DeleteChunks();
    }

    private void CreateChunks ()
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

                    Chunk chunk = new(new(x, y, z), centre, chunkSize, numPointsPerAxis, meshHolder);
                    chunk.SetMaterial(material);
                    chunks[i] = chunk;
                    i++;
                }
            }
        }
    }

    //Todo: Potentially optimize it so it doesn't have to check every chunk for a sphere intersection.
    public void AlterModel ( Vector3 point, float weight, float radius )
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

    #region testing
    //private void FixedUpdate ()
    //{
    //    if ( editHistory.Count > 0 && Input.GetKey(KeyCode.Z) )
    //    {
    //        Undo();
    //    }
    //}

    //Work In Progress/ Testing.
    //private void Undo ()
    //{
    //Debug.Log("Undo.");

    //var currentData = editHistory.Pop();

    //Debug.Log(currentData.changedValues.Length);

    //var editSize = rawDensityTexture.width;
    //float editPixelWorldSize = boundsSize / editSize;
    //int editRadius = currentData.radius;

    //floatBuffer.SetData(currentData.changedValues);

    //setFloatsCompute.SetBuffer(0, "changedFloats", floatBuffer);
    //setFloatsCompute.SetInts("brushCentre", currentData.position.x, currentData.position.y, currentData.position.z);
    //setFloatsCompute.SetInt("brushRadius", currentData.radius);
    //setFloatsCompute.SetInt("size", editSize);

    //ComputeHelper.Dispatch(setFloatsCompute, editSize, editSize, editSize);

    //float worldRadius = (editRadius + 1) * editPixelWorldSize;
    //for ( int i = 0; i < chunks.Length; i++ )
    //{
    //    var chunk = chunks[i];
    //    if ( MathUtility.SphereIntersectsBox(currentData.point, worldRadius, chunk.centre, Vector3.one * chunk.size) )
    //    {
    //        chunk.terra = true;
    //        GenerateChunk(chunk);
    //    }
    //}
    //}

    //private struct ActionData
    //{
    //    public int radius;
    //    public int3 position;
    //    public Vector3 point;
    //    public float[] changedValues;
    //}

    //private Stack<ActionData> editHistory = new();
    #endregion

    private void Create3DTexture ( ref RenderTexture texture, int size, string name )
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

//Original ->
//using System.Collections.Generic;
//using Unity.Collections;
//using Unity.Mathematics;
//using UnityEngine;
//using UnityEngine.Rendering;

//public class GenTest : MonoBehaviour
//{

//    [Header("Init Settings")]
//    public int numChunks = 4;

//    public int numPointsPerAxis = 10;
//    public float boundsSize = 10;
//    public float isoLevel = 0f;
//    public bool useFlatShading;

//    public float noiseScale;
//    public float noiseHeightMultiplier;
//    public bool blurMap;
//    public int blurRadius = 3;

//    [Header("References")]
//    public ComputeShader meshCompute;
//    public ComputeShader densityCompute;
//    public ComputeShader blurCompute;
//    public ComputeShader editCompute;
//    public Material material;


//    // Private
//    ComputeBuffer triangleBuffer;
//    ComputeBuffer triCountBuffer;
//    [HideInInspector] public RenderTexture rawDensityTexture;
//    [HideInInspector] public RenderTexture processedDensityTexture;
//    Chunk[] chunks;

//    VertexData[] vertexDataArray;

//    int totalVerts;

//    // Stopwatches
//    System.Diagnostics.Stopwatch timer_fetchVertexData;
//    System.Diagnostics.Stopwatch timer_processVertexData;
//    RenderTexture originalMap;

//    void Start ()
//    {
//        InitTextures();
//        CreateBuffers();

//        CreateChunks();

//        var sw = System.Diagnostics.Stopwatch.StartNew();
//        GenerateAllChunks();
//        Debug.Log("Generation Time: " + sw.ElapsedMilliseconds + " ms");

//        ComputeHelper.CreateRenderTexture3D(ref originalMap, processedDensityTexture);
//        ComputeHelper.CopyRenderTexture3D(processedDensityTexture, originalMap);

//    }

//    void InitTextures ()
//    {

//        // Explanation of texture size:
//        // Each pixel maps to one point.
//        // Each chunk has "numPointsPerAxis" points along each axis
//        // The last points of each chunk overlap in space with the first points of the next chunk
//        // Therefore we need one fewer pixel than points for each added chunk
//        int size = numChunks * (numPointsPerAxis - 1) + 1;
//        Create3DTexture(ref rawDensityTexture, size, "Raw Density Texture");
//        Create3DTexture(ref processedDensityTexture, size, "Processed Density Texture");

//        if ( !blurMap )
//        {
//            processedDensityTexture = rawDensityTexture;
//        }

//        // Set textures on compute shaders
//        densityCompute.SetTexture(0, "DensityTexture", rawDensityTexture);
//        editCompute.SetTexture(0, "EditTexture", rawDensityTexture);
//        blurCompute.SetTexture(0, "Source", rawDensityTexture);
//        blurCompute.SetTexture(0, "Result", processedDensityTexture);
//        meshCompute.SetTexture(0, "DensityTexture", (blurCompute) ? processedDensityTexture : rawDensityTexture);
//    }

//    void GenerateAllChunks ()
//    {
//        // Create timers:
//        timer_fetchVertexData = new System.Diagnostics.Stopwatch();
//        timer_processVertexData = new System.Diagnostics.Stopwatch();

//        totalVerts = 0;
//        ComputeDensity();


//        for ( int i = 0; i < chunks.Length; i++ )
//        {
//            GenerateChunk(chunks[i]);
//        }
//        Debug.Log("Total verts " + totalVerts);

//        // Print timers:
//        Debug.Log("Fetch vertex data: " + timer_fetchVertexData.ElapsedMilliseconds + " ms");
//        Debug.Log("Process vertex data: " + timer_processVertexData.ElapsedMilliseconds + " ms");
//        Debug.Log("Sum: " + (timer_fetchVertexData.ElapsedMilliseconds + timer_processVertexData.ElapsedMilliseconds));


//    }

//    void ComputeDensity ()
//    {
//        // Get points (each point is a vector4: xyz = position, w = density)
//        int textureSize = rawDensityTexture.width;

//        densityCompute.SetInt("textureSize", textureSize);

//        densityCompute.SetFloat("planetSize", boundsSize);
//        densityCompute.SetFloat("noiseHeightMultiplier", noiseHeightMultiplier);
//        densityCompute.SetFloat("noiseScale", noiseScale);

//        ComputeHelper.Dispatch(densityCompute, textureSize, textureSize, textureSize);

//        ProcessDensityMap();
//    }

//    void ProcessDensityMap ()
//    {
//        if ( blurMap )
//        {
//            int size = rawDensityTexture.width;
//            blurCompute.SetInts("brushCentre", 0, 0, 0);
//            blurCompute.SetInt("blurRadius", blurRadius);
//            blurCompute.SetInt("textureSize", rawDensityTexture.width);
//            ComputeHelper.Dispatch(blurCompute, size, size, size);
//        }
//    }

//    void GenerateChunk ( Chunk chunk )
//    {


//        // Marching cubes
//        int numVoxelsPerAxis = numPointsPerAxis - 1;
//        int marchKernel = 0;


//        meshCompute.SetInt("textureSize", processedDensityTexture.width);
//        meshCompute.SetInt("numPointsPerAxis", numPointsPerAxis);
//        meshCompute.SetFloat("isoLevel", isoLevel);
//        meshCompute.SetFloat("planetSize", boundsSize);
//        triangleBuffer.SetCounterValue(0);
//        meshCompute.SetBuffer(marchKernel, "triangles", triangleBuffer);

//        Vector3 chunkCoord = (Vector3)chunk.id * (numPointsPerAxis - 1);
//        meshCompute.SetVector("chunkCoord", chunkCoord);

//        ComputeHelper.Dispatch(meshCompute, numVoxelsPerAxis, numVoxelsPerAxis, numVoxelsPerAxis, marchKernel);

//        // Create mesh
//        int[] vertexCountData = new int[1];
//        triCountBuffer.SetData(vertexCountData);
//        ComputeBuffer.CopyCount(triangleBuffer, triCountBuffer, 0);

//        timer_fetchVertexData.Start();
//        triCountBuffer.GetData(vertexCountData);

//        int numVertices = vertexCountData[0] * 3;

//        // Fetch vertex data from GPU

//        triangleBuffer.GetData(vertexDataArray, 0, 0, numVertices);

//        timer_fetchVertexData.Stop();

//        //CreateMesh(vertices);
//        timer_processVertexData.Start();
//        chunk.CreateMesh(vertexDataArray, numVertices, useFlatShading);
//        timer_processVertexData.Stop();
//    }

//    void Update ()
//    {

//        // TODO: move somewhere more sensible
//        material.SetTexture("DensityTex", originalMap);
//        //material.SetFloat("oceanRadius", FindObjectOfType<Water>().radius);
//        material.SetFloat("planetBoundsSize", boundsSize);

//        /*
//		if (Input.GetKeyDown(KeyCode.G))
//		{
//			Debug.Log("Generate");
//			GenerateAllChunks();
//		}
//		*/
//    }



//    void CreateBuffers ()
//    {
//        int numPoints = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis;
//        int numVoxelsPerAxis = numPointsPerAxis - 1;
//        int numVoxels = numVoxelsPerAxis * numVoxelsPerAxis * numVoxelsPerAxis;
//        int maxTriangleCount = numVoxels * 5;
//        int maxVertexCount = maxTriangleCount * 3;

//        triCountBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Raw);
//        triangleBuffer = new ComputeBuffer(maxVertexCount, ComputeHelper.GetStride<VertexData>(), ComputeBufferType.Append);
//        vertexDataArray = new VertexData[maxVertexCount];
//    }

//    void ReleaseBuffers ()
//    {
//        ComputeHelper.Release(triangleBuffer, triCountBuffer);
//    }

//    void OnDestroy ()
//    {
//        ReleaseBuffers();
//        foreach ( Chunk chunk in chunks )
//        {
//            chunk.Release();
//        }
//    }


//    void CreateChunks ()
//    {
//        chunks = new Chunk[numChunks * numChunks * numChunks];
//        float chunkSize = (boundsSize) / numChunks;
//        int i = 0;

//        for ( int y = 0; y < numChunks; y++ )
//        {
//            for ( int x = 0; x < numChunks; x++ )
//            {
//                for ( int z = 0; z < numChunks; z++ )
//                {
//                    Vector3Int coord = new Vector3Int(x, y, z);
//                    float posX = (-(numChunks - 1f) / 2 + x) * chunkSize;
//                    float posY = (-(numChunks - 1f) / 2 + y) * chunkSize;
//                    float posZ = (-(numChunks - 1f) / 2 + z) * chunkSize;
//                    Vector3 centre = new Vector3(posX, posY, posZ);

//                    GameObject meshHolder = new GameObject($"Chunk ({x}, {y}, {z})");
//                    meshHolder.transform.parent = transform;
//                    meshHolder.layer = gameObject.layer;

//                    Chunk chunk = new Chunk(coord, centre, chunkSize, numPointsPerAxis, meshHolder);
//                    chunk.SetMaterial(material);
//                    chunks[i] = chunk;
//                    i++;
//                }
//            }
//        }
//    }


//    public void Terraform ( Vector3 point, float weight, float radius )
//    {

//        int editTextureSize = rawDensityTexture.width;
//        float editPixelWorldSize = boundsSize / editTextureSize;
//        int editRadius = Mathf.CeilToInt(radius / editPixelWorldSize);
//        //Debug.Log(editPixelWorldSize + "  " + editRadius);

//        float tx = Mathf.Clamp01((point.x + boundsSize / 2) / boundsSize);
//        float ty = Mathf.Clamp01((point.y + boundsSize / 2) / boundsSize);
//        float tz = Mathf.Clamp01((point.z + boundsSize / 2) / boundsSize);

//        int editX = Mathf.RoundToInt(tx * (editTextureSize - 1));
//        int editY = Mathf.RoundToInt(ty * (editTextureSize - 1));
//        int editZ = Mathf.RoundToInt(tz * (editTextureSize - 1));

//        editCompute.SetFloat("weight", weight);
//        editCompute.SetFloat("deltaTime", Time.deltaTime);
//        editCompute.SetInts("brushCentre", editX, editY, editZ);
//        editCompute.SetInt("brushRadius", editRadius);

//        editCompute.SetInt("size", editTextureSize);
//        ComputeHelper.Dispatch(editCompute, editTextureSize, editTextureSize, editTextureSize);

//        //ProcessDensityMap();
//        int size = rawDensityTexture.width;

//        if ( blurMap )
//        {
//            blurCompute.SetInt("textureSize", rawDensityTexture.width);
//            blurCompute.SetInts("brushCentre", editX - blurRadius - editRadius, editY - blurRadius - editRadius, editZ - blurRadius - editRadius);
//            blurCompute.SetInt("blurRadius", blurRadius);
//            blurCompute.SetInt("brushRadius", editRadius);
//            int k = (editRadius + blurRadius) * 2;
//            ComputeHelper.Dispatch(blurCompute, k, k, k);
//        }

//        //ComputeHelper.CopyRenderTexture3D(originalMap, processedDensityTexture);

//        float worldRadius = (editRadius + 1 + ((blurMap) ? blurRadius : 0)) * editPixelWorldSize;
//        for ( int i = 0; i < chunks.Length; i++ )
//        {
//            Chunk chunk = chunks[i];
//            if ( MathUtility.SphereIntersectsBox(point, worldRadius, chunk.centre, Vector3.one * chunk.size) )
//            {

//                chunk.terra = true;
//                GenerateChunk(chunk);

//            }
//        }
//    }

//    void Create3DTexture ( ref RenderTexture texture, int size, string name )
//    {
//        //
//        var format = UnityEngine.Experimental.Rendering.GraphicsFormat.R32_SFloat;
//        if ( texture == null || !texture.IsCreated() || texture.width != size || texture.height != size || texture.volumeDepth != size || texture.graphicsFormat != format )
//        {
//            //Debug.Log ("Create tex: update noise: " + updateNoise);
//            if ( texture != null )
//            {
//                texture.Release();
//            }
//            const int numBitsInDepthBuffer = 0;
//            texture = new RenderTexture(size, size, numBitsInDepthBuffer);
//            texture.graphicsFormat = format;
//            texture.volumeDepth = size;
//            texture.enableRandomWrite = true;
//            texture.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;


//            texture.Create();
//        }
//        texture.wrapMode = TextureWrapMode.Repeat;
//        texture.filterMode = FilterMode.Bilinear;
//        texture.name = name;
//    }



//}