using System;
using System.Collections.Generic;

public enum EventType
{
    OnUndo = 0,
    OnDataLoad = 1,
    OnCreateNew = 2,
    OnPerformAction = 3,
    OnEdit = 4,
    OnQueuePopup = 5,
}

public static class EventManager<T>
{
    private static Dictionary<EventType, Action<T>> eventsParameter = new();

    private static Dictionary<EventType, Action> events = new();

    public static void AddListener ( EventType type, Action<T> action )
    {
        if ( action == null )
            return;

        if ( eventsParameter.ContainsKey(type) )
        {
            eventsParameter[type] += action;
        }
        else
        {
            eventsParameter.Add(type, action);
        }
    }

    public static void AddListener ( EventType type, Action action )
    {
        if ( action == null )
            return;

        if ( events.ContainsKey(type) )
        {
            events[type] += action;
        }
        else
        {
            events.Add(type, action);
        }
    }

    public static void InvokeEvent ( T input, EventType type )
    {
        if ( !eventsParameter.ContainsKey(type) )
            return;

        eventsParameter[type].Invoke(input);
    }

    public static void InvokeEvent(EventType type)
    {
        if ( !events.ContainsKey(type) )
            return;

        events[type].Invoke();
    }

    public static void RemoveListener(EventType type, Action<T> action)
    {
        if ( !eventsParameter.ContainsKey(type) )
            return;

        eventsParameter[type] -= action;
    }

    public static void RemoveListener(EventType type, Action action)
    {
        if ( !events.ContainsKey(type) )
            return;

        events[type] -= action;
    }
}
