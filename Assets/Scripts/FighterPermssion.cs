using UnityEngine;

public class FighterPermissions
{
    public void Resolve(FighterContext ctx)
    {
        // 1️⃣ Hitstun always overrides everything
        if (ctx.Hitstun == FighterContext.HitstunState.Hit)
        {
            ctx.CanAcceptNewAttack = false;
            ctx.CanMoveHorizontally = false;
            ctx.CanJump = false;
            ctx.CanCrouch = false;
            return;
        }

        // 2️⃣ Blocking rules
        if (ctx.Block == FighterContext.BlockState.Blocking)
        {
            ctx.CanAcceptNewAttack = false;

            // Allow movement if standing block, lock if crouch-blocking
            if (ctx.Movement == FighterContext.MovementState.Crouch)
            {
                ctx.CanMoveHorizontally = false;
                ctx.CanJump = false;
                ctx.CanCrouch = true;
            }
            else
            {
                ctx.CanMoveHorizontally = true;
                ctx.CanJump = true;     
                ctx.CanCrouch = true;
            }
            return;
        }

        // 3️⃣ Attacking rules
        switch (ctx.Attack)
        {
            case FighterContext.AttackState.Startup:
            case FighterContext.AttackState.Active:
                ctx.CanAcceptNewAttack = false;
                ctx.CanMoveHorizontally = false;
                ctx.CanJump = false;
                ctx.CanCrouch = false;
                return;

            case FighterContext.AttackState.Recovery:
                ctx.CanAcceptNewAttack = false;
                ctx.CanMoveHorizontally = true;
                ctx.CanJump = false;
                ctx.CanCrouch = false;
                return;
        }

        // 4️⃣ Default: free movement
        ctx.CanAcceptNewAttack = true;
        ctx.CanMoveHorizontally = true;
        ctx.CanJump = true;
        ctx.CanCrouch = true;
    }
}