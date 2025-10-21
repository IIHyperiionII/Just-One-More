using UnityEngine;

public class BallSpawner : MonoBehaviour 
{
    [Header("Ball Setup")]
    public GameObject ballPrefab;
    public Transform ballSpawnPoint;

    [Header("Physics Settings")]
    // Random sideways push
    public float initialRandomForce = 2f;

    private GameObject currentBall;

    public void DropBall()
    {
        if (currentBall != null)
            DestroyCurrentBall();
        
        // Spawn new ball
        Vector3 spawnPos = ballSpawnPoint != null
            ? ballSpawnPoint.position
            : transform.position;

        currentBall = Instantiate(ballPrefab, spawnPos, Quaternion.identity, transform);

        // Setup physics
        Rigidbody2D rb = currentBall.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Reset velocity
            rb.linearVelocity = Vector2.zero;

            // Random sideways force (variety)
            float randomX = Random.Range(-initialRandomForce, initialRandomForce);
            rb.AddForce(new Vector2(randomX, 0));
        }
    }

    public void DestroyCurrentBall()
    {
        if (currentBall != null)
        {
            Destroy(currentBall);
            currentBall = null;
        }
    }
}
