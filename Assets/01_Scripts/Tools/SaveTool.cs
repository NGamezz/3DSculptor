using Unity.Mathematics;
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

        var rawDensityTexture = meshCreator.GetRenderTexture();

        var request = await AsyncGPUReadback.RequestAsync(rawDensityTexture);

        while ( request.done == false )
        {
            await Awaitable.NextFrameAsync();
        }

        float[] floats = new float[request.layerCount * rawDensityTexture.width * rawDensityTexture.height];

        int index = 0;

        for ( int i = 0; i < request.layerCount; i++ )
        {
            var array = request.GetData<float>(i);
            for ( int t = 0; t < array.Length; t++ )
            {
                var val = array[t];
                floats[index++] = val;
            }
        }

        int3 dimensions = new(rawDensityTexture.width, rawDensityTexture.height, rawDensityTexture.volumeDepth);

        SaveData<float[], int3> saveData = new()
        {
            data = floats,
            buildVersion = 0,
            dataB = dimensions
        };

        if (path == "")
        {
            Debug.Log("Path does not exist yet.");
            SimpleFileBrowser.FileBrowser.ShowSaveDialog((path) => HandleSave(path, ref saveData), null, SimpleFileBrowser.FileBrowser.PickMode.Files, false, Application.persistentDataPath);
        }
        else
        {
            Debug.Log("Saving.");
            CreateSaveFile.SaveToFile(ref saveData, path);
        }
    }

    private void HandleSave<T,U>(string[] path, ref SaveData<T,U> data)
    {
        this.path = path[0];
        CreateSaveFile.SaveToFile(ref data, this.path);
    }

    public override void Deactivate()
    {
    }
}