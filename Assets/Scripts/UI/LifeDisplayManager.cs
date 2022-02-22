using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using System;

public class LifeDisplayManager : MonoBehaviour
{

    //[SerializeField] private float startPadding = 80f;
    //[SerializeField] private float distanceBetweenImages = 100f;
    [SerializeField] private GameObject lifePrefab;

    private List<LifeDisplayItem> lifeDisplayItems;

    private bool isFirstTime = true;
    private Game game;

    void OnEnable()
    {
        game = Game.GetInstance();

        if (isFirstTime)
        {
            int lifeCount = game.GetInitialLifeCount();
            if (lifeCount > 0)
            {
                Setup(lifeCount);
                isFirstTime = false;
            }
        }
        lifeDisplayItems = GetComponentsInChildren<LifeDisplayItem>().ToList();
    }

    private void Setup(int totalLifeCount)
    {
        for (int currentLife = 0; currentLife < totalLifeCount; currentLife++)
        {
            LifeDisplayItem.Create(
                lifePrefab, 
                transform, 
                new Vector3(0,0,0), 
                currentLife);
        }        
    }

    private void Update()
    {
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        lifeDisplayItems.ForEach(life => {
            if (life.GetLifeNumber() < game.PlayerLife)
            {
                life.SetSprite(true);
            }
            else
            {
                life.SetSprite(false);
            }
        });

    }
}
