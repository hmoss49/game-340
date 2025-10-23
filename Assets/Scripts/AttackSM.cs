using UnityEngine;
using System;

public class AttackSM
{
    private readonly FighterContext ctx;

    private float timer;
    private AttackData currentAttack;
    private Transform attacker;

    // Optional hooks
    public Action<string> OnAnimTrigger;
    public Action OnAttackEnd;

    public AttackSM(FighterContext context) { ctx = context; }

    public bool IsAttacking => ctx.Attack != FighterContext.AttackState.None;

    /// <summary>Begin an attack using data (startup/active/recovery + hitbox info).</summary>
    public void BeginAttack(AttackData data, Transform attackerTransform)
    {
        if (data == null || IsAttacking) return;

        currentAttack = data;
        attacker = attackerTransform;

        ctx.Attack = FighterContext.AttackState.Startup;
        timer = currentAttack.startupTime;

        if (!string.IsNullOrEmpty(currentAttack.animationTrigger))
            OnAnimTrigger?.Invoke(currentAttack.animationTrigger);
    }

    public void Tick(float dt)
    {
        if (currentAttack == null || ctx.Attack == FighterContext.AttackState.None) return;

        timer -= dt;

        switch (ctx.Attack)
        {
            case FighterContext.AttackState.Startup:
                if (timer <= 0f)
                {
                    ctx.Attack = FighterContext.AttackState.Active;
                    timer = currentAttack.activeTime;
                    SpawnHitbox();
                }
                break;

            case FighterContext.AttackState.Active:
                if (timer <= 0f)
                {
                    ctx.Attack = FighterContext.AttackState.Recovery;
                    timer = currentAttack.recoveryTime;
                }
                break;

            case FighterContext.AttackState.Recovery:
                if (timer <= 0f)
                {
                    ctx.Attack = FighterContext.AttackState.None;
                    var ended = currentAttack;
                    currentAttack = null;
                    OnAttackEnd?.Invoke();
                }
                break;
        }
    }

    private void SpawnHitbox()
    {
        if (currentAttack == null || currentAttack.hitboxPrefab == null || attacker == null) return;
        currentAttack.hitboxOffset.x *= ctx.FacingDirection;
        
        var go = UnityEngine.Object.Instantiate(currentAttack.hitboxPrefab, attacker.position, Quaternion.identity);
        var hb = go.GetComponent<Hitbox>();
        if (hb != null)
        {
            hb.Initialize(currentAttack, attacker);
            if (currentAttack.attachToFighter)
                go.transform.SetParent(attacker, worldPositionStays: true);
        }
    }
}
