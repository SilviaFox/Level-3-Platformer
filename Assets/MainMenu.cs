using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] string level;
    [SerializeField] RectTransform fade;

    public void StartGame()
    {
        LeanTween.alpha(fade,1,1).setOnComplete(LoadGame);
    }

    void LoadGame()
    {
        GameManager.LoadLevel(level);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
