using UnityEngine;
using System.Collections;

// Plinko ball with player-controlled push mechanic (up to 3 pushes)
// Shows direction preview line and handles collision with pegs and buckets
public class Ball : MonoBehaviour
{
    private float lifetime = 30f;
    private float destroyDelay = 0.2f;
    private bool scoreRegistered = false;
    private float pushForce = 40f;
    private int maxPushes = 3;
    private int currentPushes;
    private Rigidbody2D rb;
    private LineRenderer previewLine;
    private Vector2 savedDirection;
    private bool isChargingPush;
    [SerializeField] private AudioClip pegHitSound;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Continuous collision detection prevents ball from falling through buckets at high speeds
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        currentPushes = 0;

        // Create dynamic preview line to show push direction
        previewLine = new GameObject("Push Preview").AddComponent<LineRenderer>();
        previewLine.transform.SetParent(transform);

        previewLine.positionCount = 2;
        previewLine.startWidth = 0.15f;
        previewLine.endWidth = 0.075f;
        previewLine.material = new Material(Shader.Find("Sprites/Default"));
        
        previewLine.material.color = Color.green;

        previewLine.sortingOrder = 15;
        previewLine.enabled = true;

        // Schedule the ball for destruction after its lifetime expires
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // Allow pushes only if player has pushes remaining and hasn't scored yet
        if (currentPushes < maxPushes && !scoreRegistered)
        {   
            UpdatePushPreview();

            // Charge on mouse click, shoots after some time
            if (Input.GetMouseButtonDown(0) && !isChargingPush)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                savedDirection = (mousePos - transform.position).normalized;
                StartCoroutine(PushTowardsMouse());
            }
        } 
        else 
        {
            // Hide preview when out of pushes or scored
            previewLine.enabled = false;
        }
    }

    // Charge push for 0.5 seconds (white preview) then apply force
    IEnumerator PushTowardsMouse()
    {
        isChargingPush = true;

        // Turn preview white during charge
        if (previewLine != null)
            previewLine.material.color = Color.white;

        yield return new WaitForSeconds(0.5f);

        // Apply force and increment push counter
        rb.AddForce(savedDirection * pushForce, ForceMode2D.Impulse);

        currentPushes++;

        isChargingPush = false;
    }

    // Update preview line color and position based on remaining pushes
    void UpdatePushPreview() 
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
    
        Vector3 direction = mousePos - transform.position;
        direction.z = 0;
        
        // Color indicates remaining pushes: green (0 used), orange (1 used), red (2 used)
        if (!isChargingPush) {
            if (currentPushes == 0)
                previewLine.material.color = Color.green;
            else if (currentPushes == 1)
                previewLine.material.color = Color.orange;
            else if (currentPushes == 2)
                previewLine.material.color = Color.red;
        }

        // Draw line from ball towards mouse
        previewLine.SetPosition(0, transform.position + direction.normalized * 0.3f);
        previewLine.SetPosition(1, transform.position + direction.normalized * 1.2f);
    }

    // Detect when ball enters a bucket and register the score
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bucket") && !scoreRegistered)
        {
            scoreRegistered = true;

            Bucket bucket = collision.GetComponent<Bucket>();
            if (bucket != null)
            {
                bucket.OnBallEntered();
            }

            StartCoroutine(DestroyAfterDelay(destroyDelay));
        }
    }

    // Play sound when ball hits a peg, with volume based on impact force
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Peg"))
        {
            // Calculate volume from impact force (0.1 to 0.4 range)
            float impact = collision.relativeVelocity.magnitude;
            float volume = Mathf.Clamp(impact * 0.1f, 0.1f, 0.4f);
            float randPitch = Random.Range(0.75f, 1.25f);

            SoundController.Instance.PlaySound(pegHitSound, volume, randPitch);
        }
    }

    // Destroy ball after delay using unscaled time (ignores time pause/slowdown)
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
