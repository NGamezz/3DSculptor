public class CreateNew : Tool
{
    public override void Activate ( Brush previousTool )
    {
        EventManager<TextPopup>.InvokeEvent(new(2, "Creating New Model."), EventType.OnQueuePopup);

        DataHolder.SaveVersion = 0;

        EventManager<bool>.InvokeEvent(EventType.OnCreateNew);
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