using Unity.Mathematics;
using UnityEngine;

public class SineAnimate : MonoBehaviour
{
    [SerializeField] private float sineStrength = 0.5f;

    private void Animate ()
    {
        var position = transform.position;
        position.y += math.sin(Time.time) * sineStrength;
        transform.position = position;
    }

    private void FixedUpdate ()
    {
        Animate();
    }
}