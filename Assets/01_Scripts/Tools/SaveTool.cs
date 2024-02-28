using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class SaveTool : Tool
{
    [SerializeField] private GameObject modelObject;

    private string path = "";

    public override void Activate ()
    {
        Debug.Log("Activate Save.");

        VoxelHolder data = new();
        var mesh = modelObject.GetComponent<MeshFilter>().mesh;
        data.voxels = Vector3ArrayToFloat3(mesh.vertices);

        if ( path == "" )
        {
            Debug.Log("Path does not exist yet.");
            SimpleFileBrowser.FileBrowser.ShowSaveDialog(( path ) => HandleSave(path, data), null, SimpleFileBrowser.FileBrowser.PickMode.Files, false, Application.persistentDataPath);
        }
        else
        {
            Debug.Log("Saving.");
            CreateSaveFile.SaveToFile(data, data.version, path);
        }
    }

    private float3[] Vector3ArrayToFloat3 ( Vector3[] array )
    {
        var newArray = new float3[array.Length];
        for ( int i = 0; i < array.Length; i++ )
        {
            newArray[i] = array[i]; 
        }
        return newArray;
    }

    private void HandleSave ( string[] path , VoxelHolder data)
    {
        this.path = path[0];
        CreateSaveFile.SaveToFile(data, 0, this.path);
    }

    public override void Deactivate ()
    {
    }
}