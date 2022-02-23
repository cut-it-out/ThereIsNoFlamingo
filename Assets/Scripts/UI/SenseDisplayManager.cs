using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SenseDisplayManager : MonoBehaviour
{
    [SerializeField] private Image glowImage;
    [SerializeField] private Image fillImage;
    [SerializeField] private bool isThisTheLeftSenseLight = true;

    private Game game;
    private bool isGlowActive = false;

    private void Start()
    {
        game = Game.GetInstance();

        bool isThisActived = PlayerPrefs.GetInt("isLeftSenseLightDisplayed", 1) == 1 ? true : false;
        gameObject.SetActive(isThisTheLeftSenseLight ? isThisActived : !isThisActived);
    }

    private void Update()
    {
        HandleSenseLightFill();
        HandleSenseLightGlow();
    }

    private void HandleSenseLightFill()
    {
        float fillAmount;

        if (!game.IsRealViewInCooldown)
        {
            if (game.IsRealViewEnabled)
            {
                fillAmount = game.RealViewEnabledFill;
                isGlowActive = false;
            }
            else
            {
                fillAmount = 1f;
                isGlowActive = true;
            }
        }
        else
        {
            fillAmount = game.RealViewCooldownFill;
            isGlowActive = false;
        }

        fillImage.fillAmount = Mathf.Clamp(fillAmount, 0f, 1f);
    }

    private void HandleSenseLightGlow()
    {
        glowImage.gameObject.SetActive(isGlowActive);
    }
}
