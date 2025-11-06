using UnityEngine;

public interface IDamageable
{
    void ReceiveHit(AttackData data, Transform attacker);

    void ApplyKnockback(AttackData data, float directionX);

    void Loss();
}