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
    public void StartButtonSceneTransition()
    {
        SceneManager.LoadScene("SelectScreen");
    }

    public void SelectCharacter(int characterIndex)
    {
        PlayerPrefs.SetInt("Character", characterIndex);
        characterChosen = true;
    }

    public void SelectButtonSceneTransition()
    {
        if (characterChosen)
        {
            SceneManager.LoadScene("GameScreen");
        }
    }
    
}
