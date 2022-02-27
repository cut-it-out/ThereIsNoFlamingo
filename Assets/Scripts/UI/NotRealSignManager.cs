using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotRealSignManager : MonoBehaviour
{
    [SerializeField] GameObject notRealText;
    [SerializeField] GameObject realText;

    Game game;

    private void Awake()
    {
        game = Game.GetInstance();
        SetRealViewText(game.IsRealViewEnabled);
    }

    private void OnEnable()
    {
        game.OnRealViewToggle += Game_OnRealViewToggle;
        SetRealViewText(game.IsRealViewEnabled);
    }

    private void SetRealViewText(bool value)
    {
        notRealText.SetActive(!value);
        //realText.SetActive(value);
    }

    private void Game_OnRealViewToggle(bool value)
    {
        SetRealViewText(value);
    }

    private void OnDisable()
    {
        game.OnRealViewToggle -= Game_OnRealViewToggle;
    }
}
