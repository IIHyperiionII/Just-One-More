using UnityEngine;

public class Ball : MonoBehaviour
{
    private float lifetime = 10f;
    private float destroyDelay = 0.2f;
    private bool scoreRegistered = false;

    void Start()
    {
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
}
