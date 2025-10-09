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
    //public TextMeshProUGUI scoreText;
    public GameObject enemies;
    //public JumpOverGoomba jumpOverGoomba;

    public GameManager gameManager;

    public Animator marioAnimator;

    public AudioSource marioAudio;
    // state
    [System.NonSerialized]
    public bool alive = true;
    public AudioSource marioDeathAudio;
    public float deathImpulse = 15;
    private bool moving = false;
    private bool jumpedState = false;
    public Transform gameCamera;
    int collisionLayerMask = (1 << 3) | (1 << 6) | (1 << 7);

    // Start is called before the first frame update
    void Start()
    {
        // Set to be 30 FPS
        Application.targetFrameRate = 30;

        // Get necessary components
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();

        // Set initial state
        marioAnimator.SetBool("onGround", onGroundState);
        alive = true; // Make sure Mario is alive at start

        // Make sure we have a GameManager reference
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
            if (gameManager == null)
            {
                GameObject managerObject = GameObject.FindGameObjectWithTag("Manager");
                if (managerObject != null)
                {
                    gameManager = managerObject.GetComponent<GameManager>();
                }
            }

            if (gameManager != null)
                Debug.Log("Found GameManager reference");
            else
                Debug.LogError("Could not find GameManager reference!");
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
        //    if (marioBody.velocity.x > 0.1f)
        //        marioAnimator.SetTrigger("onSkid");
        //}
        //
        //if (Input.GetKeyDown("d") && !faceRightState)
        //{
        //    faceRightState = true;
        //    //marioSprite.flipX = false;
        //    if (marioBody.velocity.x < -0.1f)
        //        marioAnimator.SetTrigger("onSkid");
        //}

        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.linearVelocity.x));
    }
    

    void PlayJumpSound()
    {
        // play jump sound
        marioAudio.PlayOneShot(marioAudio.clip);
    }

    void PlayDeathImpulse()
    {
        marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Only process enemy collisions if Mario is still alive
        if (other.gameObject.CompareTag("Enemy") && alive)
        {
            Debug.Log("Collided with goomba side trigger collider - this is a death!");

            // This is specifically for side collisions or bottom collisions
            // Make sure this trigger collider is set up to only detect side hits

            // Play death animation
            marioAnimator.Play("mario-die");

            // Play death sound
            if (marioDeathAudio != null)
                marioDeathAudio.PlayOneShot(marioDeathAudio.clip);

            // Set alive to false to stop movement
            alive = false;

            // Play death impulse animation
            PlayDeathImpulse();

            // Show game over screen after a delay to let animation play
            Invoke("GameOverScene", 1.5f);

            // Log for debugging
            Debug.Log("Mario death sequence triggered");
        }
    }

    void GameOverScene()
    {
        Debug.Log("GameOverScene method called in PlayerMovement");

        // Try to use existing reference
        if (gameManager != null)
        {
            gameManager.GameOver();
            Debug.Log("GameOver called on existing GameManager reference");
            return;
        }

        // Try to find GameManager in scene
        gameManager = GameObject.FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            gameManager.GameOver();
            Debug.Log("Found GameManager and called GameOver()");
            return;
        }

        // Try to find by tag as last resort
        GameObject managerObject = GameObject.FindGameObjectWithTag("Manager");
        if (managerObject != null)
        {
            GameManager manager = managerObject.GetComponent<GameManager>();
            if (manager != null)
            {
                manager.GameOver();
                Debug.Log("Found GameManager by tag and called GameOver()");
                return;
            }
        }

        Debug.LogError("CRITICAL ERROR: Could not find any GameManager!");
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // Check if collided with ground/platforms
        if (((collisionLayerMask & (1 << col.transform.gameObject.layer)) > 0) & !onGroundState)
        {
            onGroundState = true;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);
        }

        // Check if we're stomping a Goomba
        if (col.gameObject.CompareTag("Enemy"))
        {
            // Calculate if Mario is above the Goomba
            float marioBottom = transform.position.y - GetComponent<Collider2D>().bounds.extents.y;
            float goombaTop = col.transform.position.y + col.collider.bounds.extents.y;

            // Also check velocity - we should be moving downward
            bool comingFromAbove = marioBody.linearVelocity.y <= 0;

            // If Mario's bottom is above Goomba's top, it's a stomp
            if (marioBottom > goombaTop - 0.1f && comingFromAbove)
            {
                Debug.Log("Stomped on Goomba from above!");

                // Apply a small bounce when stomping
                marioBody.linearVelocity = new Vector2(marioBody.linearVelocity.x, 0);
                marioBody.AddForce(Vector2.up * jumpForce / 2, ForceMode2D.Impulse);

                // Get the GoombaController and call GetStomped
                GoombaController goomba = col.gameObject.GetComponent<GoombaController>();
                if (goomba != null)
                {
                    goomba.GetStomped();
                    Debug.Log("Called GetStomped() from proper stomp collision");
                }
            }
            else
            {
                // If we're hitting the Goomba from the side, don't do anything here
                // Side collisions should be handled by OnTriggerEnter2D for isTrigger colliders
                Debug.Log("Hit Goomba but not from above - no stomping");
            }
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
            //    if (marioBody.velocity.magnitude < maxSpeed)
            //        marioBody.AddForce(movement * speed);
            //}
            //
            // stop
            //if (Input.GetKeyUp("a") || Input.GetKeyUp("d"))
            //{
            //    // stop
            //    marioBody.velocity = Vector2.zero;
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
            //    if (marioBody.velocity.y > 0 && marioBody.velocity.y < maxJumpVelocity)
            //    {
            //        marioBody.AddForce(Vector2.up * holdForce, ForceMode2D.Force);
            //    }
            //}
            //
            //// Short hop: if released early, cut upward velocity
            //if (Input.GetKeyUp("space"))
            //{
            //    if (marioBody.velocity.y > 0)
            //    {
            //        marioBody.velocity = new Vector2(marioBody.velocity.x, marioBody.velocity.y * 0.25f);
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

    //public void ResetGame()
    //{
    //    // reset position
    //    marioBody.transform.position = new Vector3(-5.00f, -2.50f, 0.0f);
    //    // reset sprite direction
    //    faceRightState = true;
    //    marioSprite.flipX = false;
    //    // reset score
    //    //scoreText.text = "Score: 0";
    //    // reset Goomba
    //    foreach (Transform eachChild in enemies.transform)
    //    {
    //        eachChild.localPosition = eachChild.GetComponent<EnemyMovement>().startPosition;
    //    }
    //    // reset score
    //    jumpOverGoomba.score = 0;
    //    jumpOverGoomba.gameOverUI.SetActive(false);
    //    jumpOverGoomba.gameStartResetButton.SetActive(true);
    //    jumpOverGoomba.gameStartScore.SetActive(true);
    //    // reset animation
    //    marioAnimator.SetTrigger("gameRestart");
    //    alive = true;
    //    gameCamera.position = new Vector3(0, 0, -1);
    //
    //    //Scene currentScene = SceneManager.GetActiveScene();
    //    //SceneManager.LoadScene(currentScene.name);
    //}

    public void ResetGame()
    {
        // reset position
        marioBody.transform.position = new Vector3(-5.33f, -2.36f, 0.0f);
        // reset sprite direction
        faceRightState = true;
        marioSprite.flipX = false;

        // reset animation
        marioAnimator.SetTrigger("gameRestart");
        alive = true;

        // reset camera position
        gameCamera.position = new Vector3(0, 0, -10);

        // Reset the score in GameManager
        if (gameManager != null)
            gameManager.ResetScore();

        // Respawn Goombas
        foreach (Transform eachChild in enemies.transform)
        {
            // Check if the child is active
            if (!eachChild.gameObject.activeSelf)
            {
                // Respawn it by creating a new instance
                Instantiate(eachChild.gameObject, eachChild.GetComponent<EnemyMovement>().startPosition, Quaternion.identity, enemies.transform);
            }
            else
            {
                // Just reset position
                eachChild.localPosition = eachChild.GetComponent<EnemyMovement>().startPosition;
            }
        }

        // Resume time in case it was paused
        Time.timeScale = 1.0f;
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
