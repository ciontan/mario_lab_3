using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class GameManagerScript : MonoBehaviour
{
    public TextMeshProUGUI finalScoreText;
    public GameObject gameOverUI;

    public GameObject gameStartScore;

    public GameObject gameStartResetButton;

    public JumpOverGoomba jumpOverGoomba;

    void Start()
    {
        gameOverUI.SetActive(false);
        gameStartResetButton.SetActive(true);
        gameStartScore.SetActive(true);
    }

    void Update()
    {

    }

    public void gameOver()
    {
        gameOverUI.SetActive(true);
        gameStartResetButton.SetActive(false);
        gameStartScore.SetActive(false);
        finalScoreText.text = "Score: " + jumpOverGoomba.score.ToString();
    }

}