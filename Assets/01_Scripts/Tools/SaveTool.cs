using System;
using System.IO;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class SaveTool : Tool
{
    [SerializeField] private GameObject modelObject;
    private Mesh mesh;

    private MeshCreator meshCreator;

    private string path = "";

    private void Awake()
    {
        if (modelObject == null)
            mesh = FindAnyObjectByType<ChunksHolder>().GatherMeshes();

        if (meshCreator == null)
            meshCreator = FindAnyObjectByType<MeshCreator>();
    }

    public override async void ActivateAsync()
    {
        Debug.Log("Activate Save.");

        //VoxelHolder data = new();

        //var data = new SaveData<Texture2D>();
        var texture = meshCreator.GetRenderTexture();

        Texture2D tex = new(texture.width, texture.height, TextureFormat.RGB24, false);
        RenderTexture.active = texture;

        var request = await AsyncGPUReadback.RequestAsync(texture);

        request.WaitForCompletion();

        //tex.LoadRawTextureData(request.GetData<uint>());
        //tex.Apply();
        //byte[] bytes = tex.EncodeToPNG();

        //System.IO.File.WriteAllBytes(Path.Combine(Application.persistentDataPath, "TestFile.png"), bytes);

        //if (mesh == null)
        //{
        //    mesh = modelObject.GetComponent<MeshFilter>().mesh;
        //    if (mesh == null)
        //    {
        //        throw new System.Exception("Selected model has no MeshFilter.");
        //    }
        //}

        SaveData<byte[]> data = new();
        var nativeArray = request.GetData<byte>();

        byte[] newArray = new byte[nativeArray.Length];
        for (int i = 0; i < newArray.Length; i++)
        {
            newArray[i] = nativeArray[i];
        }
        nativeArray.Dispose();
        data.data = newArray;

        if (path == "")
        {
            Debug.Log("Path does not exist yet.");
            SimpleFileBrowser.FileBrowser.ShowSaveDialog((path) => HandleSave(path, data), null, SimpleFileBrowser.FileBrowser.PickMode.Files, false, Application.persistentDataPath);
        }
        else
        {
            Debug.Log("Saving.");
            CreateSaveFile.SaveToFile(data, 0, path);
        }
    }

    private void HandleSave<T>(string[] path, T data)
    {
        this.path = path[0];
        CreateSaveFile.SaveToFile(data, 0, this.path);
    }

    public override void Deactivate()
    {
    }
}