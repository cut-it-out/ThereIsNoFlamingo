using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject StartButton;
    [SerializeField] GameObject ContinueButton;
    [SerializeField] GameObject ResetButton;

    private void OnEnable()
    {
        if (Game.GetInstance().IsGameOver)
        {
            StartButton.SetActive(true);
            ContinueButton.SetActive(false);
            ResetButton.SetActive(false);
        }
        else
        {
            StartButton.SetActive(false);
            ContinueButton.SetActive(true);
            ResetButton.SetActive(true);
        }
    }
}
