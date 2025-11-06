using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [FormerlySerializedAs("dummy")] [Header("References")]
    public Enemy enemy; // Reference to your TrainingDummy
    public Image greenBar;      // Child green bar image

    [Header("Settings")]
    public float lerpSpeed = 5f;    // How smoothly the fill adjusts

    private float targetPercent = 1f;

    private void Awake()
    {
        if (enemy == null)
            enemy = GetComponentInParent<TrainingDummy>();

        if (greenBar == null)
            greenBar = GetComponentInChildren<Image>();
    }

    private void OnEnable()
    {
        if (enemy != null && enemy.currentHealth != null)
            enemy.currentHealth.ChangeEvent += OnHealthChanged;
    }

    private void OnDisable()
    {
        if (enemy != null && enemy.currentHealth != null)
            enemy.currentHealth.ChangeEvent -= OnHealthChanged;
    }

    private void OnHealthChanged(float current)
    {
        targetPercent = Mathf.Clamp01(current / enemy.maxHealth);
    }

    private void Update()
    {
        // Smoothly interpolate toward new fill
        greenBar.fillAmount = Mathf.Lerp(greenBar.fillAmount, targetPercent, Time.deltaTime * lerpSpeed);
    }
}