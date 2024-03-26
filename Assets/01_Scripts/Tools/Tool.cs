using UnityEngine;

public abstract class Tool
{
    public bool Brush = false;

    public bool IgnoreCooldown = true;

    public string Name;

    public KeyCode[] KeyBind;

    public abstract void Activate(Brush previousTool);
    public abstract void Deactivate();
}

public interface ISizeChangable
{
    public abstract void ChangeSize(float size);
}