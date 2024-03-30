using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class SaveTool : Tool
{
    public Func<RenderTexture> OnRequestRenderTexture;

    private string path = "";

    private bool canSave = true;

    public void ResetPath ()
    {
        path = "";
    }

    public override void Deactivate ()
    {
    }

    public override async void Activate ( Brush previousTool )
    {
        if ( !canSave )
            return;
        canSave = false;

        EventManagerGeneric<TextPopup>.InvokeEvent(new(2, "Starting Save."), EventType.OnQueuePopup);

        var rawDensityTexture = OnRequestRenderTexture?.Invoke();

        if ( rawDensityTexture == null )
            return;

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
            dataB = dimensions,
        };

        if ( path == "" )
        {
            Debug.Log("Path does not exist yet.");
            EventManagerGeneric<bool>.InvokeEvent(true, EventType.OnPause);
            SimpleFileBrowser.FileBrowser.ShowSaveDialog(( path ) => HandleSave(path, saveData), UponCancelSave, SimpleFileBrowser.FileBrowser.PickMode.Files, false, Application.persistentDataPath);
        }
        else
        {
            Debug.Log("Saving.");
            CreateSaveFile.SaveToFile(saveData, path);
        }

        ResetCanSave();
    }

    private async void ResetCanSave ()
    {
        await Awaitable.WaitForSecondsAsync(0.5f);
        canSave = true;
    }

    private void UponCancelSave ()
    {
        EventManagerGeneric<TextPopup>.InvokeEvent(new(2, "Cancelled Save."), EventType.OnQueuePopup);
        EventManagerGeneric<bool>.InvokeEvent(false, EventType.OnPause);
    }

    private void StartSave ()
    {
        Activate(null);
    }

    public void OnStart ()
    {
        EventManager.AddListener(EventType.StartSave, StartSave);
        EventManager.AddListener(EventType.OnCreateNew, ResetPath);
    }

    public void OnDisable ()
    {
        EventManager.RemoveListener(EventType.StartSave, StartSave);
        EventManager.RemoveListener(EventType.OnCreateNew, ResetPath);
    }

    private void HandleSave<T, U> ( string[] path, SaveData<T, U> data )
    {
        EventManagerGeneric<bool>.InvokeEvent(false, EventType.OnPause);
        this.path = path[0];
        CreateSaveFile.SaveToFile(data, this.path);
    }
}