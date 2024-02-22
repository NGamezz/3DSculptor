using Unity.Mathematics;
using UnityEngine;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Unity.VisualScripting;
using NUnit.Framework.Constraints;

public struct Triangle
{
    public Vector3[] vertices;

    public Triangle(int _)
    {
        vertices = new Vector3[3];
    }
}

public struct Voxel
{
    public float3 Position;
    public float Value;

    public Voxel(int _)
    {
        Position = float3.zero;
        Value = 0;
    }
}

public struct GridCell
{
    public float4[] Corners;

    public GridCell(int _)
    {
        Corners = new float4[8];
    }

}


public struct Cube
{
    //public Voxel[] Voxels;
    public float4[] Voxels;
    public float3[] edgePoints;
    public Triangle[] Triangles;
    public int TriangleCount;
    public int Index;

    public Cube(int size = 0)
    {
        TriangleCount = 0;
        Voxels = new float4[8];
        edgePoints = new float3[12];
        Triangles = new Triangle[12];
        for (int i = 0; i < 12; i++)
        {
            Triangles[i] = new Triangle(0);
        }
        Index = 0;
    }
}

public class MarchingCubes : MonoBehaviour
{
    [SerializeField] private float floorLevel = 0.5f;
    [SerializeField] private float scale = 0.05f;
    [SerializeField] private int size = 50;

    public T[][][] SetupJaggedArray<T>(int size)
    {
        T[][][] jaggedArray = new T[size][][];
        for (int i = 0; i < size; i++)
        {
            jaggedArray[i] = new T[size][];
            for (int y = 0; y < size; y++)
            {
                jaggedArray[i][y] = new T[size];
            }
        }
        return jaggedArray;
    }

    private List<Vector3> voxels = new();

