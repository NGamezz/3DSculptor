using Unity.Mathematics;

public class LoadTool : Tool
{
    private MeshCreator meshCreator;

    public override void Activate(Brush previousBrush)
    {
        LoadSaveFile.LoadFileAsync<float[], int3>((data) => HandleLoad(data));
    }

    private void HandleLoad ( SaveData<float[], int3> data)
    {
        meshCreator.LoadVertices(data);
    }

    public override void Deactivate()
    {
    }

    private void Awake()
    {
        meshCreator = FindAnyObjectByType<MeshCreator>();
    }
}
