using Unity.Mathematics;
using UnityEngine;

public class ActionData
{
    public int radius;
    public PointData[] floats;
    public Vector3 position;
}

public struct PointData
{
    public int3 index;
    public float value;
}