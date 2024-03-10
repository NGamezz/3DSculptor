using UnityEngine;
using Unity.Mathematics;

public static class Utility
{
    public static Vector3[] Float3ToVector3(float3[] array)
    {
        Vector3[] newArray = new Vector3[array.Length];

        for (int i = 0; i < array.Length; i++)
        {
            var element = array[i];
            newArray[i] = new Vector3(element.x, element.y, element.z);
        }
        return newArray;
    }

    public static Vector2[] Float2ToVector2(float2[] oldArray)
    {
        var newArray = new Vector2[oldArray.Length];

        for (int i = 0; i < oldArray.Length; i++)
        {
            var element = oldArray[i];
            newArray[i] = new Vector2(element.x, element.y);
        }

        return newArray;
    }

    public static float3[] Vector3ArrayToFloat3(Vector3[] array)
    {
        var newArray = new float3[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            newArray[i] = array[i];
        }
        return newArray;
    }

}