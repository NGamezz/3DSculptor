using System;

public interface ICommand
{
    public void Execute (Action<object> actionToPerform );
    public void Undo ();
    public void Redo ();
}