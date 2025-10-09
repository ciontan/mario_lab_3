using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JumpOverGoomba : MonoBehaviour
{
    public Transform enemyLocation;
    //public TextMeshProUGUI scoreText;
    private bool onGroundState;
    private bool jumpInProgress = false;
    private bool isCollidingWithObstacle = false;

    //[System.NonSerialized]
    //public int score = 0; // we don't want this to show up in the inspector

    private bool countScoreState = false;
    public Vector3 boxSize;
    public float maxDistance;
    public LayerMask layerMask;
    // Start is called before the first frame update
    //public TextMeshProUGUI finalScoreText;
    //public GameObject gameOverUI;

    //public GameObject gameStartScore;

    //public GameObject gameStartResetButton;

    //public JumpOverGoomba jumpOverGoomba;
    public GameManager gameManager;

    void Start()
    {
        //gameOverUI.SetActive(false);
        //gameStartResetButton.SetActive(true);
        //gameStartScore.SetActive(true);
        gameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check ground state every frame
        CheckGroundState();

        // Handle jump input - only allow jump if on ground AND not colliding with obstacle
        if (Input.GetKeyDown(KeyCode.Space) && onGroundState && !isCollidingWithObstacle && !jumpInProgress)
        {
            jumpInProgress = true;
            countScoreState = true;
            Debug.Log("Jump initiated");
        }
    }

    //public void gameOver()
    //{
    //    Debug.Log("gameover triggered");
    //    gameOverUI.SetActive(true);
    //    gameStartResetButton.SetActive(false);
    //    gameStartScore.SetActive(false);
    //    finalScoreText.text = "Score: " + jumpOverGoomba.score.ToString();
    //}

    void FixedUpdate()
    {
        //// when jumping, and Goomba is near Mario and we haven't registered our score
        //if (jumpInProgress && countScoreState)
        //{
        //    if (Mathf.Abs(transform.position.x - enemyLocation.position.x) < 0.5f)
        //    {
        //        countScoreState = false;
        //        score++;
        //        scoreText.text = "Score: " + score.ToString();
        //        Debug.Log(score);
        //    }
        //}

        // No longer increase score when jumping over Goomba
        if (!onGroundState && countScoreState)
        {
            if (Mathf.Abs(transform.position.x - enemyLocation.position.x) < 0.5f)
            {
                countScoreState = false;
                // Removed: gameManager.IncreaseScore(1);
                Debug.Log("Jumped over Goomba but score not increased");
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // If colliding with an obstacle, prevent jumping
        if (col.gameObject.CompareTag("Obstacle"))
        {
            isCollidingWithObstacle = true;
            onGroundState = false;
            Debug.Log("Hit obstacle - jumping disabled");
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        // While in contact with obstacle, keep jumping disabled
        if (col.gameObject.CompareTag("Obstacle"))
        {
            isCollidingWithObstacle = true;
            onGroundState = false;
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Obstacle"))
        {
            isCollidingWithObstacle = false;
            Debug.Log("Left obstacle - jumping may be enabled if on ground");
        }

        if (col.gameObject.CompareTag("Ground"))
        {
            onGroundState = false;
            jumpInProgress = false;
        }
    }

    private void CheckGroundState()
    {
        // Don't allow ground state to be true if colliding with obstacle
        if (isCollidingWithObstacle)
        {
            onGroundState = false;
            return;
        }

        bool wasOnGround = onGroundState;
        onGroundState = onGroundCheck();

        // Reset jump when landing
        if (!wasOnGround && onGroundState)
        {
            jumpInProgress = false;
            Debug.Log("Landed on ground");
        }
    }

    private bool onGroundCheck()
    {
        if (Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, maxDistance, layerMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position - transform.up * maxDistance, boxSize);
    }
}