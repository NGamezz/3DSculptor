using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class KeyBind
{
    public List<KeyCode> KeyCodes = new();

    public bool IsKeyBindActivated ()
    {
        if(KeyCodes.Count == 0)
        {
            return false;
        }
        foreach ( var keyCode in KeyCodes )
        {
            if ( !Input.GetKey(keyCode) )
            {
                return false;
            }
        }
        return true;
    }
}