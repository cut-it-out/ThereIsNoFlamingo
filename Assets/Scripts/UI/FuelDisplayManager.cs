using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelDisplayManager : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    private Game game;

    private void Awake()
    {
        game = Game.GetInstance();
        HandleFuelContainerFill();

    }

    private void OnEnable()
    {
        game.OnFuelChanged += HandleFuelContainerFill;
        HandleFuelContainerFill();
    }


    private void OnDisable()
    {
        game.OnFuelChanged -= HandleFuelContainerFill;
    }

    private void HandleFuelContainerFill()
    {
        float fillAmount;

        fillAmount = game.CurrentFuel / game.MaxFuel;
        
        fillImage.fillAmount = Mathf.Clamp(fillAmount, 0f, 1f);
    }

}
