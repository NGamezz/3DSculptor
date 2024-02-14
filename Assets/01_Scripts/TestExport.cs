using UnityEngine;

public class TestExport : MonoBehaviour
{
    [SerializeField] private GameObject[] testMesh;

    private void Start()
    {
        CreateSaveFile.SaveToFile<float>(10, 0, "nonoExist");
        var save = LoadSaveFile.LoadFileAsync<float>();
        Debug.Log(save);

        ExportToFbx.Export(testMesh, "data", () => Debug.Log("Filename Exists."));
    }
}
