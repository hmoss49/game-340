using UnityEngine;
using UnityEngine.SceneManagement;

public class UINavigation: MonoBehaviour
{
    public void StartButtonSceneTransition()
    {
        SceneManager.LoadScene("SelectScreen");
    }
}
