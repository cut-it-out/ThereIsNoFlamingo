using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : RealViewAffectedObject
{
    [Header("Player Movement")]
    [SerializeField] float playerSpeed = 3f;
    [SerializeField] float playerYPosition = -12f;

    [Header("Player Movement padding")]
    [SerializeField] float paddingLeft;
    [SerializeField] float paddingRight;

    private Vector2 minBounds;
    private Vector2 maxBounds;

    private InputManager inputManager;
    private Vector2 movingDirection;

    private void Awake()
    {
        inputManager = InputManager.GetInstance();        
    }

    private void Start()
    {        
        InitBounds();
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

    private void InitBounds()
    {
        Camera mainCamera = Camera.main;
        minBounds = mainCamera.ViewportToWorldPoint(new Vector2(0, 0));
        maxBounds = mainCamera.ViewportToWorldPoint(new Vector2(1, 1));
    }

    private void InputManager_OnMovement(Vector2 direction)
    {
        movingDirection = direction;
    }


    private void Update()
    {
        MovePlayer();
    }


    private void MovePlayer()
    {
        Vector2 delta = movingDirection * playerSpeed * Time.deltaTime;
        Vector2 newPos = new Vector2();

        newPos.x = Mathf.Clamp(transform.position.x + delta.x, minBounds.x + paddingLeft, maxBounds.x - paddingRight);
        newPos.y = playerYPosition;

        transform.position = newPos;
    }

}
