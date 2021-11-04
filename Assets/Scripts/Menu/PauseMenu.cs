using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public void Resume()
    {
        GameManager.Unpause();
    }

    public void Restart()
    {
        GameManager.RestartStage();
    }

    public void Exit()
    {
        GameManager.LoadMenu();
    }
}
