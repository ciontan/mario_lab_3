using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Unity Events - Observer Pattern implementation
    public UnityEvent gameStart = new UnityEvent();
    public UnityEvent gameRestart = new UnityEvent();
    public UnityEvent<int> scoreChange = new UnityEvent<int>();
    public UnityEvent gameOver = new UnityEvent();

    // UI references
    public GameObject gameOverUI;
    public TextMeshProUGUI scoreText;

    private int score = 0;

    // Subscribe to the GoombaController.OnGoombaStomped event when enabled
    private void OnEnable()
    {
        // Subscribe to the event
        GoombaController.OnGoombaStomped += OnGoombaStomped;
    }

    // Unsubscribe when disabled to prevent memory leaks
    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        GoombaController.OnGoombaStomped -= OnGoombaStomped;
    }

    void Start()
    {
        // Initialize game state
        score = 0;

        // Hide game over UI at startup
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }

        // Update score display
        SetScore(score);

        // Call the UnityEvent to notify observers about game start
        gameStart.Invoke();

        // Ensure time is running
        Time.timeScale = 1.0f;
    }

    // Event handler for GoombaController.OnGoombaStomped
    private void OnGoombaStomped(int points)
    {
        IncreaseScore(points);
        Debug.Log("Stomped a Goomba! Score: " + score);
    }

    // Restart the game - called by button click
    public void GameRestart()
    {
        // Reset score
        score = 0;
        SetScore(score);

        // Hide game over UI
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }

        // Call the UnityEvent to notify observers about restart
        gameRestart.Invoke();

        // Ensure time is running
        Time.timeScale = 1.0f;

        Debug.Log("Game restarted");
    }

    // Increase the score by a specified amount
    public void IncreaseScore(int increment)
    {
        score += increment;
        SetScore(score);
        Debug.Log("Score increased by: " + increment + ", Total: " + score);
    }

    // Update the score display and notify observers
    public void SetScore(int newScore)
    {
        score = newScore;

        // Update score text directly if available
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }

        // Notify observers through UnityEvent
        scoreChange.Invoke(score);
    }

    // Game over handler
    public void GameOver()
    {
        Debug.Log("Game Over triggered");

        // Show game over UI
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // Notify observers through UnityEvent
        gameOver.Invoke();

        // Pause the game
        Time.timeScale = 0.0f;
    }

    // Method specifically for the restart button to call
    public void RestartButtonClicked()
    {
        Debug.Log("Restart button clicked");
        GameRestart();
    }
}