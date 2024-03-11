using UnityEngine;

public abstract class Tool : MonoBehaviour
{
    public bool Brush = true;

    public string Name;

    [SerializeField] public KeyBind keyBind;

    public abstract void Activate(Brush previousTool);
    public abstract void Deactivate();
}

public interface ISizeChangable
{
    public abstract void ChangeSize(float size);
}
