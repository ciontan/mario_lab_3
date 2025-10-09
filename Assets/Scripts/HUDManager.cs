using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class HUDManager : MonoBehaviour
{
    private Vector3[] scoreTextPosition = {
        new Vector3(-747, 473, 0),
        new Vector3(0, 0, 0)
        };
    private Vector3[] restartButtonPosition = {
        new Vector3(844, 455, 0),
        new Vector3(0, -150, 0)
    };

    public GameObject scoreText;
    public Transform restartButton;
    public GameObject gameOverPanel;

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the UI to game start state
        GameStart();

        // Find the GameManager for direct access
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("HUDManager could not find GameManager!");
        }

        Debug.Log("HUDManager initialized");
    }

    // Update is called once per frame
    void Update()
    {
        // No update logic needed
    }

    // Set UI to game start state - called by GameManager's gameStart event
    public void GameStart()
    {
        // hide gameover panel
        gameOverPanel.SetActive(false);
        scoreText.transform.localPosition = scoreTextPosition[0];
        restartButton.localPosition = restartButtonPosition[0];

        Debug.Log("UI reset to game start state");
    }

    // Update score display - called by GameManager's scoreChange event
    public void SetScore(int score)
    {
        scoreText.GetComponent<TextMeshProUGUI>().text = "Score: " + score.ToString();
        Debug.Log("Score display updated: " + score);
    }

    // Show game over UI - called by GameManager's gameOver event
    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        scoreText.transform.localPosition = scoreTextPosition[1];
        restartButton.localPosition = restartButtonPosition[1];

        Debug.Log("Game over UI displayed");
    }

    // Restart button click handler - Connect this to the button in the Inspector
    public void RestartButtonClicked()
    {
        Debug.Log("Restart button clicked in HUDManager");

        // Call GameManager's restart method
        if (gameManager != null)
        {
            gameManager.GameRestart();
        }
        else
        {
            // Fallback if reference is missing
            gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.GameRestart();
            }
            else
            {
                Debug.LogError("Could not find GameManager to restart game!");
            }
        }
    }
}