public class CreateNew : Tool
{
    public override void Activate ( Brush previousTool )
    {
        EventManagerGeneric<TextPopup>.InvokeEvent(new(2, "Creating New Model."), EventType.OnQueuePopup);

        DataHolder.SaveVersion = 0;

        EventManager.InvokeEvent(EventType.OnCreateNew);
    }

    public override void Deactivate ()
    {
    }

   public void OnStart ()
    {
        Brush = false;
        DataHolder.SaveVersion = 0;
    }
}