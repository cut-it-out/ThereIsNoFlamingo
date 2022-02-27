using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CanvasType
{
    IntroUI,
    GameUI,
    MainMenu,
    GameOver,
    LifeLostFeedbackScreen,
    YouDiedSplashScreen,
    PositiveFeedbackScreen,
    TutorialScreen
}

public class CanvasManager : Singleton<CanvasManager>
{
    List<CanvasController> canvasControllerList;
    CanvasController lastActiveCanvas;

    protected override void Awake()
    {
        base.Awake();
        canvasControllerList = GetComponentsInChildren<CanvasController>().ToList();
        canvasControllerList.ForEach(x => x.gameObject.SetActive(false));
        SwitchCanvas(CanvasType.MainMenu);
        
    }

    public void ActivateIntro()
    {
        // active intro canvas and trigger intro
        SwitchCanvas(CanvasType.TutorialScreen);
        canvasControllerList.Find(x => x.canvasType == CanvasType.TutorialScreen).GetComponent<TutorialScreenManager>().TriggerIntroStart();
    }

    public void SwitchCanvas(CanvasType _type)
    {
        if (lastActiveCanvas != null)
        {
            lastActiveCanvas.gameObject.SetActive(false);
        }

        CanvasController desiredCanvas = canvasControllerList.Find(x => x.canvasType == _type);
        if (desiredCanvas != null)
        {
            desiredCanvas.gameObject.SetActive(true);
            lastActiveCanvas = desiredCanvas;
        }
        else { Debug.LogWarning($"The {desiredCanvas.canvasType} canvas was not found!"); }
    }

    public void ActivateCanvas(CanvasType _type, bool desiredActiveState)
    {
        CanvasController desiredCanvas = canvasControllerList.Find(x => x.canvasType == _type);
        if (desiredCanvas != null)
        {
            desiredCanvas.gameObject.SetActive(desiredActiveState);
        }
        else { Debug.LogWarning($"The {desiredCanvas.canvasType} canvas was not found!"); }
    }
}
