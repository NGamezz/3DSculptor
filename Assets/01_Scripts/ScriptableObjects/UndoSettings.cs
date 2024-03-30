using System;
using UnityEngine;

[CreateAssetMenu]
public class UndoSettings : ScriptableObject
{
    public int maxAmountOfStoredUndos;
    public int amountOfActivationsPerCall;

    public void SetMaxAmountOfStoredUndos ( string input )
    {
        if ( !Int32.TryParse(input, out int amount) )
            return;

        if ( amount <= 0 )
            return;

        maxAmountOfStoredUndos = amount;
        PlayerPrefs.SetString("MaxAmountOfUndos", amount.ToString());
    }
}