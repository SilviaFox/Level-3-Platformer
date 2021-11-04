using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDLogic : MonoBehaviour
{
    [SerializeField] RectTransform[] UIElements;
    [SerializeField] float tweenTime;

    public void HideHUD()
    {
        foreach (RectTransform element in UIElements)
        {
            LeanTween.moveLocal(element.gameObject, element.localPosition + (Vector3.left * 400), tweenTime).setEaseInOutSine();
        }
    }

}
