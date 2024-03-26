using UnityEngine;

[CreateAssetMenu]
public class UndoSettings : ScriptableObject
{
    public int maxAmountOfStoredUndos;
    public int amountOfActivationsPerCall;
}