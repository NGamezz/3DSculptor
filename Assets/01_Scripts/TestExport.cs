using Unity.Mathematics;
using UnityEngine;

public class TestExport : MonoBehaviour
{
    [SerializeField] private GameObject testMesh;

    public void Export()
    {
        CreateSaveFile.SaveToFile<float>(10, 0, "nonoExist");
        LoadSaveFile.LoadFileAsync<float>(HandleSaveLoad);

        ExportMeshToOBJ.ExportToOBJ(testMesh);
    }

    private void HandleSaveLoad<T>(T data)
    {
        Debug.Log(data);
    }
}

public struct VoxelData
{
    float3[] vertices;
    float2[] uv;
    float3[] normals;
}