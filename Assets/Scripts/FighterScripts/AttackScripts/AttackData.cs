using UnityEngine;

[CreateAssetMenu(menuName = "Fighter/Attack Data", fileName = "NewAttack")]
public class AttackData : ScriptableObject
{
    public enum KnockbackType { Away, Up, Down, UpAway, DownAway}

    [Header("Identity")]
    public string attackName;
    public string motionInput;  // e.g. "236" or "→↓↘"
    public string buttonName;   // e.g. "LP", "HK"

    [Header("Frame Data (seconds)")]
    [Tooltip("Startup duration before the attack becomes active.")]
    public float startupTime;
    [Tooltip("Active duration while the hitbox is active.")]
    public float activeTime;
    [Tooltip("Recovery duration after the attack.")]
    public float recoveryTime;

    [Header("Hit Properties")]
    [Tooltip("Damage dealt to targets hit by this attack.")]
    public float damage;
    [Tooltip("Duration of hitstun on the target.")]
    public float hitstunTime;
    [Tooltip("Horizontal knockback strength.")]
    public float knockbackStrength;
    [Tooltip("Direction type of knockback.")]
    public KnockbackType knockbackType;

    [Header("Hitbox Data")]
    [Tooltip("Prefab for the hitbox spawned during active frames.")]
    public GameObject hitboxPrefab;
    [Tooltip("Local offset from the fighter’s origin.")]
    public Vector2 hitboxOffset;
    [Tooltip("Size of the hitbox area if prefab supports dynamic resizing.")]
    public Vector2 hitboxSize = Vector2.one;
    [Tooltip("Optional rotation of the hitbox in degrees.")]
    public float hitboxRotation;
    [Tooltip("Parent transform (optional) where hitbox should attach, otherwise spawns at fighter root.")]
    public bool attachToFighter = false;

    [Header("Animation / VFX")]
    [Tooltip("Trigger name for the Animator.")]
    public string animationTrigger;
    [Tooltip("Optional sound effect to play when the attack begins.")]
    public AudioClip soundEffect;
    
    public AttackData Clone()
    {
        AttackData clone = CreateInstance<AttackData>();

        clone.attackName = attackName;
        clone.motionInput = motionInput;
        clone.buttonName = buttonName;

        clone.startupTime = startupTime;
        clone.activeTime = activeTime;
        clone.recoveryTime = recoveryTime;

        clone.damage = damage;
        clone.hitstunTime = hitstunTime;
        clone.knockbackStrength = knockbackStrength;
        clone.knockbackType = knockbackType;

        clone.hitboxPrefab = hitboxPrefab;
        clone.hitboxOffset = hitboxOffset;
        clone.hitboxSize = hitboxSize;
        clone.hitboxRotation = hitboxRotation;
        clone.attachToFighter = attachToFighter;

        clone.animationTrigger = animationTrigger;
        clone.soundEffect = soundEffect;

        return clone;
    }
    
    public override bool Equals(object obj)
    {
        // Fast null & type check
        if (obj is not AttackData other)
            return false;

        // Compare only on attackName (case-sensitive, or use OrdinalIgnoreCase if you want)
        return string.Equals(this.attackName, other.attackName, System.StringComparison.Ordinal);
    }
    
    public override int GetHashCode()
    {
        // Must be consistent with Equals — use the same property
        return attackName != null ? attackName.GetHashCode() : 0;
    }
}