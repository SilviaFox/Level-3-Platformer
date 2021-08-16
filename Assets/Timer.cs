using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    Text text;
    int minutes, seconds;
    
    void Start()
    {
        text = GetComponent<Text>();
        StartCoroutine(TimerLogic());
    }

    IEnumerator TimerLogic()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            seconds ++;
            if (seconds == 60)
            {
                seconds = 0;
                minutes ++;
            }

            text.text = minutes + ":" + (seconds < 10? ("0" + seconds.ToString()) : seconds.ToString());
        }

    }
}
