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
        //Purely for testing.
        var chunkHolder = FindAnyObjectByType<ChunksHolder>();

        ExportMeshToOBJ.ExportToOBJ(_mesh: chunkHolder.GatherMeshes());
    }

    private void HandleSaveLoad<T> ( T data )
    {
        Debug.Log(data);
    }
}