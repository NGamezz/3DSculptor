using UnityEngine;

public class Brush : Tool, ISizeChangable
{
    protected GameObject ghost;
    protected bool state = false;
    protected Camera camera;

    protected BrushData brushData;

    protected Vector3 targetPosition;

    private BrushData cacheBrushData;

    private RaycastHit[] results = new RaycastHit[1];

    public void SetBrushData ( BrushData brushData )
    {
        Debug.Log(brushData);
        this.brushData = brushData;

        cacheBrushData = new()
        {
            size = brushData.size,
            weight = brushData.weight,
            ownLayer = brushData.ownLayer,
            ghostMaterial = brushData.ghostMaterial,
        };
    }

    public void OnDisable ()
    {
        brushData.ownLayer = cacheBrushData.ownLayer;
        brushData.size = cacheBrushData.size;
        brushData.weight = cacheBrushData.weight;
        brushData.ghostMaterial = cacheBrushData.ghostMaterial;
    }

    public void ChangeSize ( float size )
    {
        if ( brushData.size + size <= 0 || brushData.size + size >= 50)
            return;

        brushData.size += size;
        UpdateToolSize();
    }

    public override void Activate ( Brush previousTool )
    {
        state = true;
        ghost.SetActive(true);
    }

    public override void Deactivate ()
    {
        state = false;
        ghost.SetActive(false);
    }
  
    public virtual void OnAwake ()
    {
        camera = Camera.main;
        Brush = true;
        IgnoreCooldown = false;
        UpdateToolSize();
    }

    protected void GetPositionOnModel ()
    {
        var ray = camera.ScreenPointToRay(Input.mousePosition);

        if ( Physics.RaycastNonAlloc(ray, results) == 0 || results[0].transform.gameObject.layer == brushData.ownLayer )
        {
            targetPosition = Vector3.zero;
            return;
        }

        targetPosition = results[0].point;
        ghost.transform.position = targetPosition;
    }

    protected void Perform ( Vector3 point, bool state )
    {
        BrushActionData actionData = new()
        {
            position = point,
            radius = brushData.size,
        };

        if ( state )
        {
            actionData.strenght = -brushData.weight;

            EventManagerGeneric<BrushActionData>.InvokeEvent(actionData, EventType.OnEdit);
        }
        else
        {
            actionData.strenght = brushData.weight;

            EventManagerGeneric<BrushActionData>.InvokeEvent(actionData, EventType.OnEdit);
        }
    }

    protected void UpdateToolSize ()
    {
        ghost.transform.localScale = Vector3.one * (brushData.size * 2.0f);
    }
}

public struct BrushActionData
{
    public Vector3 position;
    public float radius;
    public float strenght;
}