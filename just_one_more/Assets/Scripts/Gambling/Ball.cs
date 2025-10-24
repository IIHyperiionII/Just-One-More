using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour
{
    private float lifetime = 10f;
    private float destroyDelay = 0.2f;
    private bool scoreRegistered = false;

    void Start()
    {
        // Schedule the ball for destruction after its lifetime expires
        Destroy(gameObject, lifetime);
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bucket") && !scoreRegistered)
        {
            scoreRegistered = true;

            // Get the bucket it collided with and register the score
            Bucket bucket = collision.GetComponent<Bucket>();
            if (bucket != null)
            {
                bucket.OnBallEntered();
            }

            // Destroy the ball shortly right after scoring
            StartCoroutine(DestroyAfterDelay(destroyDelay));
        }
    }
    IEnumerator DestroyAfterDelay(float delay)
    {
        float elapsed = 0f;
        while (elapsed < delay)
        {
            elapsed += Time.unscaledDeltaTime;  // ignore timeScale
            yield return null;
        }
        Destroy(gameObject);
    }
}
