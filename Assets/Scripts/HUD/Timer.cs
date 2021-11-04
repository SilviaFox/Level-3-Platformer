using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public static string time;
    public static bool timeCounting;

    Text text;
    int minutes, seconds;
    
    void Start()
    {
        timeCounting = true;
        text = GetComponent<Text>();
        StartCoroutine(TimerLogic());
    }

    IEnumerator TimerLogic()
    {
        while (timeCounting)
        {
            yield return new WaitForSeconds(1);

            seconds ++;
            if (seconds == 60)
            {
                seconds = 0;
                minutes ++;
            }

            time = minutes + ":" + (seconds < 10? ("0" + seconds.ToString()) : seconds.ToString());
            text.text = time;
        }

    }
}
