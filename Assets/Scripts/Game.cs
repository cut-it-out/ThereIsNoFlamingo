using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Game : Singleton<Game>
{

    [Header("RealView Fuel Settings")]
    [SerializeField] private float fuelContainerMax = 60f;
    [SerializeField] private float fuelInitialValue = 30f;
    [SerializeField] private float fuelToTimeExchangeRate = 1f; //in seconds, so 10 fuel = 10 seconds

    [Header("Visual Feedback related")]
    [SerializeField] private float visualFeedbackInterval = 1f;
    [SerializeField] private Volume globalVolume;

    //[Header("Progress related")]
    //[SerializeField] private int progressAfterScore = 5;

    //[Header("NotCatchedZone")]
    //[SerializeField] private GameObject notCatchedZone;

    [Header("Player object")]
    [SerializeField] Player player; //to easily reset player position

    // Game related
    public bool IsPaused { get; private set; }
    //public int PlayerLife { get; private set; }
    public int CurrentScore { get; private set; }
    public float CurrentFuel { get; private set; }
    public float MaxFuel { get; private set; }
    public bool IsGameOver { get; private set; } = true;
    //public bool IsIntroStillRunning { get; private set; } = false;

    #region Events

    public delegate void ScoreChanged();
    public event ScoreChanged OnScoreChanged;

    public delegate void FuelChanged();
    public event FuelChanged OnFuelChanged;

    public delegate void RealViewToggle(bool value);
    public event RealViewToggle OnRealViewToggle;
    
    #endregion


    // RealView event
    //public event EventHandler<RealViewEventArgs> OnRealViewToggle;
    //public class RealViewEventArgs : EventArgs
    //{
    //    public bool isRealViewActive;
    //}

    // RealView related variables
    public bool IsRealViewEnabled { get; private set; }

    // cached vars
    private Bloom bloomComponent;
    private CanvasManager canvasManager;
    private InputManager inputManager;


    protected override void Awake()
    {
        base.Awake();

        // get bloom effect component
        Bloom tempBloom;
        if (globalVolume.profile.TryGet<Bloom>(out tempBloom))
        {
            bloomComponent = tempBloom;
        }

        //IsGameOver = true;
        UpdateScore(true);

        // set fuel container values to initial value
        InitFuelContainer();

    }

    // Start is called before the first frame update
    void Start()
    {
        canvasManager = CanvasManager.GetInstance();
        inputManager = InputManager.GetInstance();

        // Subscribe to events
        inputManager.OnRealViewKeyPressed += InputManager_OnRealViewKeyPressed;
        inputManager.OnPauseMenuToggle += InputManager_OnPauseMenuToggle;

        //StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        // handle RealView timer updates
        HandleFuelUsageInRealView();
    }

    private void InputManager_OnRealViewKeyPressed()
    {
        if (!IsGameOver && !IsPaused)
        {
            ToggleRealView();
        }
    }

    private void InputManager_OnPauseMenuToggle()
    {
        //if (!IsIntroStillRunning && !IsGameOver)
        if (!IsGameOver)
        {
            if (IsPaused)
            {
                ResumeGame();
                canvasManager.SwitchCanvas(CanvasType.GameUI);
            }
            else
            {
                PauseGame();
                canvasManager.SwitchCanvas(CanvasType.MainMenu);
            }
        }
    }

    //public void AdjustNotCatchedCheckZone(float playerY)
    //{
    //    notCatchedZone.GetComponent<NotCatchedZone>().AdjustPositionToPlayerY(playerY);
    //}



    #region Basic Game Stuff (start, reset, pause, etc)

    public void StartGame()
    {        
        IsGameOver = false;
        UpdateScore(true);
        SetRealView(false); // Reset RealView For Start
        InitFuelContainer();
        player.ResetPlayerPositionToStart();
        Spawner.GetInstance().InitSpawner();
        ResumeGame(); // to make sure we don't stuck in pause

        // start music track from beginning
        if (AudioManager.GetInstance().IsMusicEnabled) AudioManager.GetInstance().SetMusic(true);

    }
    public void ResetGame()
    {
        StopGame();
        StartGame();
    }

    public void StopGame()
    {
        IsPaused = true;
        IsGameOver = true;
        Spawner.GetInstance().StopSpawner();
        Spawner.GetInstance().RemoveAllRemainingFallingObjects(); //remove remaining objects

    }

    public void GameOver()
    {
        // TODO: maybe refactor to use events to trigger stuff in other classes (like spawner, audiomanager)

        StopGame();

        //StartCoroutine(DisplayLifeLostFeedback()); // already handled previously
        //canvasManager.SwitchCanvas(CanvasType.YouDiedSplashScreen);
        canvasManager.SwitchCanvas(CanvasType.GameOver);

    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        IsPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        IsPaused = false;
    }


    #endregion

    #region Score related functions

    public void IncreaseCurrentScore()
    {
        UpdateScore();
        //StartCoroutine(DisplayPositiveFeedback());
    }

    private void UpdateScore(bool resetScore = false)
    {
        if (resetScore)
        {
            CurrentScore = 0;
        }
        else
        {
            CurrentScore++;
        }
        OnScoreChanged?.Invoke();
    }

    #endregion

    #region Feedback coroutines

    IEnumerator DisplayLifeLostFeedback()
    {
        AudioManager.GetInstance().PlayDamageSound();
        canvasManager.ActivateCanvas(CanvasType.LifeLostFeedbackScreen, true);
        yield return new WaitForSeconds(visualFeedbackInterval);
        canvasManager.ActivateCanvas(CanvasType.LifeLostFeedbackScreen, false);
    }

    IEnumerator DisplayPositiveFeedback()
    {
        AudioManager.GetInstance().PlayPositiveFeedbackSound();
        canvasManager.ActivateCanvas(CanvasType.PositiveFeedbackScreen, true);
        yield return new WaitForSeconds(visualFeedbackInterval);
        canvasManager.ActivateCanvas(CanvasType.PositiveFeedbackScreen, false);
    }

    #endregion


    #region Fuel and RealView Functions

    public void UpdateFuelContainer(float value = 0f)
    {
        Debug.Log($"UpdateFuelContainer - value: {value}");

        float newFuelValue = CurrentFuel + value;

        if (newFuelValue <= 0f)
        {
            //Game Over
            newFuelValue = 0f;

        }
        else if (newFuelValue > fuelContainerMax)
        {
            //progress to next stage
            // TODO: trigger some nice effect???
            newFuelValue = fuelInitialValue;
            Spawner.GetInstance().IncreaseProgressMultiplier();
        }
        
        CurrentFuel = newFuelValue;

        if (value < -1f)
        {
            StartCoroutine(DisplayLifeLostFeedback());
        }
        else if (value > 0f)
        {
            StartCoroutine(DisplayPositiveFeedback());
        }

        Debug.Log($"UpdateFuelContainer - game.MaxFuel: {MaxFuel}");
        Debug.Log($"UpdateFuelContainer - game.CurrentFuel: {CurrentFuel}");

        // Trigger related event
        OnFuelChanged?.Invoke();

        if (CurrentFuel == 0f) GameOver();

    }

    private void InitFuelContainer()
    {
        CurrentFuel = fuelInitialValue;
        MaxFuel = fuelContainerMax;
        OnFuelChanged?.Invoke();
    }

    private void HandleFuelUsageInRealView()
    {
        if (IsRealViewEnabled)
        {
            UpdateFuelContainer(-(Time.deltaTime * fuelToTimeExchangeRate));
        }
        else
        {
            // if RealView is not active
            // TODO: do I want to slowly decrease the fuel?
        }
    }

    private void ToggleRealView()
    {
        SetRealView(!IsRealViewEnabled);                
    }

    private void SetRealView(bool value)
    {
        IsRealViewEnabled = value; 
        OnRealViewToggle?.Invoke(IsRealViewEnabled);
    }


    #endregion
}
