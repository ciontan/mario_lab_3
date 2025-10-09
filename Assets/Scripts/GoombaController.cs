using System.Collections;
using UnityEngine;

public class GoombaController : MonoBehaviour
{
    // Define a delegate and event for the stomping
    public delegate void GoombaStompedEventHandler(int points);
    public static event GoombaStompedEventHandler OnGoombaStomped;

    private Animator animator;
    private bool isStomped = false;
    private BoxCollider2D sideCollider; // Reference to side trigger collider
    private Collider2D mainCollider;    // Reference to main physical collider
    private EnemyMovement movementController;

    [SerializeField] private int pointsForStomp = 100;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Find the colliders correctly
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (var collider in colliders)
        {
            if (collider is BoxCollider2D && ((BoxCollider2D)collider).isTrigger)
            {
                // This is the side collision trigger
                sideCollider = collider as BoxCollider2D;
                Debug.Log("Found side trigger collider on Goomba");
            }
            else
            {
                // This is the main collider for stomping from above
                mainCollider = collider;
                Debug.Log("Found main collider on Goomba: " + collider.GetType().Name);
            }
        }

        movementController = GetComponent<EnemyMovement>();

        // Debug collider setup
        Debug.Log("Goomba collider setup: Main=" + (mainCollider != null) + ", Side=" + (sideCollider != null));
    }

    public void GetStomped()
    {
        // Prevent double processing
        if (isStomped) return;

        isStomped = true;

        // Log for debugging
        Debug.Log("Goomba stomped! Playing animation...");

        // Disable the side trigger collider immediately to prevent death
        if (sideCollider != null)
            sideCollider.enabled = false;

        // Disable physics collider or make it trigger to prevent further collisions
        if (mainCollider != null)
        {
            // Option 1: Keep collider but make it a trigger so it doesn't block physics
            mainCollider.isTrigger = true;

            // Option 2: Disable completely if option 1 doesn't look good
            // mainCollider.enabled = false;
        }

        // Stop movement
        if (movementController != null)
            movementController.enabled = false;

        // Stop any rigidbody movement
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // Fire score event
        if (OnGoombaStomped != null)
            OnGoombaStomped(pointsForStomp);

        // Trigger animation last (important!)
        if (animator != null)
        {
            animator.SetTrigger("stomped");

            // Get animation length for proper timing
            float animationLength = 0.75f; // Default if we can't determine

            // Try to get actual animation length if possible
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
            if (clipInfo.Length > 0)
            {
                animationLength = clipInfo[0].clip.length;
                Debug.Log($"Stomped animation length: {animationLength}s");
            }

            // Use the actual animation length for the delay
            StartCoroutine(DisableAfterDelay(animationLength));
        }
        else
        {
            Debug.LogWarning("No animator found on Goomba!");
            // Fallback if no animator
            StartCoroutine(DisableAfterDelay(1.0f));
        }
    }

    private IEnumerator DisableAfterDelay(float delay)
    {
        // Wait for the animation to play fully
        Debug.Log($"Waiting {delay} seconds before disabling Goomba");
        yield return new WaitForSeconds(delay);

        // Deactivate the gameObject
        Debug.Log("Disabling stomped Goomba");
        gameObject.SetActive(false);
    }
}