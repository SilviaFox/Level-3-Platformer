using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static void RestartStage()
    {
        LeanTween.init(2000);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
