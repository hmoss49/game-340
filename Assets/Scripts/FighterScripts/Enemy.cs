using UnityEngine;

public class Enemy :MonoBehaviour
{
    public float maxHealth;
    public ObservableValue<float> currentHealth = new ObservableValue<float>();
    public ObservableValue<bool> isDefeated = new ObservableValue<bool>();

    protected virtual void Awake()
    {
        currentHealth.Value = maxHealth;
        isDefeated.Value = false;
    }
}
