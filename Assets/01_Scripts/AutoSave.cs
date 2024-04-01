using System;
using UnityEngine;

[Serializable]
public class AutoSave
{
    [Tooltip("In Seconds."), Range(2.0f, 500.0f)]
    [SerializeField] private float autoSaveInterval = 10.0f;

    private bool enableAutoSave;
    public bool EnableAutoSave
    {
        get { return enableAutoSave; }
        set
        {
            if ( value == true )
                SaveOnInterval();

            enableAutoSave = value;
        }
    }

    public void StartAutoSave ()
    {
       SaveOnInterval();
    }

    private async void SaveOnInterval ()
    {
        while ( EnableAutoSave )
        {
            await Awaitable.WaitForSecondsAsync(autoSaveInterval);

            if ( enableAutoSave == false )
                return;

            EventManager.InvokeEvent(EventType.StartSave);
        }
    }
}