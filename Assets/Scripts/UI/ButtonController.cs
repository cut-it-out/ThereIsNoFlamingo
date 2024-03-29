using UnityEngine;
using UnityEngine.UI;

public enum ButtonType
{
    START_GAME,
    PAUSE_GAME,
    RESUME_GAME,
    RESET_GAME,
    MAIN_MENU,
    QUIT,
    ENABLE_REAL_VIEW
}

[RequireComponent(typeof(Button))]
public class ButtonController : MonoBehaviour
{
    public ButtonType buttonType;

    CanvasManager canvasManager;
    Button menuButton;
    Game game;

    private void Start()
    {
        menuButton = GetComponent<Button>();
        menuButton.onClick.AddListener(OnButtonClicked);
        canvasManager = CanvasManager.GetInstance();
        game = Game.GetInstance();
    }

    void OnButtonClicked()
    {
        switch (buttonType)
        {
            case ButtonType.START_GAME:
                canvasManager.ActivateIntro();                
                break;
            case ButtonType.PAUSE_GAME:
                game.PauseGame();
                canvasManager.SwitchCanvas(CanvasType.MainMenu);
                break;
            case ButtonType.RESUME_GAME:
                game.ResumeGame();
                canvasManager.SwitchCanvas(CanvasType.GameUI);
                break;
            case ButtonType.RESET_GAME:
                game.ResetGame();
                canvasManager.SwitchCanvas(CanvasType.GameUI);
                break;
            case ButtonType.MAIN_MENU:
                break;
            case ButtonType.QUIT:
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                break;
            case ButtonType.ENABLE_REAL_VIEW:
                //Game.GetInstance().EnableRealView();
                break;
            default:
                break;
        }
    }
}
