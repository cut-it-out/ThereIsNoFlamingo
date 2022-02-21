using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    #region Events

    public delegate void Movement(Vector2 direction);
    public event Movement OnMovement;
    public delegate void RealViewActivated();
    public event RealViewActivated OnRealViewKeyPressed;
    public delegate void PauseMenuToggle();
    public event PauseMenuToggle OnPauseMenuToggle;

    #endregion

    // swipe related variables    
    [SerializeField] private float minimumDistance = 20f; // TODO maybe update minimum Distance based current device resolution
    [SerializeField] private float maximumTime = 1f;
    [SerializeField, Range(0f, 1f)] private float directionThreshold = .9f;

    private Vector2 startPosition;
    private float startTime;
    private Vector2 endPosition;
    private float endTime;

    // cached variables
    private PlayerControls playerControls;

    protected override void Awake()
    {
        base.Awake();
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls?.Disable();
    }

    void Start()
    {
        //Swipe control
        playerControls.Touch.PrimaryContact.started += ctx => StartTouchPrimary(ctx);
        playerControls.Touch.PrimaryContact.canceled += ctx => EndTouchPrimary(ctx);

        // Keyboard control
        playerControls.Player.Movement.started += ctx => OnWASDPressed(ctx);
        playerControls.Player.RealViewActivation.started += ctx => RealViewKeyPressed(ctx);
        playerControls.Player.PauseMenuActivation.started += ctx => PauseMenuKeyPressed(ctx);
    }

    #region Keyboard input

    private void RealViewKeyPressed(InputAction.CallbackContext ctx)
    {
        OnRealViewKeyPressed?.Invoke();
    }

    private void PauseMenuKeyPressed(InputAction.CallbackContext ctx)
    {
        OnPauseMenuToggle?.Invoke();
    }

    private void OnWASDPressed(InputAction.CallbackContext context)
    {
        OnMovement?.Invoke(context.ReadValue<Vector2>().normalized);
    }
    #endregion

    #region Swipe Detection
    private void StartTouchPrimary(InputAction.CallbackContext context)
    {
        startPosition = playerControls.Touch.PrimaryPosition.ReadValue<Vector2>();
        startTime = (float)context.startTime;
    }

    private void EndTouchPrimary(InputAction.CallbackContext context)
    {
        endPosition = playerControls.Touch.PrimaryPosition.ReadValue<Vector2>();
        endTime = (float)context.time;

        DetectSwipe();
    }

    private void DetectSwipe()
    {
        if (Game.GetInstance().IsPaused) return;

        if (Vector2.Distance(startPosition, endPosition) >= minimumDistance
            && (endTime - startTime) <= maximumTime)
        {
            Vector2 direction = (endPosition - startPosition).normalized;

            OnMovement?.Invoke(direction);

        }
    }
    #endregion

}
