using Unity.Collections;
using UnityEngine;

public class LoadTool : Tool
{
    private MeshCreator meshCreator;

    public override void Activate(Brush previousBrush)
    {
        LoadSaveFile.LoadFileAsync<byte[]>((data) => HandleLoad(data));
    }

    private void HandleLoad(byte[] data)
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
