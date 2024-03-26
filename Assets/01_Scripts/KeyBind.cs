using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class KeyBind
{
    public KeyCode[] KeyCodes;

    public Action UponPressed;

    public void CheckKeyBindPress()
    {
        if(IsKeyBindActivated())
        {
            UponPressed?.Invoke();
        }
    }

    public bool IsKeyBindActivated ()
    {
        if(KeyCodes.Length == 0)
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