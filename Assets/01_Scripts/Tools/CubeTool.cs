using UnityEngine;

public class CubeTool : Brush
{
    protected override void Awake ()
    {
        ghost = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ghost.GetComponent<Collider>().enabled = false;
        ghost.layer = ownLayer;

        Color color = Color.white;
        color.a = 0.5f;
        ghost.GetComponent<Renderer>().material.color = color;

        base.Awake();
    }

    private void Update ()
    {
        if ( !state )
            return;
        GetPositionOnModel();
    }
}