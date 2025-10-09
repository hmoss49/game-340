using UnityEngine;
using System;

public class AttackSM
{
    public enum AttackButton { None, LP, MP, HP, LK, MK, HK }

    private readonly FighterContext ctx;
    private float timer = 0f;

    // Default durations (seconds) – feel free to tune per button later
    public float StartupSeconds = 0.12f;
    public float ActiveSeconds  = 0.08f;
    public float RecoverySeconds = 0.25f;

    public Action OnHitboxEnable;            // enable hitbox during Active
    public Action OnHitboxDisable;           // disable hitbox
    public Action<string> OnAnimTrigger;     // e.g., "LightPunch", "HeavyKick"

    private AttackButton currentButton = AttackButton.None;

    public AttackSM(FighterContext context) { ctx = context; }

    public void BeginAttack(AttackButton btn)
    {
        if (ctx.Attack != FighterContext.AttackState.None) return;

        currentButton = btn;
        ctx.Attack = FighterContext.AttackState.Startup;
        timer = StartupSeconds;

        OnAnimTrigger?.Invoke(TriggerNameFor(btn, phase: "Start"));
    }

    public void Tick(float dt)
    {
        if (ctx.Hitstun == FighterContext.HitstunState.Hit)
        {
            EndAttack();
            return;
        }

        switch (ctx.Attack)
        {
            case FighterContext.AttackState.Startup:
                timer -= dt;
                if (timer <= 0f)
                {
                    ctx.Attack = FighterContext.AttackState.Active;
                    timer = ActiveSeconds;
                    OnHitboxEnable?.Invoke();
                    OnAnimTrigger?.Invoke(TriggerNameFor(currentButton, "Active"));
                }
                break;

            case FighterContext.AttackState.Active:
                timer -= dt;
                if (timer <= 0f)
                {
                    ctx.Attack = FighterContext.AttackState.Recovery;
                    timer = RecoverySeconds;
                    OnHitboxDisable?.Invoke();
                    OnAnimTrigger?.Invoke(TriggerNameFor(currentButton, "Recover"));
                }
                break;

            case FighterContext.AttackState.Recovery:
                timer -= dt;
                if (timer <= 0f) EndAttack();
                break;
        }
    }

    public void EndAttack()
    {
        OnHitboxDisable?.Invoke();
        ctx.Attack = FighterContext.AttackState.None;
        timer = 0f;
        OnAnimTrigger?.Invoke(TriggerNameFor(currentButton, "End"));
        currentButton = AttackButton.None;
    }

    private string TriggerNameFor(AttackButton btn, string phase)
    {
        // You can hook these to Animator triggers as you like.
        // Examples: "LightPunch", "MediumKickActive", etc.
        switch (btn)
        {
            case AttackButton.LP: return phase switch
                { "Start" => "LightPunch", "Active" => "LightPunchActive", "Recover" => "LightPunchRecover", _ => "LightPunchEnd" };
            case AttackButton.MP: return phase switch
                { "Start" => "MediumPunch", "Active" => "MediumPunchActive", "Recover" => "MediumPunchRecover", _ => "MediumPunchEnd" };
            case AttackButton.HP: return phase switch
                { "Start" => "HeavyPunch", "Active" => "HeavyPunchActive", "Recover" => "HeavyPunchRecover", _ => "HeavyPunchEnd" };
            case AttackButton.LK: return phase switch
                { "Start" => "LightKick", "Active" => "LightKickActive", "Recover" => "LightKickRecover", _ => "LightKickEnd" };
            case AttackButton.MK: return phase switch
                { "Start" => "MediumKick", "Active" => "MediumKickActive", "Recover" => "MediumKickRecover", _ => "MediumKickEnd" };
            case AttackButton.HK: return phase switch
                { "Start" => "HeavyKick", "Active" => "HeavyKickActive", "Recover" => "HeavyKickRecover", _ => "HeavyKickEnd" };
            default: return "AttackEnd";
        }
    }
}
