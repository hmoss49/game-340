using System;
using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    public Enemy enemy;
    public FighterController player;
    public GameUI gameUI;
    
    private void Start()
    {
        enemy = FindFirstObjectByType<Enemy>();
        player = FindFirstObjectByType<FighterController>();
    }

    public void UpgradeChosen(string path)
    {
        player.AddAttack(Resources.Load<AttackData>(path));
    }
    private void OnEnable()
    {
        if (enemy != null && enemy.currentHealth != null)
            enemy.isDefeated.ChangeEvent += OnDefeatedEnemy;
    }

    private void OnDisable()
    {
        if (enemy != null && enemy.currentHealth != null)
            enemy.isDefeated.ChangeEvent -= OnDefeatedEnemy;
    }

    private void OnDefeatedEnemy(bool defeated)
    {
        if (defeated)
        {
            gameUI.ToggleUpgradeMenu();
        }
    }
    
}
