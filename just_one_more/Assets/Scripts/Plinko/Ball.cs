using UnityEngine;

public class Ball : MonoBehaviour
{
    private float lifetime = 15f;
    private float destroyDelay = 0.5f;

    private BallSpawner spawner;
    private bool scoreRegistered = false;

    void Start()
    {
        spawner = FindAnyObjectByType<BallSpawner>();
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bucket") && !scoreRegistered)
        {
            scoreRegistered = true;

            Bucket bucket = collision.GetComponent<Bucket>();
            if (bucket != null)
            {
                bucket.OnBallEntered(this);
            }

            Destroy(gameObject, destroyDelay);
        }
    }

    void OnDestroy()
    {
        if (spawner != null) spawner.NotifyBallDestroyed(gameObject);
    }
}
