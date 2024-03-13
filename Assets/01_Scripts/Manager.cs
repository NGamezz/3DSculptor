using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

public class Manager : MonoBehaviour
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

public static class DataHolder
{
    public static int SaveVersion { get; set; }

    //public static void SetSave ( SaveData<float[], int3> save )
    //{
    //    if ( save != null )
    //    {
    //        currentSave = save;
    //    }
    //}

    //public static SaveData<float[], int3> GetSaveData () { return currentSave; }
}