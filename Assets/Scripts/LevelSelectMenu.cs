using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectMenu : MonoBehaviour
{
    public string mainMenu = "MainMenu";
    
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenu);
    }
}
