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

    void Start()
    {
        gameStart.Invoke();
        Time.timeScale = 1.0f;
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
        gameRestart.Invoke();
        Time.timeScale = 1.0f;
    }

    public void IncreaseScore(int increment)
    {
        Debug.Log("current score is");
        Debug.Log(score);
        score += increment;
        SetScore(score);
        Debug.Log("score after is");
        Debug.Log(score);
    }

    public void SetScore(int score)
    {
        Debug.Log("set sccore is invoked from game manager");
        Debug.Log(score);
        scoreChange.Invoke(score);
    }


    public void GameOver()
    {
        Debug.Log("gameover called");
        Time.timeScale = 0.0f;
        gameOver.Invoke();
    }
}