using UnityEngine;

public class ChunksHolder : MonoBehaviour
{
    //Purely for testing.
    public Mesh GatherMeshes ()
    {
        var meshFilters = GetComponentsInChildren<MeshFilter>();
        var combine = new CombineInstance[meshFilters.Length];

        for ( int i = 0; i < meshFilters.Length; i++ )
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }

        Mesh mesh = new();
        mesh.CombineMeshes(combine);
        return mesh;
    }
}