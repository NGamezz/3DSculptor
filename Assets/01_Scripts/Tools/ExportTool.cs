using UnityEngine;

public class ExportTool : Tool
{
    private ChunksHolder chunksHolder;

    private void Start ()
    {
        chunksHolder = FindAnyObjectByType<ChunksHolder>();
    }

    public override void Activate ( Brush previousTool )
    {
        SimpleFileBrowser.FileBrowser.ShowSaveDialog(( path ) => HandleExport(path[0]), () => EventManager<TextPopup>.InvokeEvent(new(2, "Cancelled Export."), EventType.OnQueuePopup), SimpleFileBrowser.FileBrowser.PickMode.Files, false, Application.persistentDataPath, "ExportName", "Export", "Export");
    }

    private void HandleExport ( string path )
    {
        ExportMeshToOBJ.ExportToOBJ(path, _mesh: chunksHolder.GatherMeshes());
    }

    public override void Deactivate ()
    {
    }
}