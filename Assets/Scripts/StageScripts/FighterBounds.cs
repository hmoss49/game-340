// FighterBounds.cs – trigger-based wall handling for transform movement
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FighterBounds : MonoBehaviour
{
    private Collider2D fighterCol;
    public float pushSmooth = 15f;      // higher = faster correction
    public float tolerance = 0.001f;    // tiny buffer to avoid jitter

    void Awake()
    {
        fighterCol = GetComponent<Collider2D>();
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Wall")) return;

        Bounds f = fighterCol.bounds;
        Bounds w = other.bounds;

        // compute horizontal overlap
        float overlapLeft = f.max.x - w.min.x;
        float overlapRight = w.max.x - f.min.x;
        float push = 0f;

        if (f.center.x < w.center.x && overlapLeft > tolerance)
            push = -overlapLeft;
        else if (f.center.x > w.center.x && overlapRight > tolerance)
            push = overlapRight;

        if (Mathf.Abs(push) > 0f)
        {
            // ease out instead of snapping
            Vector3 target = transform.position + new Vector3(push, 0, 0);
            transform.position = Vector3.Lerp(
                transform.position,
                target,
                Time.deltaTime * pushSmooth
            );
        }
    }
}