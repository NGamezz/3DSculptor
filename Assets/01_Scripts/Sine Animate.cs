using System.Net.NetworkInformation;
using Unity.Mathematics;
using UnityEngine;

public class SineAnimate : MonoBehaviour
{
    [SerializeField] private float sineStrength = 0.5f;
    [SerializeField] private float frequency = 1.0f;

    private Vector3 cachedPosition = Vector3.zero;

    private void Start ()
    {
        cachedPosition = transform.position;
    }

    private void Animate ()
    {
        var position = transform.position;
        position.y = cachedPosition.y + math.sin(Time.time * frequency) * sineStrength;
        transform.position = position;
    }

    private void FixedUpdate ()
    {
        if ( gameObject.activeInHierarchy )
            Animate();
    }
}