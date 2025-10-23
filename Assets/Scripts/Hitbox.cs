using UnityEngine;

/// <summary>
/// Modular Hitbox that reads parameters from AttackData
/// and applies damage/knockback to any IDamageable.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class Hitbox : MonoBehaviour
{
    [HideInInspector] public AttackData attackData;
    [HideInInspector] public Transform attacker;

    private BoxCollider2D col;
    private float timer;
    private bool active;
    private bool hasHit;

    void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;
    }

    void Update()
    {
        if (!active) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Disable();
        }
    }

    /// <summary>
    /// Initializes this hitbox using AttackData values.
    /// </summary>
    public void Initialize(AttackData data, Transform source)
    {
        attackData = data;
        attacker = source;

        if (col != null)
        {
            col.size = data.hitboxSize;
            col.offset = data.hitboxOffset;
        }

        transform.localRotation = Quaternion.Euler(0f, 0f, data.hitboxRotation);
        active = true;
        hasHit = false;
        timer = data.activeTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!active || hasHit) return;

        IDamageable dmg = other.GetComponent<IDamageable>();
        if (dmg != null && attacker != null)
        {
            dmg.ReceiveHit(attackData, attacker);
            hasHit = true; // prevent multi-hits per activation
        }
    }

    private void Disable()
    {
        active = false;
        Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (attackData == null) return;
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(attackData.hitboxOffset, attackData.hitboxSize);
    }
#endif
}