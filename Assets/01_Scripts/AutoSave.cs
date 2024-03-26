using System;
using UnityEngine;

[Serializable]
public class AutoSave
{
    [Tooltip("In Seconds."), Range(2.0f, 500.0f)]
    [SerializeField] private float autoSaveInterval = 10.0f;

    [SerializeField] private bool autoSave = true;

    public void StartAutoSave()
    {
        SaveOnInterval();
    }

    private async void SaveOnInterval ()
    {
        while ( autoSave )
        {
            await Awaitable.WaitForSecondsAsync(autoSaveInterval);

            EventManager<bool>.InvokeEvent(EventType.StartSave);
        }
    }
}