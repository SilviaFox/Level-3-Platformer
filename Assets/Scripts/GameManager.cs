using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static string currentLevel;

    void Start()
    {
        LeanTween.init(2000);
    }
    
    public static void RestartStage()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(currentLevel);
    }

    public static void EndStage()
    {
        InputManager.inputActions.Gameplay.Disable();
        SceneManager.LoadScene("Complete", LoadSceneMode.Additive);
    }

    public static void LoadLevel(string level)
    {
        currentLevel = level;
        SceneManager.LoadScene(level);
    }

    public static void LoadMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public static void Pause()
    {
        Time.timeScale = 0;
        SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
        InputManager.inputActions.Gameplay.Disable();
    }

    public static void Unpause()
    {
        Time.timeScale = 1;
        SceneManager.UnloadSceneAsync("PauseMenu");
        InputManager.inputActions.Gameplay.Enable();
    }
}
