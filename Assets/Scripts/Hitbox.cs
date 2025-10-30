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

    private BoxCollider2D _col;
    private float _timer;
    private bool _active;
    private bool _hasHit;

    void Awake()
    {
        _col = GetComponent<BoxCollider2D>();
        _col.isTrigger = true;
    }

    void Update()
    {
        if (!_active) return;

        _timer -= Time.deltaTime;
        if (_timer <= 0f)
        {
            Disable();
        }
    }
    
    /// Initializes this hitbox using AttackData values.
    public void Initialize(AttackData data, Transform source, float facingDirection)
    {
        attackData = data;
        attacker = source;

        if (_col != null)
        {
            _col.size = data.hitboxSize;
            _col.offset = new Vector2(data.hitboxOffset.x * facingDirection, data.hitboxOffset.y);
        }
        transform.localRotation = Quaternion.Euler(0f, 0f, data.hitboxRotation);
        _active = true;
        _hasHit = false;
        _timer = data.activeTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_active || _hasHit) return;

        IDamageable dmg = other.GetComponent<IDamageable>();
        if (dmg != null && attacker != null)
        {
            dmg.ReceiveHit(attackData, attacker);
            _hasHit = true; // prevent multi-hits per activation
        }
    }

    private void Disable()
    {
        _active = false;
        Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (attackData == null) return;
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(_col.offset, _col.size);
    }
#endif
}