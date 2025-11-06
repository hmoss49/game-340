using UnityEngine;
using System;

public class MovementSM
{
    private readonly FighterContext ctx;

    public Action<float> OnMove;
    public Action OnJump;
    public Action<bool> OnCrouch;
    public Action<string> OnAnimState;

    public MovementSM(FighterContext context) { ctx = context; }

    public void Tick(float dt, FighterInput input)
    {
        if (ctx.Hitstun == FighterContext.HitstunState.Hit)
        {
            OnMove?.Invoke(0f);
            OnCrouch?.Invoke(false);
            return;
        }

        bool horizontalApplied = false;

        if (input.OnGround)
        {
            if (input.CrouchHeld)
            {
                ctx.Movement = FighterContext.MovementState.Crouch;
                OnAnimState?.Invoke("Crouch");
                OnCrouch?.Invoke(true);
                
                float crouchMoveX = input.MoveX * 0.3f;
                OnMove?.Invoke(crouchMoveX);
                horizontalApplied = true;
            }
            else if (Mathf.Abs(input.MoveX) > 0.1f)
            {
                ctx.Movement = FighterContext.MovementState.Walking;
                OnAnimState?.Invoke("Walk");
                OnCrouch?.Invoke(false);
            }
            else
            {
                ctx.Movement = FighterContext.MovementState.Idle;
                OnAnimState?.Invoke("Idle");
                OnCrouch?.Invoke(false);
            }

            if (ctx.CanJump && input.JumpPressed && !input.CrouchHeld)
            {
                ctx.Movement = FighterContext.MovementState.Jumping;
                OnAnimState?.Invoke("Jump");
                OnJump?.Invoke();
            }
        }
        else
        {
            if (input.Velocity.y > 0.01f)
            {
                ctx.Movement = FighterContext.MovementState.InAir;
                OnAnimState?.Invoke("InAir");
            }
            else
            {
                ctx.Movement = FighterContext.MovementState.Falling;
                OnAnimState?.Invoke("Fall");
            }
            OnCrouch?.Invoke(false);
        }

        // Apply horizontal movement respecting permissions,
        // but don't override if we already applied it (e.g., crouch branch).
        if (!horizontalApplied)
        {
            if (ctx.CanMoveHorizontally)
                OnMove?.Invoke(input.MoveX);
            else
                OnMove?.Invoke(0f);
        }
    }
}
