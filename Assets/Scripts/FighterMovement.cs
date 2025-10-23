using UnityEngine;

[RequireComponent(typeof(CheckGrounded), typeof(Collider2D))]
public class FighterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float weight = 1f;
    public float jumpForce = 5f;

    [Header("Crouch Settings")]
    [Range(0.1f, 1f)] public float crouchScaleY = 0.5f;
    public float crouchTransitionSpeed = 15f; // optional: smoothness factor

    private CheckGrounded checkGrounded;
    private BoxCollider2D col;

    private float verticalVel;
    private bool wasGroundedLastFrame;
    private bool isCrouched = false;

    // saved values
    private Vector3 originalScale;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    void Awake()
    {
        checkGrounded = GetComponent<CheckGrounded>();
        col = GetComponent<BoxCollider2D>();

        originalScale = transform.localScale;
        originalColliderSize = col.size;
        originalColliderOffset = col.offset;
    }

    public void Move(float moveX)
    {
        transform.position += new Vector3(moveX * moveSpeed * Time.deltaTime, 0f, 0f);
    }

    public void Jump()
    {
        if (checkGrounded.IsGrounded())
            verticalVel = jumpForce;
    }

    public bool ApplyGravity(float gravMult)
    {
        bool grounded = checkGrounded.IsGrounded();

        if (!grounded)
        {
            if (gravMult > 1f)
            {
                verticalVel += gravity * gravMult * Time.deltaTime;
            }
            else
            {
                verticalVel += gravity * weight * Time.deltaTime;
            }
        }
        else
        {
            if (!wasGroundedLastFrame)
            {
                checkGrounded.ClampToGround(transform);
                verticalVel = 0f;
            }
            else
            {
                verticalVel = Mathf.Max(verticalVel, 0f);
            }
        }

        transform.position += new Vector3(0, verticalVel, 0) * Time.deltaTime;
        wasGroundedLastFrame = grounded;
        return grounded;
    }

    public void SetCrouch(bool crouch)
    {
        if (isCrouched == crouch) return;
        isCrouched = crouch;

        float h = originalScale.y;
        float newH = h * crouchScaleY;
        float delta = (h - newH) / 2f;

        // Base position before crouch (where feet currently are)
        Vector3 pos = transform.position;

        if (crouch)
        {
            // Shrink the visible body
            transform.localScale = new Vector3(originalScale.x, newH, originalScale.z);
            transform.position = new Vector3(pos.x, pos.y - delta, pos.z);

            // Shrink the collider too
            col.size = new Vector2(originalColliderSize.x, originalColliderSize.y * crouchScaleY);
            col.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - delta);
        }
        else
        {
            // Restore scale & position
            transform.localScale = originalScale;
            transform.position = new Vector3(pos.x, pos.y + delta, pos.z);

            // Restore collider
            col.size = originalColliderSize;
            col.offset = originalColliderOffset;

            // Re-snap to ground to avoid floating from scale change
            checkGrounded.ClampToGround(transform);
        }
    }

    public float GetVerticalVelocity() => verticalVel;
}
