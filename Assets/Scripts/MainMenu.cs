using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public string levelToLoad;
    private bool isMuted = false;

    public void StartGame()
    {
        SceneManager.LoadScene(levelToLoad);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void ToggleSound()
    {
        isMuted = !isMuted;
        AudioListener.pause = isMuted;
    }

}
