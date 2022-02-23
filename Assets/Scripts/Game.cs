using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Game : Singleton<Game>
{

    [Header("Real View times")]
    [SerializeField] private float realViewEnabledInterval = 3f;
    [SerializeField] private float realViewCooldownInterval = 10f;
    [SerializeField] private Volume globalVolume;

    [Header("Lifes related")]
    [SerializeField] private int initialLifeCount = 3;
    [SerializeField] private float lifeLostFeedbackInterval = 1f;
    [SerializeField] private float feebackShakeIntensity = 2f;

    [Header("NotCatchedZone")]
    [SerializeField] private GameObject notCatchedZone;

    [Header("Player object")]
    [SerializeField] Player player;

    // Game related
    public bool IsPaused { get; private set; }
    public int PlayerLife { get; private set; }
    public int CurrentScore { get; private set; }
    public bool IsGameOver { get; private set; } = true;
    //public bool IsIntroStillRunning { get; private set; } = false;


    // score change event
    public delegate void ScoreChanged();
    public event ScoreChanged OnScoreChanged;

    // RealView event
    public event EventHandler<RealViewEventArgs> OnRealViewToggle;
    public class RealViewEventArgs : EventArgs
    {
        public bool isRealViewActive;
    }

    // RealView related variables
    public bool IsRealViewEnabled { get; private set; }
    public bool IsRealViewInCooldown { get; private set; }
    public float RealViewEnabledFill { get; private set; }
    public float RealViewCooldownFill { get; private set; }

    private float realViewEnabledTimer;
    private float realViewCooldownTimer;
    private Coroutine realViewActiveCR, realViewInCooldownCR;

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
        HandleRealViewPublicTimers();
    }

    private void InputManager_OnRealViewKeyPressed()
    {
        if (!IsGameOver && !IsPaused)
        {
            EnableRealView();
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

    public void AdjustNotCatchedCheckZone(float playerY)
    {
        notCatchedZone.GetComponent<NotCatchedZone>().AdjustPositionToPlayerY(playerY);
    }

    #region Life, highscore related stuff

    public void IncreaseCurrentScore()
    {
        UpdateScore();
        StartCoroutine(DisplayPositiveFeedback());
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

    public void RealItemMissed()
    {
        UpdatePlayerLife(-1);
        StartCoroutine(DisplayLifeLostFeedback());
    }

    private void UpdatePlayerLife(int value)
    {
        if (PlayerLife + value <= 0)
        {
            PlayerLife = 0;
            GameOver();
        }
        else
        {
            PlayerLife += value;
        }
    }
    IEnumerator DisplayLifeLostFeedback()
    {
        AudioManager.GetInstance().PlayDamageSound();
        canvasManager.ActivateCanvas(CanvasType.LifeLostFeedbackScreen, true);
        yield return new WaitForSeconds(lifeLostFeedbackInterval);
        canvasManager.ActivateCanvas(CanvasType.LifeLostFeedbackScreen, false);
    }

    IEnumerator DisplayPositiveFeedback()
    {
        AudioManager.GetInstance().PlayPositiveFeedbackSound();
        canvasManager.ActivateCanvas(CanvasType.PositiveFeedbackScreen, true);
        yield return new WaitForSeconds(lifeLostFeedbackInterval);
        canvasManager.ActivateCanvas(CanvasType.PositiveFeedbackScreen, false);
    }

    //public float GetLifeLostFeedbackInterval() => lifeLostFeedbackInterval;
    //public float GetFeebackShakeIntensity() => feebackShakeIntensity;

    private void InitPlayerLife()
    {
        PlayerLife = initialLifeCount;
    }
    public int GetInitialLifeCount() => initialLifeCount;

    #endregion


    #region Basic Game Stuff (start, reset, pause, etc)

    public void StartGame()
    {        
        IsGameOver = false;
        UpdateScore(true);
        player.ResetPlayerPositionToStart();
        InitPlayerLife();
        ResetRealViewForStart();
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

        StartCoroutine(DisplayLifeLostFeedback());
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


    #region RealView Functions

    public float GetRealViewEnabledIntervalTotal() => realViewEnabledInterval;
    public float GetRealViewCooldownIntervalTotal() => realViewCooldownInterval;

    private void ResetRealViewForStart()
    {
        if (realViewActiveCR != null)
        {
            StopCoroutine(realViewActiveCR);
            DeactivateRealViewCounter();
        }

        if (realViewInCooldownCR != null)
        {
            StopCoroutine(realViewInCooldownCR);
            IsRealViewInCooldown = false;
            realViewInCooldownCR = null;
        }
    }

    private void HandleRealViewPublicTimers()
    {
        if (IsRealViewEnabled)
        {
            realViewEnabledTimer -= Time.deltaTime;
            RealViewEnabledFill = realViewEnabledTimer / realViewEnabledInterval;
        }
        else
        {
            realViewEnabledTimer = realViewEnabledInterval;
            if (!IsRealViewInCooldown)
            {
                RealViewEnabledFill = 1f;

                realViewCooldownTimer = 0f;
                RealViewCooldownFill = 0f;
            }
            else
            {
                RealViewEnabledFill = 0f;

                realViewCooldownTimer += Time.deltaTime;
                RealViewCooldownFill = realViewCooldownTimer / realViewCooldownInterval;
            }
        }
    }

    public void EnableRealView()
    {
        if (!IsRealViewEnabled && !IsRealViewInCooldown)
        {
            if (realViewActiveCR == null) //making sure not to start several CR
            {
                realViewActiveCR = StartCoroutine(RealViewCounter());
            }
        }

    }

    IEnumerator RealViewCounter()
    {
        ActivateRealViewCounter();

        yield return new WaitForSeconds(realViewEnabledInterval);

        DeactivateRealViewCounter();

        // start cooldown
        if (realViewInCooldownCR == null) //making sure not to start several CR
        {
            realViewInCooldownCR = StartCoroutine(RealViewCooldownCounter());
        }
    }

    private void ActivateRealViewCounter()
    {
        IsRealViewEnabled = true;
        // invoke event
        OnRealViewToggle?.Invoke(this, new RealViewEventArgs() { isRealViewActive = true });
        //bloomComponent.active = true;
    }

    private void DeactivateRealViewCounter()
    {
        IsRealViewEnabled = false;
        realViewActiveCR = null;
        // invoke event
        OnRealViewToggle?.Invoke(this, new RealViewEventArgs() { isRealViewActive = false });
        //bloomComponent.active = false;
    }

    IEnumerator RealViewCooldownCounter()
    {
        IsRealViewInCooldown = true;
        yield return new WaitForSeconds(realViewCooldownInterval);
        IsRealViewInCooldown = false;
        realViewInCooldownCR = null;

    }

    #endregion
}
