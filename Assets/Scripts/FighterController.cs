using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class FighterController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;
    public float jumpForce = 5f;
    public CheckGrounded checkGrounded;
    
    private float horizontalMovement = 0f;
    private float verticalMovement = 0f;
    private bool isGrounded;
    private bool wasGroundedLastFrame = false;

    void Update() {
        MovePlayer();
        ApplyGravity();
    }
    
// This will be called automatically by PlayerInput (Send Messages mode)
    void OnMove(InputValue value)
    {
        horizontalMovement = value.Get<float>() * moveSpeed;
    }
    
    void MovePlayer()
    {
        transform.position += new Vector3(horizontalMovement, 0,0)* Time.deltaTime;
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            JumpPlayer();
        }
    }

    void JumpPlayer()
    {
        if (checkGrounded.IsGrounded())
        {
            verticalMovement = jumpForce;
        }
    }

    void ApplyGravity()
    {
        bool isGroundedNow = checkGrounded.IsGrounded();

        if (!isGroundedNow)
        {
            // Apply gravity if not grounded
            verticalMovement += gravity * Time.deltaTime;
        }
        else
        {
            if (!wasGroundedLastFrame)
            {
                // Just landed this frame → clamp to ground
                checkGrounded.ClampToGround(transform);
                verticalMovement = 0f; // reset vertical velocity
            }
            else
            {
                // Already grounded → don’t re-clamp every frame
                verticalMovement = Mathf.Max(verticalMovement, 0f);
            }
        }

        // Apply vertical movement
        transform.position += new Vector3(0, verticalMovement, 0) * Time.deltaTime;

        // Update grounded state
        wasGroundedLastFrame = isGroundedNow;
    }



    void OnPunch(InputValue value)
    {
        if (value.isPressed)
        {
            
        }
    }
}