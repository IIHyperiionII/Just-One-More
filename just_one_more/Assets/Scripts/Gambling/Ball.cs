using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour
{
    private float lifetime = 10f;
    private float destroyDelay = 0.2f;
    private bool scoreRegistered = false;

    private float pushForce = 40f;
    private int maxPushes = 3;
    private int currentPushes;
    private Rigidbody2D rb;
    private LineRenderer previewLine;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentPushes = 0;

        previewLine = new GameObject("Push Preview").AddComponent<LineRenderer>();
        previewLine.transform.SetParent(transform);

        previewLine.positionCount = 2;
        previewLine.startWidth = 0.1f;
        previewLine.endWidth = 0.05f;
        previewLine.material = new Material(Shader.Find("Sprites/Default"));
        
        // Zelená s gradientem
        previewLine.startColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        previewLine.endColor = new Color(0.8f, 0.8f, 0.8f, 0.3f);
        
        previewLine.sortingOrder = 15;
        previewLine.enabled = true; // ZAPNUTÉ od začátku

        // Schedule the ball for destruction after its lifetime expires
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (currentPushes < maxPushes && !scoreRegistered)
        {
            UpdatePushPreview();

            if (Input.GetMouseButtonDown(0)) 
            {
                PushTowardsMouse();
            }
        } 
        else 
        {
            previewLine.enabled = false;
        }
    }

    void PushTowardsMouse() 
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;

        rb.AddForce(direction * pushForce, ForceMode2D.Impulse);
        currentPushes++;
    }

    void UpdatePushPreview() 
    {
        // ??? TODO: Change color based on pushCount?

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
    
        Vector3 direction = mousePos - transform.position;
        direction.z = 0;
        
        previewLine.SetPosition(0, transform.position + direction.normalized * 0.1f);
        previewLine.SetPosition(1, transform.position + direction.normalized * 0.4f);
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
