using UnityEngine;

public abstract class Tool : MonoBehaviour
{
    public float Size;

    public string Name;

    [SerializeField] public KeyBind keyBind;

    public abstract void Activate ();
    public abstract void Deactivate ();
}