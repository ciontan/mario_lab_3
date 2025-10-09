//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using TMPro;
//using UnityEngine.SocialPlatforms.Impl;
//
//public class GameManagerScript : MonoBehaviour
//{
//    public TextMeshProUGUI finalScoreText;
//    public GameObject gameOverUI;
//
//    public GameObject gameStartScore;
//
//    public GameObject gameStartResetButton;
//
//    public JumpOverGoomba jumpOverGoomba;
//
//    void Start()
//    {
//        gameOverUI.SetActive(false);
//        gameStartResetButton.SetActive(true);
//        gameStartScore.SetActive(true);
//    }
//
//    void Update()
//    {
//
//    }
//
//    public void gameOver()
//    {
//        gameOverUI.SetActive(true);
//        gameStartResetButton.SetActive(false);
//        gameStartScore.SetActive(false);
//        finalScoreText.text = "Score: " + jumpOverGoomba.score.ToString();
//    }
//
//}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    // events
    public UnityEvent gameStart;
    public UnityEvent gameRestart;
    public UnityEvent<int> scoreChange;
    public UnityEvent gameOver;

    private int score = 0;

    // Reference to CoinAudioController
    private CoinAudioController coinAudioController;

    void Start()
    {
        gameStart.Invoke();
        Time.timeScale = 1.0f;

        // Get or add CoinAudioController
        coinAudioController = GetComponent<CoinAudioController>();
        if (coinAudioController == null)
        {
            coinAudioController = gameObject.AddComponent<CoinAudioController>();
            Debug.Log("Added CoinAudioController to GameManager");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GameRestart()
    {
        // reset score
        score = 0;
        SetScore(score);

        // Reset coin audio
        if (coinAudioController != null)
            coinAudioController.ResetCoinCount();

        gameRestart.Invoke();
        Time.timeScale = 1.0f;
    }

    // Use this overload to specify if the score increase is from a coin
    public void IncreaseScore(int increment, bool isCoin = false)
    {
        score += increment;

        // Play coin sound only if it's from a coin
        if (isCoin && coinAudioController != null)
        {
            coinAudioController.PlayCoinSound();
        }

        SetScore(score);
    }

    public void SetScore(int score)
    {
        scoreChange.Invoke(score);
    }


    public void GameOver()
    {
        Debug.Log("gameover called");
        Time.timeScale = 0.0f;
        gameOver.Invoke();
    }
}