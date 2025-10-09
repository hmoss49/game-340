using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;  // for Selectable

public class GameUIManager : MonoBehaviour
{
    [Header("Menu References")]
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public GameObject inputLog;
    public GameObject controlsMenu;

    [Header("Default Selectable Buttons")]
    public Selectable defaultPauseButton;
    public Selectable defaultSettingsButton;
    public Selectable defaultControlsButton;

    private bool isPaused = false;
    private bool isSettings = false;
    private bool isLog = false;
    private bool isControls = false;

    // === Input callbacks ===
    void OnPause(InputValue value)
    {
        if (!value.isPressed) return;

        if (isPaused && !isSettings)
            ResumeGame();
        else
            PauseGame();
    }

    void OnLog(InputValue value)
    {
        if (!value.isPressed) return;

        isLog = !isLog;
        if (inputLog)
            inputLog.SetActive(isLog);
    }

    // === Menu methods ===
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        Debug.Log("Quit requested.");
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0;

        //SetSelected(defaultPauseButton);
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;

        //EventSystem.current.SetSelectedGameObject(null);
    }

    public void ToggleControls()
    {
        isSettings = !isSettings;
        isControls = !isControls;

        controlsMenu.SetActive(isControls);
        settingsMenu.SetActive(!isControls);

        //SetSelected(isControls ? defaultControlsButton : defaultSettingsButton);
    }

    public void ToggleSettingsMenu()
    {
        isSettings = !isSettings;
        settingsMenu.SetActive(isSettings);
        pauseMenu.SetActive(!isSettings);

        //SetSelected(isSettings ? defaultSettingsButton : defaultPauseButton);
    }

    // === Helper ===
    private void SetSelected(Selectable target)
    {
        if (target != null)
            EventSystem.current.SetSelectedGameObject(target.gameObject);
        else
            EventSystem.current.SetSelectedGameObject(null);
    }
}
