using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;  // for Selectable

public class GameUI : MonoBehaviour
{
    [Header("Menu References")]
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public GameObject inputLog;
    public GameObject controlsMenu;
    public GameObject aboutMenu;
    public GameObject upgradeMenu;

    [Header("Default Selectable Buttons")]
    public Selectable defaultPauseButton;
    public Selectable defaultSettingsButton;
    public Selectable defaultControlsButton;
    public Selectable defaultAboutButton;
    public Selectable defaultUpgradeButton;

    private bool isPaused = false;
    private bool isSettings = false;
    private bool isLog = false;
    private bool isControls = false;
    private bool isAbout = false;
    private bool isUpgrade = false;

    // === Input callbacks ===
    void OnPause(InputValue value)
    {
        if (value.isPressed)
        {
            if (isPaused && !isSettings)
                ResumeGame();
            else
                PauseGame();
        }
    }

    void OnLog(InputValue value)
    {
        ToggleLogMenu();
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

        SetSelected(defaultPauseButton);
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;

        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ToggleControls()
    {
        isSettings = !isSettings;
        isControls = !isControls;

        controlsMenu.SetActive(isControls);
        settingsMenu.SetActive(!isControls);

        SetSelected(isControls ? defaultControlsButton : defaultSettingsButton);
    }

    public void ToggleSettingsMenu()
    {
        isSettings = !isSettings;
        settingsMenu.SetActive(isSettings);
        pauseMenu.SetActive(!isSettings);

        SetSelected(isSettings ? defaultSettingsButton : defaultPauseButton);
    }
    
    public void ToggleAboutMenu()
    {
        isAbout = !isAbout;
        aboutMenu.SetActive(isAbout);
        pauseMenu.SetActive(!isSettings);

        SetSelected(isSettings ? defaultAboutButton : defaultPauseButton);
    }

    public void ToggleLogMenu()
    {
        isLog = !isLog;
        inputLog.SetActive(isLog);
    }
    
    public void ToggleUpgradeMenu()
    {
        isUpgrade = !isUpgrade;
        upgradeMenu.SetActive(isUpgrade);

        if (isUpgrade)
        {
            Time.timeScale = 0;
            SetSelected(isUpgrade ? defaultUpgradeButton : defaultPauseButton);
        }
        else
        {
            Time.timeScale = 1;
            SetSelected(null);
        }
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
