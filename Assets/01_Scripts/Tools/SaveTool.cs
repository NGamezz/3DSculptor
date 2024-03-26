using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class SaveTool : Tool
{
    public Func<RenderTexture> OnRequestRenderTexture;

    private string path = "";

    public void ResetPath ()
    {
        path = "";
    }

    public override void Deactivate ()
    {
    }

    public override async void Activate ( Brush previousTool )
    {
        EventManager<TextPopup>.InvokeEvent(new(2, "Starting Save."), EventType.OnQueuePopup);

        var rawDensityTexture = OnRequestRenderTexture?.Invoke();

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

        if ( path == "" )
        {
            Debug.Log("Path does not exist yet.");
            SimpleFileBrowser.FileBrowser.ShowSaveDialog(( path ) => HandleSave(path, saveData), () => EventManager<TextPopup>.InvokeEvent(new(2, "Cancelled Save."), EventType.OnQueuePopup), SimpleFileBrowser.FileBrowser.PickMode.Files, false, Application.persistentDataPath);
        }
        else
        {
            Debug.Log("Saving.");
            CreateSaveFile.SaveToFile(saveData, path);
        }
    }

    private void StartSave ()
    {
        Activate(null);
    }

    public void OnStart ()
    {
        EventManager<bool>.AddListener(EventType.StartSave, StartSave);
        EventManager<bool>.AddListener(EventType.OnCreateNew, ResetPath);
    }

    public void OnDisable ()
    {
        EventManager<bool>.RemoveListener(EventType.StartSave, StartSave);
        EventManager<bool>.RemoveListener(EventType.OnCreateNew, ResetPath);
    }

    private void HandleSave<T, U> ( string[] path, SaveData<T, U> data )
    {
        this.path = path[0];
        CreateSaveFile.SaveToFile(data, this.path);
    }
}