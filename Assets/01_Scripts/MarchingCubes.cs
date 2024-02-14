using Unity.Mathematics;
using UnityEngine;

public class MarchingCubes : MonoBehaviour
{
    [SerializeField] private float treshHold = 0.5f;
    [SerializeField] private float scale = 0.9f;
    [SerializeField] private int size = 50;
    [SerializeField] private GameObject testCube;

    //Testing
    void CreateNoiseMap(int size)
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    var currentValue = Perlin3D.Get3DNoise(new float3(x * scale, y * scale, z * scale));
                    if (currentValue > treshHold)
                    {
                        var cube = Instantiate(testCube, new Vector3(x, y, z), Quaternion.identity);
                        cube.transform.parent = transform;
                    }
                }
            }
        }
    }

    private void Start()
    {
        CreateNoiseMap(size);
    }
}