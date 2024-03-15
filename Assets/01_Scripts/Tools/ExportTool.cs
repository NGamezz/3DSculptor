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
        SimpleFileBrowser.FileBrowser.ShowSaveDialog(( path ) => HandleExport(path[0]), () => DataHolder.TextPopupManager.QueuePopup(new(2, "Canceled Export.")), SimpleFileBrowser.FileBrowser.PickMode.Files, false, Application.persistentDataPath, "ExportName", "Export", "Export");
    }

    private void HandleExport ( string path )
    {
        ExportMeshToOBJ.ExportToOBJ(path, _mesh: chunksHolder.GatherMeshes());
    }

    public override void Deactivate ()
    {
    }
}