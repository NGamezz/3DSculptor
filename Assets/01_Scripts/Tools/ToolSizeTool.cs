using UnityEngine;

public class ToolSizeTool : Tool
{
    public float increment = 1;

    public void OnStart ()
    {
        Brush = false;
    }

    public override void Activate (Brush previousTool )
    {
        previousTool.ChangeSize(increment);
    }

    public override void Deactivate ()
    {
    }
}