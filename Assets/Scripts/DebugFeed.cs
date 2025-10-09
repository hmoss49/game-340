using TMPro;
using UnityEngine;

public class DebugFeed : MonoBehaviour
{
    [Tooltip("Assign a TextMeshProUGUI element in your Canvas to show the compact summary (e.g., 236LP).")]
    public TextMeshProUGUI summaryText;   // ✅ Correct TMP type for UI

    [Tooltip("Find the FighterController to read from (auto-find if not set).")]
    public FighterController controller;

    private bool overlayVisible = true;

    void Start()
    {
#if UNITY_2023_1_OR_NEWER
        if (!controller) controller = Object.FindFirstObjectByType<FighterController>();
#else
        if (!controller) controller = Object.FindObjectOfType<FighterController>();
#endif
    }

    void Update()
    {
        overlayVisible = gameObject.activeInHierarchy;
        if (overlayVisible && summaryText && controller)
            summaryText.text = controller.GetCompactSummary();
    }
}