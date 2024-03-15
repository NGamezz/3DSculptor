using UnityEngine;

public class AutoSave : MonoBehaviour
{
    [Tooltip("In Seconds."), Range(2.0f, 500.0f)]
    [SerializeField] private float autoSaveInterval = 10.0f;

    [SerializeField] private bool autoSave = true;

    private SaveTool saveTool;

    private void Start ()
    {
        saveTool = FindAnyObjectByType<SaveTool>();

        SaveOnInterval();
    }

    private async void SaveOnInterval ()
    {
        while ( autoSave )
        {
            await Awaitable.WaitForSecondsAsync(autoSaveInterval);

            saveTool.Activate(null);
        }
    }
}