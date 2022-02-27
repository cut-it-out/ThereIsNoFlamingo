using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class TutorialScreenManager : MonoBehaviour
{
    [SerializeField] private float animTime = 2f;

    [SerializeField] private GameObject movementInfo;
    [SerializeField] private GameObject fuelInfo;
    [SerializeField] private GameObject realityInfo;
    [SerializeField] private GameObject pompomInfo;
    [SerializeField] private GameObject scoreInfo;
    [SerializeField] private GameObject pressAnyKey;

    private Coroutine displayTutorialCR;
    Game game;

    // Start is called before the first frame update
    private void Awake()
    {
        game = Game.GetInstance();

        movementInfo.GetComponent<RectTransform>().localScale = new Vector3(0.01f, 0.01f, 0.01f);
        fuelInfo.GetComponent<RectTransform>().localScale = new Vector3(0.01f, 0.01f, 0.01f);
        realityInfo.GetComponent<RectTransform>().localScale = new Vector3(0.01f, 0.01f, 0.01f);
        pompomInfo.GetComponent<RectTransform>().localScale = new Vector3(0.01f, 0.01f, 0.01f);
        scoreInfo.GetComponent<RectTransform>().localScale = new Vector3(0.01f, 0.01f, 0.01f);
        pressAnyKey.GetComponent<RectTransform>().localScale = new Vector3(0.01f, 0.01f, 0.01f);
    }

    private void TutorialScreenManager_OnAnyKeyPressed()
    {
        StopTutorialAndMoveToGame();
    }

    private void StopTutorialAndMoveToGame()
    {
        if (displayTutorialCR != null)
        {
            StopCoroutine(displayTutorialCR);
            displayTutorialCR = null;            
        }

        InputManager.GetInstance().OnAnyKeyPressed -= TutorialScreenManager_OnAnyKeyPressed;

        game.StartGame();
        CanvasManager.GetInstance().SwitchCanvas(CanvasType.GameUI);
    }

    public void TriggerIntroStart()
    {
        InputManager.GetInstance().OnAnyKeyPressed += TutorialScreenManager_OnAnyKeyPressed;
        displayTutorialCR = StartCoroutine(DisplayTutorial());
    }

    IEnumerator DisplayTutorial()
    {
        movementInfo.GetComponent<RectTransform>().DOScale(1f, animTime);
        yield return new WaitForSeconds(animTime);

        fuelInfo.GetComponent<RectTransform>().DOScale(1f, animTime);
        yield return new WaitForSeconds(animTime);

        realityInfo.GetComponent<RectTransform>().DOScale(1f, animTime);
        yield return new WaitForSeconds(animTime);

        pompomInfo.GetComponent<RectTransform>().DOScale(1f, animTime);
        yield return new WaitForSeconds(animTime);

        scoreInfo.GetComponent<RectTransform>().DOScale(1f, animTime);
        yield return new WaitForSeconds(animTime);

        pressAnyKey.GetComponent<RectTransform>().DOScale(1f, animTime);
        yield return new WaitForSeconds(animTime);

    }

}
