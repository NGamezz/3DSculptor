public class CreateNew : Tool
{
    private MeshCreator creator;
    private SaveTool saveTool;

    public override void Activate ( Brush previousTool )
    {
        EventManager<TextPopup>.InvokeEvent(new(2, "Creating New Model."), EventType.OnQueuePopup);
        //DataHolder.TextPopupManager.QueuePopup(new(2, "Creating New Model."));

        if(saveTool != null)
            saveTool.ResetPath();

        DataHolder.SaveVersion = 0;

        EventManager<bool>.InvokeEvent(EventType.OnCreateNew);
        //creator.CreateNew();
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