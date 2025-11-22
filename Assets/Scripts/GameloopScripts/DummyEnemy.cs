using System;
using UnityEngine;
using System.Collections;

public class DummyEnemy : Enemy, IDamageable
{
    [Header("Stats")]
    public float flashDuration = 0.1f;

    public float deathDuration;
    
    private FighterMovement movement;
    private FighterBounds bounds;
    private SpriteRenderer sprite;
    private Collider2D dummyCollider;
    private Color originalColor;
    
    private bool beingPushed;
    private bool flashing;
    private bool onGround;

    protected override void Awake()
    {
        base.Awake();
        movement = GetComponent<FighterMovement>();
        bounds = GetComponent<FighterBounds>();
        sprite = GetComponent<SpriteRenderer>();
        dummyCollider = GetComponent<Collider2D>();
        originalColor = sprite.color;
        currentHealth.Value = maxHealth;
    }

    void Update()
    {
        movement.ApplyMotionPhysics(1f);
    }

    // === IDamageable ===
    public void ReceiveHit(AttackData data, Transform attacker)
    {
        if (data == null || attacker == null) return;

        float dirX = CalculateDirection(attacker);
        ApplyKnockback(data, dirX);
        currentHealth.Value -= data.damage;
        if (!flashing) StartCoroutine(FlashWhite());

        if (currentHealth.Value <= 0)
        {
            Loss();
        }
    }

    public void ApplyKnockback(AttackData data, float directionX)
    {
        Vector2 knockback = Vector2.zero;

        switch (data.knockbackType)
        {
            case AttackData.KnockbackType.Away:
                knockback = new Vector2(directionX * data.knockbackStrength, 0);
                break;
            case AttackData.KnockbackType.Up:
                knockback = new Vector2(0, data.knockbackStrength);
                break;
            case AttackData.KnockbackType.Down:
                knockback = new Vector2(0, -data.knockbackStrength);
                break;
            case AttackData.KnockbackType.UpAway:
                knockback = new Vector2(directionX * data.knockbackStrength, data.knockbackStrength/2);
                break;
            case AttackData.KnockbackType.DownAway:
                knockback = new Vector2(directionX * data.knockbackStrength, -data.knockbackStrength/2);
                break;
        }

        movement.ApplyKnockback(knockback);
    }

    public void Loss()
    {
        StartCoroutine(Death());
    }

    public float CalculateDirection(Transform attacker)
    {
        return (transform.position.x > attacker.position.x) ? 1f : -1f;
    }

    private IEnumerator FlashWhite()
    {
        flashing = true;
        sprite.color = Color.white;
        yield return new WaitForSeconds(flashDuration);
        sprite.color = originalColor;
        flashing = false;
    }

    private IEnumerator Death()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(deathDuration);
        if (!isDefeated.Value)
            isDefeated.Value = true;
        Destroy(gameObject);
    }
}
