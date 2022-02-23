using UnityEngine;
using TMPro;

public class DisplayScore : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;
    
    Game game;

    private void Awake()
    {
        game = Game.GetInstance();
        DisplayScore_OnScoreChanged();
    }

    private void OnEnable()
    {
        game.OnScoreChanged += DisplayScore_OnScoreChanged;
        DisplayScore_OnScoreChanged();
    }
    private void OnDisable()
    {
        game.OnScoreChanged -= DisplayScore_OnScoreChanged;
    }

    private void DisplayScore_OnScoreChanged()
    {
        scoreText.text = game.CurrentScore.ToString();
    }

}
