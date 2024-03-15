public class CreateNew : Tool
{
    private MeshCreator creator;
    private SaveTool saveTool;

    public override void Activate ( Brush previousTool )
    {
        DataHolder.TextPopupManager.QueuePopup(new(2, "Creating New Model."));

        if(saveTool != null)
            saveTool.ResetPath();

        DataHolder.SaveVersion = 0;

        creator.CreateNew();
    }

    public override void Deactivate ()
    {
    }

    void Start()
    {
        creator = FindAnyObjectByType<MeshCreator>();
        saveTool = FindAnyObjectByType<SaveTool>();
        Brush = false;

        DataHolder.SaveVersion = 0;
    }
}