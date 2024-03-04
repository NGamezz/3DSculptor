using UnityEngine;

public class SphereTool : Brush
{
    protected override void Awake ()
    {
        ghost= GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ghost.GetComponent<Collider>().enabled = false;
        ghost.layer = ownLayer;
        
        ghost.GetComponent<Renderer>().material = ghostMaterial;

        base.Awake();
    }

    private void Update ()
    {
        if ( !state )
            return;
        GetPositionOnModel();
        if ( Input.GetMouseButton(0) )
        {
            Perform(targetPosition, true);
        }
        else if ( Input.GetMouseButton(1) )
        {
            Perform(targetPosition, false);
        }
    }
}