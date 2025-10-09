// HitstunSM.cs
using UnityEngine;

public class HitstunSM
{
    private readonly FighterContext ctx;
    private float timer = 0f;

    public HitstunSM(FighterContext context)
    {
        ctx = context;
    }

    // Call this when the fighter gets hit
    public void ApplyHit(float hitstunSeconds)
    {
        ctx.Hitstun = FighterContext.HitstunState.Hit;
        timer = hitstunSeconds;
        // TODO: trigger a hit animation here, e.g. Animator.SetTrigger("Hit");
    }

    // Update every frame
    public void Tick(float deltaTime)
    {
        if (ctx.Hitstun == FighterContext.HitstunState.Hit)
        {
            timer -= deltaTime;
            if (timer <= 0f)
            {
                ctx.Hitstun = FighterContext.HitstunState.NotHit;
                // TODO: trigger recover animation here if needed
            }
        }
    }
}