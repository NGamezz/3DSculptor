using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu]
public class BrushData : ScriptableObject
{
    public float size;
    public float weight;
    [Layer]
    public int ownLayer;
    public Material ghostMaterial;
}