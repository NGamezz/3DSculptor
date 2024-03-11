using UnityEngine;

public class ToolSizeTool : Tool
{
    [SerializeField] private float increment = 1;

    private void Start ()
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