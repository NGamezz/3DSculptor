using UnityEngine;

public abstract class Tool : MonoBehaviour
{
    public bool Utility = false;

    public string Name;

    [SerializeField] public KeyBind keyBind;

    public abstract void Activate ();
    public abstract void Deactivate ();
}

public interface ISizeChangable
{
    public abstract void ChangeSize (float size);
}
