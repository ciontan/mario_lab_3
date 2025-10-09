using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 70;

    public float maxSpeed = 80;
    public float jumpForce = 30f;
    public float holdForce = 10f;
    public float maxJumpVelocity = 20f;
    private bool isJumping = false;
    private bool onGroundState = true;
    private Rigidbody2D marioBody;
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;
    public TextMeshProUGUI scoreText;
    public GameObject enemies;
    public JumpOverGoomba jumpOverGoomba;

    public GameManagerScript gameManager;
    public Animator marioAnimator;

    public AudioSource marioAudio;
    // state
    [System.NonSerialized]
    public bool alive = true;
    public AudioClip marioDeath;
    public float deathImpulse = 15;
    private bool moving = false;
    private bool jumpedState = false;
    int collisionLayerMask = (1 << 3) | (1 << 6) | (1 << 7);

    // Start is called before the first frame update
    void Start()
    {
        // Set initial values for physics
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        marioAnimator = GetComponent<Animator>();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManagerScript>();
        marioAnimator.SetBool("onGround", onGroundState);

        // Get the PlayerInput component and disable all action maps initially
        var playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        if (playerInput != null)
        {
            // Disable all action maps first
            foreach (var actionMap in playerInput.actions.actionMaps)
            {
                actionMap.Disable();
            }

            // Enable only the gameplay action map
            var gameplayMap = playerInput.actions.FindActionMap("gameplay");
            if (gameplayMap != null)
            {
                gameplayMap.Enable();
            }
        }
    }

    void Update()
    {
        //float moveHorizontal = Input.GetAxisRaw("Horizontal");
        //if (moveHorizontal < 0 && faceRightState)
        //{
        //    faceRightState = false;
        //    //marioSprite.flipX = true;
        //}
//
        //if (moveHorizontal > 0 && !faceRightState)
        //{
        //    faceRightState = true;
        //    //marioSprite.flipX = false;
        //}
//
        //if (Input.GetKeyDown("a") && faceRightState)
        //{
        //    faceRightState = false;
        //    //marioSprite.flipX = true;
        //    if (marioBody.linearVelocity.x > 0.1f)
        //        marioAnimator.SetTrigger("onSkid");
        //}
//
        //if (Input.GetKeyDown("d") && !faceRightState)
        //{
        //    faceRightState = true;
        //    //marioSprite.flipX = false;
        //    if (marioBody.linearVelocity.x < -0.1f)
        //        marioAnimator.SetTrigger("onSkid");
        //}

        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.linearVelocity.x));
    }

    void PlayJumpSound()
    {
        // play jump sound
        marioAudio.PlayOneShot(marioAudio.clip);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Collided with goomba!");

            // Check if already dead to prevent multiple calls
            if (!alive) return;

            // Mark as dead first
            alive = false;

            // Play death animation
            marioAnimator.Play("mario-die");

            // Create a separate AudioSource for death sound so it won't be interrupted
            AudioSource.PlayClipAtPoint(marioDeath, transform.position, 1.0f);
            Debug.Log("Using PlayClipAtPoint for death audio");

            // Add death impulse
            marioBody.linearVelocity = Vector2.zero;
            marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);

            // Delay the game over screen to allow animation and sound to play
            StartCoroutine(DelayedGameOver());
        }
    }

    IEnumerator DelayedGameOver()
    {
        // Wait to let animation and sound play (use WaitForSecondsRealtime to ignore timeScale)
        yield return new WaitForSecondsRealtime(1.0f);
        Debug.Log("Showing game over screen");

        // Make sure not to set Time.timeScale = 0 until after the audio has played
        gameManager.gameOver();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (((collisionLayerMask & (1 << col.transform.gameObject.layer)) > 0) & !onGroundState)
        {
            onGroundState = true;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);
        }
    }

    // FixedUpdate may be called once per frame. See documentation for details.
    void FixedUpdate()
    {
        if (alive && moving)
        {
            Move(faceRightState == true ? 1 : -1);
            //float moveHorizontal = Input.GetAxisRaw("Horizontal");

            //if (Mathf.Abs(moveHorizontal) > 0)
            //{
            //    Vector2 movement = new Vector2(moveHorizontal, 0);
            //    // check if it doesn't go beyond maxSpeed
            //    if (marioBody.linearVelocity.magnitude < maxSpeed)
            //        marioBody.AddForce(movement * speed);
            //}
//
            // stop
            //if (Input.GetKeyUp("a") || Input.GetKeyUp("d"))
            //{
            //    // stop
            //    marioBody.linearVelocity = Vector2.zero;
            //}


            // Making the jump more like the actual mario game
            // Start jump

            //if (Input.GetKeyDown("space") && onGroundState)
            //{
            //    marioBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            //    onGroundState = false;
            //    isJumping = true;
            //    marioAnimator.SetBool("onGround", onGroundState);
            //}
//
            //// Hold jump: apply smaller force while rising
            //if (Input.GetKey("space") && isJumping)
            //{
            //    if (marioBody.linearVelocity.y > 0 && marioBody.linearVelocity.y < maxJumpVelocity)
            //    {
            //        marioBody.AddForce(Vector2.up * holdForce, ForceMode2D.Force);
            //    }
            //}
//
            //// Short hop: if released early, cut upward velocity
            //if (Input.GetKeyUp("space"))
            //{
            //    if (marioBody.linearVelocity.y > 0)
            //    {
            //        marioBody.linearVelocity = new Vector2(marioBody.linearVelocity.x, marioBody.linearVelocity.y * 0.25f);
            //    }
            //    isJumping = false;
            //}
        }

    }


    public void RestartButtonCallback(int input)
    {
        Debug.Log("Restart!");
        // reset everything
        ResetGame();
        // resume time
        Time.timeScale = 1.0f;
    }

    public void ResetGame()
    {
        // Load the current scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);

        // Note: The commented code below won't run because scene loading
        // will destroy this object before the remaining code executes
        // If you want to use the manual reset approach instead of scene loading,
        // remove the SceneManager.LoadScene line and uncomment the code below

        //// reset position
        //marioBody.transform.position = new Vector3(-5.00f, -2.50f, 0.0f);
        //// reset sprite direction
        //faceRightState = true;
        //marioSprite.flipX = false;
        //// reset score
        //scoreText.text = "Score: 0";
        //// reset Goomba
        //foreach (Transform eachChild in enemies.transform)
        //{
        //    eachChild.localPosition = eachChild.GetComponent<EnemyMovement>().startPosition;
        //}
        //// reset score
        //jumpOverGoomba.score = 0;
        //gameManager.gameOverUI.SetActive(false);
        //gameManager.gameStartResetButton.SetActive(true);
        //gameManager.gameStartScore.SetActive(true);
        //// reset animation
        //marioAnimator.SetTrigger("gameRestart");
        //alive = true;
        //gameCamera.position = new Vector3(0, 0, -0.5);
    }

    void FlipMarioSprite(int value)
    {
        if (value == -1 && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
            if (marioBody.linearVelocity.x > 0.05f)
                marioAnimator.SetTrigger("onSkid");

        }

        else if (value == 1 && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
            if (marioBody.linearVelocity.x < -0.05f)
                marioAnimator.SetTrigger("onSkid");
        }
    }

    void Move(int value)
    {

        Vector2 movement = new Vector2(value, 0);
        // check if it doesn't go beyond maxSpeed
        if (marioBody.linearVelocity.magnitude < maxSpeed)
            marioBody.AddForce(movement * speed);
    }

    public void MoveCheck(int value)
    {
        if (value == 0)
        {
            moving = false;
        }
        else
        {
            FlipMarioSprite(value);
            moving = true;
            Move(value);
        }
    }

    public void Jump()
    {
        if (alive && onGroundState)
        {
            // jump
            marioBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            onGroundState = false;
            jumpedState = true;
            isJumping = true;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);

        }
    }
    
    public void JumpHold()
    {
        if (alive && jumpedState)
        {
            // jump higher
            marioBody.AddForce(Vector2.up * jumpForce * 30, ForceMode2D.Force);
            jumpedState = false;
            isJumping = false;

        }
    }

}
