using TMPro;
using UnityEngine;

public class ScoreUIManager : SingletonMonoBehaviour<ScoreUIManager>
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private int score;

    public int Score => score;


    public int AddScore(int scoreAmount)
    {
        score += scoreAmount;
        UpdateScoreText();
        return score;
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreText();
    }
    public int GetScore() { return score; }

    private void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }
}
