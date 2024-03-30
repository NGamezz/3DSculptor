using Unity.Mathematics;

public class LoadTool : Tool
{
    public override void Activate ( Brush previousBrush )
    {
        LoadSaveFile.LoadFileAsync<float[], int3>(( data ) => HandleLoad(data));
    }

    public override void Deactivate ()
    {
    }

    private void HandleLoad ( SaveData<float[], int3> data )
    {
        EventManagerGeneric<SaveData<float[], int3>>.InvokeEvent(data, EventType.OnDataLoad);
    }
}