using System.Collections.Generic;
using Unity.Mathematics;

public class UndoTool : Tool
{
    private DoubleStack<ActionData> actionHistory = new();

    private UndoSettings undoSettings;

    public void SetUndoSettings ( UndoSettings settings )
    {
        this.undoSettings = settings;
    }

    public void OnEnable ()
    {
        EventManager<ActionData>.AddListener(EventType.OnPerformAction, PerformAction);
        EventManager<SaveData<float[], int3>>.AddListener(EventType.OnDataLoad, ResetActionStack);
    }

    public void OnDisable ()
    {
        EventManager<ActionData>.RemoveListener(EventType.OnPerformAction, PerformAction);
        EventManager<SaveData<float[], int3>>.RemoveListener(EventType.OnDataLoad, ResetActionStack);
    }

    private void ResetActionStack ( SaveData<float[], int3> _ )
    {
        actionHistory.Clear();
    }

    private void PerformAction ( ActionData actionData )
    {
        actionHistory.Push(actionData);

        if ( actionHistory.Count >= undoSettings.maxAmountOfStoredUndos * undoSettings.amountOfActivationsPerCall )
        {
            actionHistory.PopBottom();
        }
    }

    public void Undo ()
    {
        if ( actionHistory.Count <= 0 )
            return;

        for ( int i = 0; i < undoSettings.amountOfActivationsPerCall; i++ )
        {
            if ( actionHistory.Count <= 0 )
            { continue; }

            var currentData = actionHistory.Pop();

            if ( currentData == null )
                return;

            EventManager<ActionData>.InvokeEvent(currentData, EventType.OnUndo);
        }
    }

    public override void Activate ( Brush previousBrush )
    {
        Undo();
    }

    public override void Deactivate ()
    {
    }
}

public class DoubleStack<T>
{
    private readonly List<T> values = new();

    public int Count { get { return values.Count; } }

    public void Push ( T item )
    {
        values.Add(item);
    }

    public T Pop ()
    {
        if ( values.Count <= 0 )
            return default;

        T item = values[^1];
        values.Remove(item);

        return item;
    }

    public T PopBottom ()
    {
        if ( values.Count <= 0 )
            return default;

        T item = values[0];
        values.Remove(item);

        return item;
    }

    public T Peek ()
    {
        if ( values.Count <= 1 )
            return default;

        T item = values[values.Count - 2];
        return item;
    }

    public T PeekBottom ()
    {
        if ( values.Count <= 1 )
            return default;

        T item = values[1];
        return item;
    }

    public void Clear ()
    {
        values.Clear();
    }
}