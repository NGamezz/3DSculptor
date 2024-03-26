using UnityEngine;

public class SphereTool : Brush
{
    public override void OnAwake ()
    {
        ghost = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        ghost.GetComponent<Collider>().enabled = false;
        ghost.layer = brushData.ownLayer;

        ghost.GetComponent<Renderer>().material = brushData.ghostMaterial;

        base.OnAwake();
    }

    public void OnUpdate ()
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