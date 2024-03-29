using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : RealViewAffectedObject
{
    [Header("Player Movement")]
    [SerializeField] float playerXPosition = 0f;
    [SerializeField] float playerYPosition = -12f;

    [Header("Player Movement padding")]
    [SerializeField] float paddingLeft;
    [SerializeField] float paddingRight;

    [Header("Tweening settings")]
    [SerializeField] private float timeToTween = 0.5f;

    float playerSpeed;
    private const float PLAYER_SPEED_MULTIPLIER = 3.5f; // times the falling object current speed

    private Vector2 minBounds;
    private Vector2 maxBounds;
    private Vector2 movingDirection;

    // cached variables
    private Game game;
    private InputManager inputManager;
    private Camera mainCamera;

    private void Awake()
    {
        game = Game.GetInstance();
        inputManager = InputManager.GetInstance();
        mainCamera = Camera.main;
        InitBounds();
        SetIsReal(true); // to make sure player obj is visible in REAL view
        ResetPlayerPositionToStart();
        //Game.GetInstance().AdjustNotCatchedCheckZone(playerYPosition);
    }

    public void ResetPlayerPositionToStart()
    {
        transform.position = new Vector2(playerXPosition, playerYPosition); // set player to start
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        // subscribe for movement
        inputManager.OnMovement += InputManager_OnMovement;
        Spawner.GetInstance().OnProgressMultiplierChange += Player_OnProgressMultiplierChange;
    }

    private void Player_OnProgressMultiplierChange(float newSpeed)
    {
        playerSpeed = newSpeed * PLAYER_SPEED_MULTIPLIER;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        FallingObject fallingObject = collision.gameObject.GetComponent<FallingObject>();
        if (fallingObject != null && fallingObject.IsRealObject)
        {
            //catched the REAL one
            game.IncreaseCurrentScore();
            game.UpdateFuelContainer(fallingObject.GetFuelGain());
            //TODO: add some nice effect to visualize correct catch :)
            DoAnim(fallingObject.gameObject.transform);
            fallingObject.DestroySelf(timeToTween);
        }
        else
        {
            // catched the WRONG one
            if (!game.IsRealViewEnabled)
            {
                game.UpdateFuelContainer(-fallingObject.GetFuelLost());
                DoAnim(fallingObject.gameObject.transform);
                fallingObject.DestroySelf(timeToTween);
            }
        }
    }

    private void DoAnim(Transform t)
    {
        t.DOScale(0.01f, timeToTween).SetEase(Ease.InOutBounce);
    }

    private void InitBounds()
    {        
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
