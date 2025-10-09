using System.Collections;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float jumpDuration = 0.5f;
    private Vector3 startPosition;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void Animate()
    {
        startPosition = transform.position;
        Debug.Log($"Starting coin animation from position: {startPosition}");
        StartCoroutine(JumpAnimation());
    }
    private IEnumerator JumpAnimation()
    {
        float elapsedTime = 0;

        while (elapsedTime < jumpDuration)
        {

            float jumpProgress = elapsedTime / jumpDuration;
            float heightCurve = Mathf.Sin(jumpProgress * Mathf.PI);
            transform.position = startPosition + new Vector3(0, heightCurve * jumpHeight, 0);
            if (jumpProgress > 0.7f)
            {
                Color coinColor = sr.color;
                coinColor.a = 1 - ((jumpProgress - 0.7f) / 0.3f);
                sr.color = coinColor;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}