using UnityEngine;

[CreateAssetMenu(menuName = "Fighter/Upgrade Data", fileName = "NewUpgrade")]
public class UpgradeData : ScriptableObject
{
    public enum UpgradeType { PlayerStats, AttackStats, NewMove}
    public enum PlayerStatType {Speed, Jump, Weight}
    public enum AttackStatType {Damage, Knockback, HitboxSize}
    
    public string upgradeName;

    public UpgradeType upgradeType;
    public string newMoveLocation;
    public PlayerStatType playerStatType;
    public int playerStatValue;
    public AttackStatType attackStatType;
    public int attackStatValue;
    
    public override bool Equals(object obj)
    {
        // Fast null & type check
        if (obj is not UpgradeData other)
            return false;

        // Compare only on attackName (case-sensitive, or use OrdinalIgnoreCase if you want)
        return string.Equals(this.upgradeName, other.upgradeName, System.StringComparison.Ordinal);
    }
    
    public override int GetHashCode()
    {
        // Must be consistent with Equals — use the same property
        return upgradeName != null ? upgradeName.GetHashCode() : 0;
    }
}