using UnityEngine;

[RequireComponent(typeof(CheckGrounded), typeof(Collider2D))]
public class FighterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float weight = 1f;
    public float jumpForce = 5f;
    public float friction = 0.5f;

    [Header("Crouch Settings")]
    [Range(0.1f, 1f)] public float crouchScaleY = 0.5f;
    public float crouchTransitionSpeed = 15f; // optional: smoothness factor

    private CheckGrounded checkGrounded;
    private Collider2D col;
    
    private bool wasGroundedLastFrame;
    private bool isCrouched;
    
    private Vector2 velocity;
    private bool beingPushed;
    private bool grounded;

    // saved values
    private Vector3 originalScale;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    void Awake()
    {
        checkGrounded = GetComponent<CheckGrounded>();
        col = GetComponent<Collider2D>();

        originalScale = transform.localScale;
        originalColliderSize = col.transform.localScale;
        originalColliderOffset = col.offset;
    }

    void Update()
    {
        grounded = checkGrounded.IsGrounded();
    }

    public void Move(float moveX)
    {
        transform.position += new Vector3(moveX * moveSpeed * Time.deltaTime, 0f, 0f);
    }

    public void Jump()
    {
        if (checkGrounded.IsGrounded())
            velocity.y = jumpForce;
    }

    public bool ApplyGravity(float gravMult)
    {
        if (!grounded)
        {
            if (gravMult > 1f)
            {
                velocity.y += gravity * gravMult * Time.deltaTime;
            }
            else
            {
                velocity.y += gravity * weight * Time.deltaTime;
            }
        }
        else
        {
            if (!wasGroundedLastFrame)
            {
                checkGrounded.ClampToGround(transform);
                velocity.y = 0f;
            }
            else
            {
                velocity.y = Mathf.Max(velocity.y, 0f);
            }
        }
        
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
            col.transform.localScale = new Vector2(originalColliderSize.x, originalColliderSize.y * crouchScaleY);
            col.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - delta);
        }
        else
        {
            // Restore scale & position
            transform.localScale = originalScale;
            transform.position = new Vector3(pos.x, pos.y + delta, pos.z);

            // Restore collider
            col.transform.localScale = originalColliderSize;
            col.offset = originalColliderOffset;

            // Re-snap to ground to avoid floating from scale change
            checkGrounded.ClampToGround(transform);
        }
    }

    public void ApplyHorizontalFriction()
    {
        if (beingPushed)
        {
            velocity.x = Mathf.Lerp(velocity.x, 0f, friction * Time.deltaTime);
            if (grounded && Mathf.Abs(velocity.x) < 0.05f && Mathf.Abs(velocity.y) < 0.05f)
            {
                beingPushed = false;
                velocity.x = 0f;
            }
        }
    }

    public void ApplyMovement()
    {
        transform.position += new Vector3(velocity.x, velocity.y, 0) * Time.deltaTime;
    }

    public void ApplyKnockback(Vector2 knockback)
    {
        velocity += knockback;
        beingPushed = true;
    }

    public bool ApplyMotionPhysics(float gravMult)
    {
        ApplyGravity(gravMult);
        ApplyHorizontalFriction();
        ApplyMovement();
        return grounded;
    }

    public Vector2 GetVelocity() => velocity;
}
