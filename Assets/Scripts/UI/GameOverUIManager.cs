using UnityEngine;
using TMPro;

public class GameOverUIManager : MonoBehaviour
{
    [SerializeField] TMP_Text highScore;
    [SerializeField] TMP_Text levelReached;

    private void OnEnable()
    {
        int maxScoreReached = Game.GetInstance().CurrentScore;
        float maxLevelReached = Spawner.GetInstance().GetProgressMultiplier();
        //maxScoreReached = maxScoreReached == 0 ? 1 : maxScoreReached;
        highScore.text = maxScoreReached.ToString();
        levelReached.text = "LEVEL REACHED: " + maxLevelReached.ToString();
    }
    
}
