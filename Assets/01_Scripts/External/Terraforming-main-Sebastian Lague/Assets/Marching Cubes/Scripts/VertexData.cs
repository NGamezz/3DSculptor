using UnityEngine;
using Unity.Mathematics;
using System;

public struct VertexData
{
    public Vector3 position;
    public Vector3 normal;
    public int2 id;
}

[Serializable]
public struct VoxelHolder
{
    public float3 position;
    public float3 normal;
    public int2 id;
}