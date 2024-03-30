using UnityEngine;

public class ExportTool : Tool
{
    private ChunksHolder chunksHolder;

    public void SetChunksHolder ( ChunksHolder chunksHolder )
    {
        this.chunksHolder = chunksHolder;
    }

    public override void Activate ( Brush previousTool )
    {
        SimpleFileBrowser.FileBrowser.ShowSaveDialog(( path ) => HandleExport(path[0]),
            () => EventManagerGeneric<TextPopup>.InvokeEvent(new(2, "Cancelled Export."), EventType.OnQueuePopup),
            SimpleFileBrowser.FileBrowser.PickMode.Files, false, Application.persistentDataPath, "ExportName", "Export", "Export");
    }

    private void HandleExport ( string path )
    {
        ExportMeshToOBJ.ExportToOBJ(path, _mesh: chunksHolder.GatherMeshes());
    }

    public override void Deactivate ()
    {
    }
}