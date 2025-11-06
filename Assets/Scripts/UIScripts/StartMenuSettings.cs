using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartMenuUIManager : MonoBehaviour
{
    [Header("Menu References")]
    public GameObject settingsMenu;
    public GameObject controlsMenu;

    [Header("Default Buttons")]
    public Selectable defaultStartButton;
    public Selectable defaultSettingsButton;
    public Selectable defaultControlsButton;

    private bool isSettings = false;
    private bool isControls = false;

    // === Menu methods ===
    public void ToggleSettingsMenu()
    {
        isSettings = !isSettings;
        settingsMenu.SetActive(isSettings);

        // If opening, highlight first settings button
        // If closing, highlight main Start button again
        SetSelected(isSettings ? defaultSettingsButton : defaultStartButton);
    }

    public void ToggleControls()
    {
        isControls = !isControls;
        controlsMenu.SetActive(isControls);
        settingsMenu.SetActive(!isControls);

        SetSelected(isControls ? defaultControlsButton : defaultSettingsButton);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        Debug.Log("Quit requested from start menu.");
    }

    // === Helper ===
    private void SetSelected(Selectable target)
    {
        if (target != null && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(target.gameObject);
        else if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    // Optional: set default selection when scene loads
    void Start()
    {
        if (defaultStartButton != null)
            SetSelected(defaultStartButton);
    }
}