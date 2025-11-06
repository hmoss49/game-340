using UnityEngine;
using System.Collections;

/// <summary>
/// Training dummy that uses FighterMovement gravity and grounding,
/// can be hit, knocked back, and bounces off walls.
/// </summary>
[RequireComponent(typeof(FighterMovement))]
[RequireComponent(typeof(FighterBounds))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class TrainingDummy : Enemy, IDamageable
{
    [Header("Stats")]
    public float flashDuration = 0.1f;
    public float respawnDuration;
    public GameObject respawnPoint;

    private FighterMovement movement;
    private FighterBounds bounds;
    private SpriteRenderer sprite;
    private Collider2D dummyCollider;
    private Color originalColor;
    
    private bool beingPushed;
    private bool flashing;
    private bool onGround;

    void Awake()
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
                knockback = new Vector2(directionX * data.knockbackStrength, data.knockbackStrength);
                break;
            case AttackData.KnockbackType.DownAway:
                knockback = new Vector2(directionX * data.knockbackStrength, -data.knockbackStrength);
                break;
        }

        movement.ApplyKnockback(knockback / Mathf.Max(0.1f, movement.weight));
    }

    public void Loss()
    {
        StartCoroutine(Respawn());
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

    private IEnumerator Respawn()
    {
        sprite.color = originalColor;
        sprite.enabled = false;
        dummyCollider.enabled = false;
        yield return new WaitForSeconds(0.2f);
        isDefeated.Value = true;
        yield return new WaitForSeconds(respawnDuration);
        currentHealth.Value = maxHealth;
        isDefeated.Value = false;
        sprite.enabled = true;
        dummyCollider.enabled = true;
    }
}
