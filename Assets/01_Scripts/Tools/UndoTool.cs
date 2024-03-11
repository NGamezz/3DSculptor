using System;
using System.Collections.Generic;
using UnityEngine;

public class UndoTool : Tool
{
    private static List<Command> commands = new();

    [SerializeField] private bool undo = true;

    private static int currentIndex = 0;

    public static void PerformAction ( Action<object> actionToPerform, Action<object> undo )
    {
        var newActionHolder = new Command
        {
            Action = actionToPerform,
            Undo = undo
        };

        commands.Add( newActionHolder );
        newActionHolder.Action?.Invoke(null);

        currentIndex = Array.IndexOf(commands.ToArray(), newActionHolder);
    }

    public void Undo ()
    {
        if ( currentIndex <= 0 )
        {
            Debug.LogWarning("No Undo's Available.");
            return;
        }

        var currentCommand = commands[currentIndex--];

        if ( currentCommand.undone )
            return;

        currentCommand.undone = true;
        currentCommand.Undo?.Invoke(null);
    }

    public void Redo ()
    {
        if ( currentIndex >= commands.Count || currentIndex + 1 >= commands.Count )
        {
            Debug.LogWarning("No Redo's Available.");
            return;
        }

        var currentCommand = commands[++currentIndex];

        if ( !currentCommand.undone )
            return;

        currentCommand.undone = false;
        currentCommand.Action?.Invoke(null);
    }

    public override void Activate (Brush previousBrush )
    {
        if ( undo )
        {
            Undo();
        }
        else
        {
            Redo();
        }
    }

    public override void Deactivate ()
    {
    }
}

public class Command
{
    public Action<object> Action;
    public Action<object> Undo;
    public bool undone = false;
}