using UnityEngine;

/// <summary>
/// Prevents two fighters or a fighter + dummy from overlapping,
/// creating a soft push effect without full physics.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class BodyPush : MonoBehaviour
{
    public float pushForce = 8f;

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Only push fighters or dummies
        if (!collision.collider.CompareTag("Player") && !collision.collider.CompareTag("Dummy"))
            return;

        // Get direction to push away
        Vector2 direction = (collision.collider.transform.position.x < transform.position.x)
            ? Vector2.right : Vector2.left;

        // Apply small translation
        collision.collider.transform.position += (Vector3)(direction * pushForce * Time.deltaTime);
    }
}