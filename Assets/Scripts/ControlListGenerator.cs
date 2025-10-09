using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ControlListGenerator : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Your Input Actions asset (e.g., PlayerControls).")]
    public InputActionAsset inputAsset;

    [Tooltip("Parent transform where control labels will be placed (e.g., a vertical layout group).")]
    public Transform listParent;

    [Tooltip("TextMeshProUGUI prefab for each line of text.")]
    public TextMeshProUGUI linePrefab;

    [Header("Options")]
    [Tooltip("If true, regenerate on Awake instead of Start (useful when menus are enabled late).")]
    public bool generateOnAwake = false;

    [Tooltip("If true, will show which action map each control comes from.")]
    public bool showActionMapNames = false;

    // Define which action maps to include
    string[] allowedMaps = { "Gameplay", "System" };
    
    void Awake()
    {
        if (generateOnAwake)
            GenerateList();
    }

    void Start()
    {
        if (!generateOnAwake)
            GenerateList();
    }

    public void GenerateList()
    {
        if (!inputAsset || !listParent || !linePrefab)
        {
            Debug.LogWarning("ControlListGenerator missing references!");
            return;
        }

        // Clean existing children
        foreach (Transform child in listParent)
            Destroy(child.gameObject);

        foreach (var map in inputAsset.actionMaps)
        {
            if (System.Array.IndexOf(allowedMaps, map.name) == -1)
                continue; // skip everything else


            foreach (var action in map.actions)
            {
                string bindings = "";

                foreach (var b in action.bindings)
                {
                    if (b.isComposite)
                    {
                        bindings += "[";
                        int index = System.Array.IndexOf(action.bindings.ToArray(), b);
                        for (int i = index + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; i++)
                        {
                            var part = action.bindings[i];
                            bindings += $"{part.name}: {part.ToDisplayString()}  ";
                        }
                        bindings += "]";
                    }
                    else if (!b.isPartOfComposite)
                    {
                        bindings += $"{b.ToDisplayString()}  ";
                    }
                }

                var line = Instantiate(linePrefab, listParent);
                line.text = $"{action.name}: {bindings}";
            }
        }
    }

}
