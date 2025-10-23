using UnityEngine;

public interface IDamageable
{
    void ReceiveHit(AttackData data, Transform attacker);
}