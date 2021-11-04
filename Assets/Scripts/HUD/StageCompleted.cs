using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageCompleted : MonoBehaviour
{
    [SerializeField] Text time, score, stageCompleted;
    Vector3 timePos, scorePos, stageCompPos;
    [SerializeField] Vector3 timePosOffset, scorePosOffset, stageCompOffset;
    [SerializeField] float tweenTime = 0.5f;

    [SerializeField] Image topBar, bottomBar, background;
    float backgroundAlpha;
    [SerializeField] float barOffset = 100;


    [SerializeField] Image fadeToBlack;
    [SerializeField] float fadeToBlackTime;

    // Start is called before the first frame update
    void Start()
    {
        
        InputManager.inputActions.Gameplay.Disable();

        // Get text and stop counting time
        Timer.timeCounting = false;
        time.text = "Time: " + Timer.time;
        score.text = "Score: " + PlayerScore.score.ToString();

        // Get current positions of all UI Elements
        timePos = time.rectTransform.localPosition;
        scorePos = score.rectTransform.localPosition;
        stageCompPos = stageCompleted.rectTransform.localPosition;

        // Move UI Elements to offset positions
        time.rectTransform.localPosition = timePos + timePosOffset;
        score.rectTransform.localPosition = scorePos + scorePosOffset;
        stageCompleted.rectTransform.localPosition = stageCompPos + stageCompOffset;

        // Set background alpha to 0 for later tween
        backgroundAlpha = background.color.a;
        background.color = new Color(0,0,0,0);

        // Move top and bottom bars out of the way for now
        topBar.rectTransform.localPosition = topBar.rectTransform.localPosition + (Vector3.up * barOffset);
        bottomBar.rectTransform.localPosition = bottomBar.rectTransform.localPosition + (Vector3.down * barOffset);

        // Tween that thing
        ShowStageCompleted();
    }

    void ShowStageCompleted()
    {
        StartCoroutine(GlobalMusicManager.instance.FadeOut(1));
        FindObjectOfType<HUDLogic>().HideHUD();

        LeanTween.alpha(background.rectTransform, backgroundAlpha, 1);
        LeanTween.moveLocal(topBar.gameObject, topBar.rectTransform.localPosition - (Vector3.up * barOffset), tweenTime / 2);
        LeanTween.moveLocal(bottomBar.gameObject, bottomBar.rectTransform.localPosition - (Vector3.down * barOffset), tweenTime / 2);

        LeanTween.moveLocal(stageCompleted.gameObject, stageCompPos, tweenTime).setEaseInOutSine().setOnComplete(ShowTime);
    }

    void ShowTime()
    {
        LeanTween.moveLocal(time.gameObject, timePos, tweenTime).setEaseInOutSine().setOnComplete(ShowScore);
    }

    void ShowScore()
    {
        LeanTween.moveLocal(score.gameObject, scorePos, tweenTime).setEaseInOutSine().setOnComplete(FadeToBlack);
    }

    void FadeToBlack()
    {
        LeanTween.alpha(fadeToBlack.rectTransform, 1, fadeToBlackTime).setEaseInCirc().setOnComplete(ChangeLevel);
    }

    void ChangeLevel()
    {
        GameManager.LoadMenu();
    }



}
