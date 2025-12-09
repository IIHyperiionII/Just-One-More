using UnityEngine;
using System.Collections;

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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentPushes = 0;

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
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
    
        Vector3 direction = mousePos - transform.position;
        direction.z = 0;
        
        if (currentPushes == 0)
            previewLine.material.color = Color.green;
        else if (currentPushes == 1)
            previewLine.material.color = Color.orange;
        else if (currentPushes == 2)
            previewLine.material.color = Color.red;

        previewLine.SetPosition(0, transform.position + direction.normalized * 0.3f);
        previewLine.SetPosition(1, transform.position + direction.normalized * 1.2f);
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
