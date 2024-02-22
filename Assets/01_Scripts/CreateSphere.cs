using UnityEngine;

public class CreateSphere : MonoBehaviour
{
    [SerializeField] private int gridSize;
    private Vector3[] normals;
    private Vector3[] vertices;

    private Mesh mesh;

    public float radius = 1f;
}
