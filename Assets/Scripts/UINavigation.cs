using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UINavigation: MonoBehaviour
{
    private bool characterChosen;
    private void Start()
    {
        characterChosen = false;
    }
    public void ToSelectScene()
    {
        SceneManager.LoadScene("SelectScreen");
    }
    
    public void ToMainMenuScene()
    {
        SceneManager.LoadScene("StartScreen");
        Time.timeScale = 1;
    }

    public void SelectCharacter(int characterIndex)
    {
        PlayerPrefs.SetInt("Character", characterIndex);
        characterChosen = true;
    }

    public void ToGameScene()
    {
        if (characterChosen)
        {
            SceneManager.LoadScene("GameScreen");
        }
    }

    public void ToTutorialScene()
    {
        if (characterChosen)
        {
            SceneManager.LoadScene("TutorialScreen");
        }
        {
            
        }
    }
    
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        Debug.Log("Quit requested.");
    }
    
}
