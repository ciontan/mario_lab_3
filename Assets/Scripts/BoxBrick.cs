using System.Collections;
using UnityEngine;

public class BoxBrick : MonoBehaviour
{
    [SerializeField] private AudioSource sfx;       // coin SFX
    [SerializeField] private LayerMask playerMask;  // layer for Mario

    [SerializeField] private Sprite usedSprite;     // sprite for after box is used
    [SerializeField] private GameObject coinPrefab; // prefab to spawn

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private bool used = false;
    public GameManager gameManager;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (used) return;

        // Must be the player (by layer)
        if (((1 << col.collider.gameObject.layer) & playerMask) == 0) return;

        // Accept hit only from below.
        bool fromBelow = false;

        // Primary: contact normal on THIS box tends to be ~Vector2.down when hit on its bottom.
        for (int i = 0; i < col.contactCount; i++)
        {
            var n = col.GetContact(i).normal;
            if (n.y < -0.5f) { fromBelow = true; break; }
        }

        // Fallback: player moving upward, but only if below the box center
        if (!fromBelow && col.relativeVelocity.y > 0.1f)
        {
            if (col.transform.position.y < transform.position.y)
                fromBelow = true;
        }

        if (!fromBelow) return;

        StartCoroutine(TriggerBox());
    }

    private IEnumerator TriggerBox()
    {
        used = true;

        if (sfx) sfx.Play();

        // Change to "used" sprite
        if (usedSprite != null)
            sr.sprite = usedSprite;

        // Spawn coin prefab
        if (coinPrefab != null)
        {
            // Calculate spawn position 1 unit above the box
            Vector3 localSpawnPos = new Vector3(0, 1f, 0);
            Vector3 spawnPosition = transform.TransformPoint(localSpawnPos);
            Debug.Log($"Box position: {transform.position}, Spawn position: {spawnPosition}");
            GameObject coinObj = Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
            Coin coinScript = coinObj.GetComponent<Coin>();
            if (coinScript != null)
                coinScript.Animate();
            // Increase score and specify it's from a coin (true)
            gameManager.IncreaseScore(1, true);
            Destroy(coinObj, 1f); // coin disappears after 1s
        }

        yield return null;
    }
}