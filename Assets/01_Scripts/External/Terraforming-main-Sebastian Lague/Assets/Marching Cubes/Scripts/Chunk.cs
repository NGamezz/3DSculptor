using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;

//By Sebastian Lague.
public class Chunk
{
    public Vector3 Centre;
    public float Size;
    public Mesh Mesh;

    public ComputeBuffer PointsBuffer;
    public MeshFilter Filter;
    public bool Terra;
    public Vector3Int Id;

    private MeshRenderer Renderer;
    private MeshCollider Collider;

    // Mesh processing
    private Dictionary<int2, int> vertexIndexMap;
    private List<Vector3> processedVertices;
    private List<Vector3> processedNormals;
    private List<int> processedTriangles;

    public Chunk(Vector3Int coord, Vector3 centre, float size, int numPointsPerAxis, GameObject meshHolder)
    {
        this.Id = coord;
        this.Centre = centre;
        this.Size = size;

        Mesh = new Mesh
        {
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
        };

        int numPointsTotal = numPointsPerAxis * numPointsPerAxis * numPointsPerAxis;
        ComputeHelper.CreateStructuredBuffer<PointData>(ref PointsBuffer, numPointsTotal);

        // Mesh rendering and collision components
        Filter = meshHolder.AddComponent<MeshFilter>();
        Renderer = meshHolder.AddComponent<MeshRenderer>();

        Filter.mesh = Mesh;
        Collider = Renderer.gameObject.AddComponent<MeshCollider>();

        vertexIndexMap = new Dictionary<int2, int>();
        processedVertices = new List<Vector3>();
        processedNormals = new List<Vector3>();
        processedTriangles = new List<int>();
    }

    public void CreateMesh(VertexData[] vertexData, int numVertices, bool useFlatShading)
    {
        vertexIndexMap.Clear();
        processedVertices.Clear();
        processedNormals.Clear();
        processedTriangles.Clear();

        int triangleIndex = 0;

        for (int i = 0; i < numVertices; i++)
        {
            VertexData data = vertexData[i];

            if (!useFlatShading && vertexIndexMap.TryGetValue(data.Id, out int sharedVertexIndex))
            {
                processedTriangles.Add(sharedVertexIndex);
            }
            else
            {
                if (!useFlatShading)
                {
                    vertexIndexMap.Add(data.Id, triangleIndex);
                }
                processedVertices.Add(data.Position);
                processedNormals.Add(data.Normal);
                processedTriangles.Add(triangleIndex);
                triangleIndex++;
            }
        }

        Collider.sharedMesh = null;

        Mesh.Clear();
        Mesh.SetVertices(processedVertices);
        Mesh.SetTriangles(processedTriangles, 0, true);

        if (useFlatShading)
        {
            Mesh.RecalculateNormals();
        }
        else
        {
            Mesh.SetNormals(processedNormals);
        }

        Collider.sharedMesh = Mesh;
    }

    public struct PointData
    {
        public Vector3 position;
        public Vector3 normal;
        public float density;
    }

    public void AddCollider()
    {
        Collider.sharedMesh = Mesh;
    }

    public void SetMaterial(Material material)
    {
        Renderer.material = material;
    }

    public void Release()
    {
        ComputeHelper.Release(PointsBuffer);
    }

    public void DrawBoundsGizmo(Color col)
    {
        Gizmos.color = col;
        Gizmos.DrawWireCube(Centre, Vector3.one * Size);
    }
}