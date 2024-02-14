using UnityEngine;

public class TestExport : MonoBehaviour
{
    [SerializeField] private GameObject[] testMesh;

    private void Start()
    {
        ExportToFbx.Export(testMesh, "data", () => Debug.Log("Filename Exists.")); ;
    }
}
