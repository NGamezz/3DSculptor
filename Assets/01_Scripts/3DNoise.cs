using Unity.Mathematics;
using UnityEngine;

public class Perlin3D : MonoBehaviour
{
    public static float Get3DNoise(float3 position)
    {
        float ab = Mathf.PerlinNoise(position.x, position.y);
        float bc = Mathf.PerlinNoise(position.y, position.z);
        float ac = Mathf.PerlinNoise(position.x, position.z);

        float ba = Mathf.PerlinNoise(position.y, position.x);
        float cb = Mathf.PerlinNoise(position.z, position.y);
        float ca = Mathf.PerlinNoise(position.z, position.x);

        return (ab + bc + ac + ba + cb + ca) / 6.0f;
    }
}