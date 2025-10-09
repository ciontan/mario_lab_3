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
        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.linearVelocity.x));
        if (Mathf.Abs(moveDirection) > 0)
        {
            Vector2 movement = new Vector2(moveDirection, 0);
            if (marioBody.linearVelocity.magnitude < maxSpeed)
                marioBody.AddForce(movement * speed);
        }
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
        // Log collision information
        Debug.Log($"Collision with {col.gameObject.name} on layer {col.gameObject.layer}");
        Debug.Log($"Current ground state: {onGroundState}, isJumping: {isJumping}");

        // Get contact points for debugging
        ContactPoint2D[] contacts = new ContactPoint2D[col.contactCount];
        col.GetContacts(contacts);
        foreach (ContactPoint2D contact in contacts)
        {
            Debug.Log($"Contact normal: {contact.normal}, relative velocity: {col.relativeVelocity}");
        }

        // Check if this is a valid ground collision
        if (((collisionLayerMask & (1 << col.transform.gameObject.layer)) > 0) && !onGroundState)
        {
            // Only set ground state if we're moving downward or at the peak of our jump
            if (marioBody.linearVelocity.y <= 0.1f)
            {
                onGroundState = true;
                isJumping = false;  // Reset jumping state
                // update animator state
                marioAnimator.SetBool("onGround", onGroundState);
                Debug.Log("Ground state set to true");
            }
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
    }
    private bool jumpedState = false;
    public void Jump()
    {
        if (alive && onGroundState)
        {
            // jump
            marioBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            onGroundState = false;
            jumpedState = true;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);

        }
    }


    public void JumpHold()
    {
        if (alive && jumpedState)
        {
            // jump higher
            marioBody.AddForce(Vector2.up * jumpForce * holdForce, ForceMode2D.Force);
            jumpedState = false;

        }
    }

    private int moveDirection = 0; // -1 for left, 1 for right, 0 for none
    public void MoveCheck(int value)
    {
        moveDirection = value;
        FlipMarioSprite(value);
    }
}