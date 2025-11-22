using System.Collections.Generic;
using TMPro;
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
    public GameObject resultsMenu;
    public GameObject upgradeMenu;
    public GameObject upgradeChoiceOne;
    public GameObject upgradeChoiceTwo;
    public GameObject upgradeChoiceThree;
    public TextMeshProUGUI resultsText;
    public TextMeshProUGUI timerText;

    [Header("Default Selectable Buttons")]
    public Selectable defaultPauseButton;
    public Selectable defaultSettingsButton;
    public Selectable defaultControlsButton;
    public Selectable defaultAboutButton;
    public Selectable defaultUpgradeButton;
    public Selectable defaultResultsButton;
    

    private bool isPaused = false;
    private bool isSettings = false;
    private bool isLog = false;
    private bool isControls = false;
    private bool isAbout = false;
    private bool isUpgrade = false;
    private bool isResults = false;

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
        pauseMenu.SetActive(!isAbout);

        SetSelected(isAbout ? defaultAboutButton : defaultPauseButton);
    }

    public void ToggleLogMenu()
    {
        isLog = !isLog;
        inputLog.SetActive(isLog);
    }

    public void ToggleResultsMenu()
    {
        isResults = !isResults;
        resultsMenu.SetActive(isResults);
        if (isResults)
        {
            Time.timeScale = 0;
            SetSelected(isResults ? defaultResultsButton : defaultPauseButton);
        }
        else
        {
            Time.timeScale = 1;
            SetSelected(null);
        }
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

    public void UpdateUpgradeChoices(List<UpgradeData> choices, UpgradeSystem system)
    {
        ToggleUpgradeMenu();

        // Grab buttons from the three GameObjects
        Button[] buttons = new Button[3];
        buttons[0] = upgradeChoiceOne.GetComponent<Button>();
        buttons[1] = upgradeChoiceTwo.GetComponent<Button>();
        buttons[2] = upgradeChoiceThree.GetComponent<Button>();

        // Assign names + listeners
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i < choices.Count)
            {
                var upgrade = choices[i];

                // Set visible text
                var text = buttons[i].GetComponentInChildren<TMP_Text>();
                if (text != null)
                    text.text = upgrade.upgradeName;

                buttons[i].gameObject.SetActive(true);

                // Clear old listeners first
                buttons[i].onClick.RemoveAllListeners();

                // Capture upgrade variable
                UpgradeData capturedUpgrade = upgrade;
                buttons[i].onClick.AddListener(() =>
                {
                    system.UpgradeChosen(capturedUpgrade);
                });
            }
            else
            {
                buttons[i].gameObject.SetActive(false);
            }
        }
    }

    public void SetResultsText(string text)
    {
        resultsText.text = text;
    }

    public void SetTimerText(float time)
    {
        timerText.text = $"Defeat the enemies in:\n{Mathf.CeilToInt(time)}";
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
