using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScore : MonoBehaviour
{
    public static PlayerScore instance;
    public static int score;
    public static Text scoreText;

    private void Start()
    {
        instance = this;
        scoreText = GetComponent<Text>();
    }

    public void AddScoreInit(int value)
    {
        StopAllCoroutines();
        scoreText.text = score.ToString();
        StartCoroutine(AddScore(value));
    }

    IEnumerator AddScore(int value)
    {
        score += value;

        for (int i = 0; i < value + 1; i+=10)
        {
            scoreText.text = (score - value + i).ToString();
            yield return new WaitForFixedUpdate();
        }

        scoreText.text = score.ToString();
    }
    
}
