using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    void Start()
    {
        LeanTween.init(2000);
    }
    
    public static void RestartStage()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public static void LoadLevel(string level)
    {
        SceneManager.LoadScene(level);
    }
}
