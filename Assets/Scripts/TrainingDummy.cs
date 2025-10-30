using UnityEngine;
using System.Collections;

/// <summary>
/// Training dummy that uses FighterMovement gravity and grounding,
/// can be hit, knocked back, and bounces off walls.
/// </summary>
[RequireComponent(typeof(FighterMovement))]
[RequireComponent(typeof(CheckGrounded))]
[RequireComponent(typeof(FighterBounds))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class TrainingDummy : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public float maxHealth = 9999f;
    public float currentHealth;
    public float hitFriction = 6f;
    public float wallBounce = 0.4f;
    public float flashDuration = 0.1f;

    private FighterMovement movement;
    private CheckGrounded checkGrounded;
    private FighterBounds bounds;
    private SpriteRenderer sprite;

    private Vector2 velocity;
    private bool beingPushed;
    private bool flashing;
    private bool onGround;

    void Awake()
    {
        movement = GetComponent<FighterMovement>();
        checkGrounded = GetComponent<CheckGrounded>();
        bounds = GetComponent<FighterBounds>();
        sprite = GetComponent<SpriteRenderer>();

        currentHealth = maxHealth;
    }

    void Update()
    {
        bool grounded = checkGrounded.IsGrounded();

        // Apply gravity to dummy's own Y velocity (same as FighterMovement)
        if (!grounded)
        {
            velocity.y += movement.gravity * movement.weight * Time.deltaTime;
        }
        else
        {
            if (!onGround)
            {
                // Just landed
                checkGrounded.ClampToGround(transform);
                velocity.y = 0f;
            }
            else if (velocity.y < 0f)
            {
                velocity.y = 0f;
            }
        }

        // Horizontal decay from knockback
        if (beingPushed)
        {
            velocity.x = Mathf.Lerp(velocity.x, 0f, hitFriction * Time.deltaTime);
            if (grounded && Mathf.Abs(velocity.x) < 0.05f && Mathf.Abs(velocity.y) < 0.05f)
            {
                beingPushed = false;
                velocity.x = 0f;
            }
        }
        

        // Move dummy
        transform.position += (Vector3)(velocity * Time.deltaTime);

        onGround = grounded;
    }

    // === IDamageable ===
    public void ReceiveHit(AttackData data, Transform attacker)
    {
        print("Dummy got hit");
        if (data == null || attacker == null) return;

        float dirX = (transform.position.x > attacker.position.x) ? 1f : -1f;
        print(dirX);
        Vector2 knockback = Vector2.zero;

        switch (data.knockbackType)
        {
            case AttackData.KnockbackType.Away:
                knockback = new Vector2(dirX * data.knockbackStrength, 0);
                break;
            case AttackData.KnockbackType.Up:
                knockback = new Vector2(0, data.knockbackStrength);
                break;
            case AttackData.KnockbackType.Down:
                knockback = new Vector2(0, -data.knockbackStrength);
                break;
            case AttackData.KnockbackType.UpAway:
                knockback = new Vector2(dirX * data.knockbackStrength, data.knockbackStrength);
                break;
            case AttackData.KnockbackType.DownAway:
                knockback = new Vector2(dirX * data.knockbackStrength, -data.knockbackStrength);
                break;
        }

        velocity = knockback / Mathf.Max(0.1f, movement.weight);
        beingPushed = true;
        currentHealth -= data.damage;

        if (!flashing) StartCoroutine(FlashWhite());
    }

    private IEnumerator FlashWhite()
    {
        flashing = true;
        Color original = sprite.color;
        sprite.color = Color.white;
        yield return new WaitForSeconds(flashDuration);
        sprite.color = original;
        flashing = false;
    }

    // Wall bounce using FighterBounds' collision triggers
    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Wall")) return;

        var myBounds = GetComponent<Collider2D>().bounds;
        var wallBounds = other.bounds;

        bool hitLeft = myBounds.min.x <= wallBounds.max.x && myBounds.center.x < wallBounds.center.x;
        bool hitRight = myBounds.max.x >= wallBounds.min.x && myBounds.center.x > wallBounds.center.x;

        if ((hitLeft && velocity.x < 0f) || (hitRight && velocity.x > 0f))
            velocity.x *= -wallBounce;
    }
}
