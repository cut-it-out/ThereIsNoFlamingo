using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : RealViewAffectedObject
{
    [SerializeField] float playerSpeed = 1f;
    private InputManager inputManager;
    private Vector2 movingDirection;

    private void Awake()
    {
        inputManager = InputManager.GetInstance();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        // subscribe for movement
        inputManager.OnMovement += InputManager_OnMovement;
    }

    //protected override void OnDisable()
    //{
    //    base.OnDisable();
    //    inputManager.OnMovement -= InputManager_OnMovement;
    //}

    private void InputManager_OnMovement(Vector2 direction)
    {
        movingDirection = direction;
    }


    private void FixedUpdate()
    {
        transform.Translate(movingDirection * playerSpeed * Time.deltaTime);
    }


}
