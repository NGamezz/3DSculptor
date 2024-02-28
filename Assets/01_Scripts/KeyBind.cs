using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class KeyBind
{
    public List<KeyCode> keyCodes = new();

    public bool IsKeyBindActivated ()
    {
        if(keyCodes.Count == 0)
        {
            return false;
        }
        foreach ( var keyCode in keyCodes )
        {
            if ( !Input.GetKey(keyCode) )
            {
                return false;
            }
        }
        return true;
    }
}