using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : RealViewAffectedObject
{

    private InputManager inputManager;

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

    protected override void OnDisable()
    {
        base.OnDisable();
        inputManager.OnMovement -= InputManager_OnMovement;
    }

    private void InputManager_OnMovement(Vector2 direction)
    {
        throw new NotImplementedException();
    }


}
