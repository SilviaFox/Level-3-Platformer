using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBackgroundSwapper : MonoBehaviour
{
    [SerializeField] float fadeSpeed = 0.5f;
    [SerializeField] float waitTime = 5;

    void Start()
    {
        foreach (Transform sprite in transform)
        {
            LeanTween.alpha(sprite.gameObject, 0, 0);
        }
        StartSwap();
    }

    void StartSwap()
    {
        StartCoroutine(MenuSwap());
    }

    IEnumerator MenuSwap() 
    {
        foreach (Transform sprite in transform)
        {
            LeanTween.alpha(sprite.gameObject, 1, fadeSpeed);
            yield return new WaitForSeconds(waitTime);
            LeanTween.alpha(sprite.gameObject, 0, fadeSpeed);   
        }

        StartSwap();
    }
}
