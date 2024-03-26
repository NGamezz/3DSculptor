using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InputHandler
{
    public Action<Tool> UponBindingActivation;
    
    private List<KeyCommand> keyCommands = new();

    public void Initialize(ref Action onUpdate, ref Action onDestroy)
    {
        onUpdate += OnUpdate;
        onDestroy += OnDestroy;
    }

    public void OnUpdate()
    {
        foreach(var command in keyCommands)
        {
            if(command.IsKeyBindActivated())
            {
                UponBindingActivation?.Invoke(command.tool);
            }
        }
    }

    public void BindInputToCommand ( Tool tool, object context = null, params KeyCode[] keyCodes)
    {
        KeyCommand newKeyCommand = new()
        {
            tool = tool,
            keyCodes = keyCodes,
            context = context
        };

        keyCommands.Add(newKeyCommand);
    }

    private void OnDestroy()
    {
    }
}

public class KeyCommand
{
    public KeyCode[] keyCodes;
    public object context;
    public Tool tool;

    public bool IsKeyBindActivated ()
    {
        if ( keyCodes.Length == 0 )
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

public interface ICommand
{
    public abstract void Execute (object context = null);
}