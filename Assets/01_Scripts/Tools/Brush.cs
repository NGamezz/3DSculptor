using NaughtyAttributes;
using System;
using UnityEngine;

public class Brush : Tool, ISizeChangable
{
    protected GameObject ghost;
    protected bool state = false;
    protected new Camera camera;
    [SerializeField] protected Material ghostMaterial;

    [Range(1.0f, 50.0f)]
    [SerializeField] protected float size = 5.0f;

    [Layer]
    [SerializeField] protected int ownLayer;

    protected Vector3 targetPosition;

    [Range(0.001f, 0.1f)]
    [SerializeField] protected float strength = 0.05f;

    private MeshCreator meshCreator;

    protected virtual void Awake ()
    {
        camera = Camera.main;
        meshCreator = FindAnyObjectByType<MeshCreator>();
        Brush = true;
        UpdateToolSize();
    }

    /// <summary>
    /// Additive
    /// </summary>
    /// <param name="size"></param>
    public void ChangeSize ( float size )
    {
        UpdateToolSize();
        this.size += size;
    }

    protected void Perform ( Vector3 point, bool state )
    {
        if ( state )
        {
            UndoTool.PerformAction(( context ) => meshCreator.Terraform(point, -strength, this.size), ( context ) => meshCreator.Terraform(point, strength, this.size));
            //meshCreator.Terraform(point, -strength, this.size);
        }
        else
        {
            UndoTool.PerformAction(( context ) => meshCreator.Terraform(point, strength, this.size), ( context ) => meshCreator.Terraform(point, -strength, this.size));
            //meshCreator.Terraform(point, strength, this.size);
        }
    }

    protected void UpdateToolSize ()
    {
        ghost.transform.localScale = Vector3.one * (this.size * 2.0f);
    }

    public override void Activate (Brush previousTool )
    {
        state = true;
        ghost.SetActive(true);
        Debug.Log("activated");
    }

    public override void Deactivate ()
    {
        state = false;
        ghost.SetActive(false);
        Debug.Log("deactivated");
    }

    private RaycastHit[] results = new RaycastHit[1];
    public void GetPositionOnModel ()
    {
        var ray = camera.ScreenPointToRay(Input.mousePosition);

        if ( Physics.RaycastNonAlloc(ray, results) == 0 || results[0].transform.gameObject.layer == ownLayer )
        {
            targetPosition = Vector3.zero;
            return;
        }

        targetPosition = results[0].point;
        ghost.transform.position = targetPosition;
    }
}

public struct BrushActionData
{
    public Vector3 position;
    public float radius;
    public float strenght;
}