    void CreateNoiseMap(int size)
    {
        CubeData cubeData = new(size, scale);
        List<Triangle> triangles = new();

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    var cornerPosition = CalculateCubeCorners(new float3(x, y, z));
                    var cube = cubeData.CreateCube(cornerPosition);

                    int cubeIndex = GetIndex(cube);

                    int[] triangulation = new int[16];
                    for (int i = 0; i < 16; i++)
                    {
                        triangulation[i] = TriangulationTable.triangleTable[cubeIndex, i];
                    }

                    for (int i = 0; triangulation[i] != -1; i += 3)
                    {
                        Triangle triangle = new(0);
                        for (int t = 0; t < 3; t++)
                        {
                            int indexA = TriangulationTable.cornerIndexAFromEdge[triangulation[i + t]];
                            int indexB = TriangulationTable.cornerIndexBFromEdge[triangulation[i + t]];
                            triangle.vertices[t] = interpolateVerts(cube.Voxels[indexA], cube.Voxels[indexB]);
                        }
                        triangles.Add(triangle);
                    }
                }
            }
        }

        int amountOfTriangles = triangles.Count;

        Vector3[] _vertices = new Vector3[amountOfTriangles * 3];
        int[] _triangles = new int[amountOfTriangles * 3];

        for (int i = 0; i < amountOfTriangles; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                _triangles[i * 3 + j] = i * 3 + j;
                _vertices[i * 3 + j] = triangles[i].vertices[j];
            }
        }

        Mesh mesh;
        if (!TryGetComponent(out MeshFilter filter))
        {
            filter = gameObject.AddComponent<MeshFilter>();
        }

        if (Application.isEditor)
        {
            if (filter.sharedMesh == null)
            {
                filter.sharedMesh = new();
            }
            mesh = filter.sharedMesh;
        }
        else
        {
            if (filter.mesh == null)
            {
                filter.mesh = new();
            }
            mesh = filter.mesh;
        }
        mesh.Clear();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.name = "MarchOnWards";

        SetMesh(mesh, _vertices, _triangles);
    }

    private int GetIndex(Cube cube)
    {
        int value = 0;
        for (int i = 0; i < 8; i++)
        {
            if (cube.Voxels[i].w < floorLevel)
            {
                value |= 1 << i;
            }
        }
        return value;
    }

    private void DensityFunction(float3 position)
    {
        float density = -position.y;
    }

    float3 interpolateVerts(float4 v1, float4 v2)
    {
        float t = (floorLevel - v1.w) / (v2.w - v1.w);
        return v1.xyz + t * (v2.xyz - v1.xyz);
    }

    private void GenerateFaces(Cube[] cube)
    {


        //for ( int i = 0; TriangulationTable.triangleTable[cube.Index, i] != -1; i += 3 )
        //{
        //    int a = TriangulationTable.cornerIndexAFromEdge[TriangulationTable.triangleTable[cube.Index, i]];
        //    int b = TriangulationTable.cornerIndexBFromEdge[TriangulationTable.triangleTable[cube.Index, i]];

        //    int a1 = TriangulationTable.cornerIndexAFromEdge[TriangulationTable.triangleTable[cube.Index, i + 1]];
        //    int b1 = TriangulationTable.cornerIndexBFromEdge[TriangulationTable.triangleTable[cube.Index, i + 1]];

        //    int a2 = TriangulationTable.cornerIndexAFromEdge[TriangulationTable.triangleTable[cube.Index, i + 2]];
        //    int b2 = TriangulationTable.cornerIndexBFromEdge[TriangulationTable.triangleTable[cube.Index, i + 2]];

        //    Triangle triangle = new(0);
        //    triangle.vertices[0] = InterpolateEdgePosition(floorLevel, cube.Voxels[a], cube.Voxels[b]);
        //    triangle.vertices[1] = InterpolateEdgePosition(floorLevel, cube.Voxels[a1], cube.Voxels[b1]);
        //    triangle.vertices[2] = InterpolateEdgePosition(floorLevel, cube.Voxels[a2], cube.Voxels[b2]);
        //    UnityEngine.Debug.Log(triangle);
        //    triangles.Add(triangle);
        //}
    }

    private void SetMesh(Mesh mesh, Vector3[] vertices, int[] triangles)
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    private void OnDrawGizmos()
    {
        foreach (var voxel in voxels)
        {
            Gizmos.DrawCube(voxel, Vector3.one);
        }
    }

    //Get positions for the cube corners.
    private float3[] CalculateCubeCorners(float3 startPosition)
    {
        float3[] positions = new float3[8];

        positions[0] = startPosition; //0,0,0
        positions[1] = new float3(startPosition.x + 1, startPosition.y, startPosition.z);//1,0,0
        positions[2] = new float3(startPosition.x, startPosition.y, startPosition.z + 1);//0,0,1
        positions[3] = new float3(startPosition.x, startPosition.y + 1, startPosition.z);//0,1,0
        positions[4] = new float3(startPosition.x + 1, startPosition.y + 1, startPosition.z);//1,1,0
        positions[5] = new float3(startPosition.x, startPosition.y + 1, startPosition.z + 1);//0,1,1
        positions[6] = new float3(startPosition.x + 1, startPosition.y + 1, startPosition.z + 1);//1,1,1
        positions[7] = new float3(startPosition.x + 1, startPosition.y, startPosition.z + 1);//1,0,1

        return positions;
    }

    private struct CubeData
    {
        public Cube[] cubes;

        private float Scale;
        private int cubeIndex;

        public CubeData(int vertexSize, float scale)
        {
            this.Scale = scale;
            cubes = new Cube[vertexSize * vertexSize * vertexSize];

            for (int i = 0; i < cubes.Length; i++)
            {
                cubes[i] = new Cube(0);
            }

            cubeIndex = 0;
        }

        public Cube CreateCube(float3[] cornerPositions)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector3 v = cornerPositions[i];
                cubes[cubeIndex].Voxels[i] = new float4(v.x, v.y, v.z, Perlin3D.Get3DNoise(cornerPositions[i] * Scale));
            }
            cubeIndex++;
            return cubes[cubeIndex - 1];
        }
    }

    private double GetActionTime(Action action)
    {
        Stopwatch sw = Stopwatch.StartNew();
        action?.Invoke();
        return sw.ElapsedMilliseconds;
    }

    private void Start()
    {
        UnityEngine.Debug.Log(GetActionTime(() => CreateNoiseMap(size)));
    }
}