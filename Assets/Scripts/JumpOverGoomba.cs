using UnityEngine;
using TMPro;

public class JumpOverGoomba : MonoBehaviour
{
    public Transform enemyLocation;
    public TextMeshProUGUI scoreText;

    [System.NonSerialized]
    public int score = 0;

    private bool countScoreState = false;
    public TextMeshProUGUI finalScoreText;
    public GameObject gameOverUI;
    public GameObject gameStartScore;
    public GameObject gameStartResetButton;

    private void Start()
    {
        // Initialize UI elements after all components are ready
        if (scoreText != null)
        {
            scoreText.SetText("Score: 0");
        }
        if (finalScoreText != null)
        {
            finalScoreText.SetText("Score: 0");
        }

        // Set initial UI states
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
        if (gameStartResetButton != null)
        {
            gameStartResetButton.SetActive(true);
        }
        if (gameStartScore != null)
        {
            gameStartScore.SetActive(true);
        }
    }

    public void gameOver()
    {
        Debug.Log("gameover triggered");
        gameOverUI.SetActive(true);
        gameStartResetButton.SetActive(false);
        gameStartScore.SetActive(false);
        finalScoreText.text = "Score: " + score.ToString();
    }

    void FixedUpdate()
    {
        // Check if player is near the Goomba to increment score
        if (countScoreState)
        {
            if (Mathf.Abs(transform.position.x - enemyLocation.position.x) < 0.5f)
            {
                countScoreState = false;
                score++;
                scoreText.text = "Score: " + score.ToString();
                Debug.Log(score);
            }
        }
    }

    public void OnPlayerJump()
    {
        countScoreState = true;
    }
}