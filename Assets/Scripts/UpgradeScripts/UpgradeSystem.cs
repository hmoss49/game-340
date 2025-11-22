using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UpgradeSystem : MonoBehaviour
{
    public Enemy enemy;
    public FighterController player;
    public GameUI gameUI;
    public List<UpgradeData> upgrades = new List<UpgradeData>();

    private void Start()
    {
        enemy = FindFirstObjectByType<Enemy>();
        player = FindFirstObjectByType<FighterController>();

        if (enemy != null && enemy.isDefeated != null)
            enemy.isDefeated.ChangeEvent += OnDefeatedEnemy;
    }

    private void OnDisable()
    {
        if (enemy != null && enemy.isDefeated != null)
            enemy.isDefeated.ChangeEvent -= OnDefeatedEnemy;
    }

    private void OnDefeatedEnemy(bool defeated)
    {
        if (!defeated) return;
        
        List<UpgradeData> selectedUpgrades = PickRandomUpgrades(3);
        gameUI.UpdateUpgradeChoices(selectedUpgrades, this);
    }

    private List<UpgradeData> PickRandomUpgrades(int count)
    {
        HashSet<UpgradeData> chosen = new HashSet<UpgradeData>();

        int attempts = 0;
        while (chosen.Count < count && attempts < 100) // prevent infinite loop
        {
            UpgradeData candidate = upgrades[Random.Range(0, upgrades.Count)];

            if (!chosen.Contains(candidate))
            {
                chosen.Add(candidate);
            }

            attempts++;
        }

        return new List<UpgradeData>(chosen);
    }


    public void UpgradeChosen(UpgradeData upgrade)
    {
        switch (upgrade.upgradeType)
        {
            case UpgradeData.UpgradeType.PlayerStats:
                ApplyPlayerStatUpgrade(upgrade);
                break;

            case UpgradeData.UpgradeType.AttackStats:
                ApplyAttackStatUpgrade(upgrade);
                break;

            case UpgradeData.UpgradeType.NewMove:
                ApplyNewMove(upgrade);
                upgrades.Remove(upgrade);
                break;
        }

        gameUI.ToggleUpgradeMenu();
    }

    private void ApplyPlayerStatUpgrade(UpgradeData upgrade)
    {
        switch (upgrade.playerStatType)
        {
            case UpgradeData.PlayerStatType.Speed:
                player.movement.moveSpeed += upgrade.playerStatValue;
                break;
            case UpgradeData.PlayerStatType.Jump:
                player.movement.jumpForce += upgrade.playerStatValue;
                break;
            case UpgradeData.PlayerStatType.Weight:
                player.movement.weight *= upgrade.playerStatValue;
                break;
        }
    }

    private void ApplyAttackStatUpgrade(UpgradeData upgrade)
    {
        // Apply globally or to all attacks if you track them in player
        foreach (var attack in player.attacks)
        {
            switch (upgrade.attackStatType)
            {
                case UpgradeData.AttackStatType.Damage:
                    attack.damage += upgrade.attackStatValue;
                    break;
                case UpgradeData.AttackStatType.Knockback:
                    attack.knockbackStrength *= upgrade.attackStatValue;
                    break;
                case UpgradeData.AttackStatType.HitboxSize:
                    attack.hitboxSize *= upgrade.attackStatValue;
                    break;
            }
        }
    }

    private void ApplyNewMove(UpgradeData upgrade)
    {
        var newMove = Resources.Load<AttackData>(upgrade.newMoveLocation);
        if (newMove != null)
        {
            player.AddAttack(newMove);
        }
    }

    private void Update()
    {
        if (enemy == null)
        {
            enemy = FindFirstObjectByType<Enemy>();
            if (enemy != null)
            {
                enemy.isDefeated.ChangeEvent += OnDefeatedEnemy;
            }
        }
    }
}
