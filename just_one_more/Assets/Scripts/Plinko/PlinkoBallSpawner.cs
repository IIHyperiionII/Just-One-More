using UnityEngine;

public class PlinkoBallSpawner : MonoBehaviour 
{
    [Header("Ball Setup")]
    public GameObject ballPrefab;           // Drag Ball prefab sem
    public Transform ballSpawnPoint;        // Spawn pozice
    
    [Header("Physics Settings")]
    public float initialRandomForce = 50f;  // Random sideways push
    
    [Header("Auto Cleanup")]
    public float destroyAfterSeconds = 10f;

    private GameObject currentBall;
    void Update() 
    {
        // Test: Spacebar pro drop ball
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            DropBall();
        }
    }

    public void DropBall()
    {
        // Destroy old ball pokud existuje
        if (currentBall != null)
        {
            Destroy(currentBall);
        }

        // Spawn nový ball
        Vector3 spawnPos = ballSpawnPoint != null
            ? ballSpawnPoint.position
            : transform.position;

        currentBall = Instantiate(ballPrefab, spawnPos, Quaternion.identity, transform);

        // Setup physics
        Rigidbody2D rb = currentBall.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // Reset velocity

            // Random sideways force (variety)
            float randomX = Random.Range(-initialRandomForce, initialRandomForce);
            rb.AddForce(new Vector2(randomX, 0));
        }

        // Auto cleanup
        Destroy(currentBall, destroyAfterSeconds);
    }
}
