using UnityEngine;

public class CubeTool : Tool
{
    [SerializeField] private int modelLayerMask;

    private new Camera camera;

    private GameObject cubeGhost;

    private bool state = false;

    private Vector3 targetPosition = Vector3.zero;

    private void Start ()
    {
        camera = Camera.main;
        cubeGhost = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cubeGhost.GetComponent<Collider>().enabled = false;

        Color color = Color.white;
        color.a = 0.5f;
        cubeGhost.GetComponent<Renderer>().material.color = color;
    }

    private void Update ()
    {
        if ( !state )
            return;
        GetPositionOnModel();
    }

    private void FixedUpdate ()
    {
        if ( !state )
            return;
        UpdateToolSize();
    }

    private void UpdateToolSize ()
    {
        //sphereGhost.Radius = this.Size;
    }

    RaycastHit[] results = new RaycastHit[1];
    private void GetPositionOnModel ()
    {
        var ray = camera.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.blue);

        if ( Physics.RaycastNonAlloc(ray, results) == 0 || results[0].transform.gameObject.layer != modelLayerMask )
        {
            targetPosition = Vector3.zero;
            return;
        }

        targetPosition = results[0].point;
        cubeGhost.transform.position = targetPosition;
    }

    private void OnDrawGizmos ()
    {
        if ( targetPosition == Vector3.zero )
            return;
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(targetPosition, this.Size);
    }

    public override void Activate ()
    {
        state = true;
        cubeGhost.SetActive(true);
        Debug.Log("activated");
    }

    public override void Deactivate ()
    {
        state = false;
        cubeGhost.SetActive(false);
        Debug.Log("deactivated");
    }
}