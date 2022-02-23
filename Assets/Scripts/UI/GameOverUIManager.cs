using UnityEngine;
using TMPro;

public class GameOverUIManager : MonoBehaviour
{
    [SerializeField] TMP_Text highScore;

    private void OnEnable()
    {
        int maxScoreReached = Game.GetInstance().CurrentScore;
        maxScoreReached = maxScoreReached == 0 ? 1 : maxScoreReached;
        highScore.text = maxScoreReached.ToString();
    }
    
}
