// FighterContext.cs
using UnityEngine;

public class FighterContext
{
    // Movement states
    public enum MovementState { Idle, Walking, Jumping, InAir, Falling, Crouch }
    public MovementState Movement = MovementState.Idle;

    // Attack states
    public enum AttackState { None, Startup, Active, Recovery }
    public AttackState Attack = AttackState.None;

    // Blocking states
    public enum BlockState { NotBlocking, Blocking }
    public BlockState Block = BlockState.NotBlocking;

    // Hitstun states
    public enum HitstunState { NotHit, Hit }
    public HitstunState Hitstun = HitstunState.NotHit;

    // Permission flags — resolved by FighterController each frame
    public bool CanAcceptNewAttack = true;
    public bool CanMoveHorizontally = true;
    public bool CanJump = true;
    public bool CanCrouch = true;

    // Optional: you can track grounded and facing info here if useful
    public bool IsGrounded;
    public bool FacingRight = true;

    // For debugging in Inspector (optional)
    public override string ToString()
    {
        return $"Move:{Movement}, Atk:{Attack}, Block:{Block}, Hit:{Hitstun}";
    }
}