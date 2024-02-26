using Unity.Mathematics;
using UnityEngine;

public class TestExport : MonoBehaviour
{
    [SerializeField] private GameObject testMesh;

    private void Start ()
    {
        if ( testMesh != null )
            Export();
    }

    public void Export ()
    {
        CreateSaveFile.SaveToFile<float>(10, 0, "nonoExist");
        LoadSaveFile.LoadFileAsync<float>(HandleSaveLoad);

        //Purely for testing.
        var chunkHolder = FindAnyObjectByType<ChunksHolder>();

        ExportMeshToOBJ.ExportToOBJ(_mesh: chunkHolder.GatherMeshes());
    }

    private void HandleSaveLoad<T> ( T data )
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