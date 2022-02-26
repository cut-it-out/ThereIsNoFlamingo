using UnityEngine;
using TMPro;

public class DisplayScore : MonoBehaviour
{
    [SerializeField] TMP_Text scoreText;
    
    Game game;

    private void Awake()
    {
        game = Game.GetInstance();
        Game_OnScoreChanged();
    }

    private void OnEnable()
    {
        game.OnScoreChanged += Game_OnScoreChanged;
        Game_OnScoreChanged();
    }
    private void OnDisable()
    {
        game.OnScoreChanged -= Game_OnScoreChanged;
    }

    private void Game_OnScoreChanged()
    {
        scoreText.text = game.CurrentScore.ToString();
    }

}
