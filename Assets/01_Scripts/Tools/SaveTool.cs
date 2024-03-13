using UnityEngine;
using UnityEngine.Rendering;

public class SaveTool : Tool
{
    private MeshCreator meshCreator;

    private string path = "";

    private void Awake()
    {
        if (meshCreator == null)
            meshCreator = FindAnyObjectByType<MeshCreator>();
    }

    public override async void Activate(Brush previousTool)
    {
        Debug.Log("Activate Save.");

        var texture = meshCreator.GetRenderTexture();

        Texture2D tex = new(texture.width, texture.height, TextureFormat.RGB24, false);
        RenderTexture.active = texture;

        var request = await AsyncGPUReadback.RequestAsync(texture);

        request.WaitForCompletion();

        SaveData<byte[]> data = new();
        var nativeArray = request.GetData<byte>();

        byte[] newArray = new byte[nativeArray.Length];
        for (int i = 0; i < newArray.Length; i++)
        {
            newArray[i] = nativeArray[i];
        }

        nativeArray.Dispose();
        data.data = newArray;
        data.buildVersion = 0;

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