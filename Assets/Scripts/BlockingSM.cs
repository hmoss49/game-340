// BlockSM.cs
using UnityEngine;

public class BlockSM
{
    private readonly FighterContext ctx;

    public BlockSM(FighterContext context)
    {
        ctx = context;
    }

    public void Tick(float dt, FighterInput input)
    {
        // If we're in hitstun, blocking doesn't apply
        if (ctx.Hitstun == FighterContext.HitstunState.Hit)
        {
            ctx.Block = FighterContext.BlockState.NotBlocking;
            return;
        }

        // Must be holding back while grounded
        if (input.BackHeld && input.OnGround)
        {
            if (ctx.Block != FighterContext.BlockState.Blocking)
            {
                ctx.Block = FighterContext.BlockState.Blocking;
                // TODO: Trigger block start animation here
                // e.g. Animator.SetBool("Blocking", true);
            }
        }
        else
        {
            if (ctx.Block == FighterContext.BlockState.Blocking)
            {
                ctx.Block = FighterContext.BlockState.NotBlocking;
                // TODO: Trigger block end animation here
                // e.g. Animator.SetBool("Blocking", false);
            }
        }
    }
}