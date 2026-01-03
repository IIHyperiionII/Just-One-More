using UnityEngine;

// Spawns Plinko balls with random initial sideways force for variety
public class BallSpawner : MonoBehaviour 
{
    [Header("Ball Setup")]
    public GameObject ballPrefab;
    public Transform ballSpawnPoint;

    [Header("Physics Settings")]
    // Random sideways push
    public float initialRandomForce = 20f;

    private GameObject currentBall;

    // Spawn new ball at spawn point with random sideways push
    public void DropBall()
    {
        if (currentBall != null)
            DestroyCurrentBall();

        Vector3 spawnPos = ballSpawnPoint != null
            ? ballSpawnPoint.position
            : transform.position;

        currentBall = Instantiate(ballPrefab, spawnPos, Quaternion.identity);

        currentBall.transform.SetParent(transform);
        currentBall.transform.localScale = Vector3.one;

        // Setup physics
        Rigidbody2D rb = currentBall.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;

            // Random sideways force (variety)
            float randomX = Random.Range(-initialRandomForce, initialRandomForce);
            rb.AddForce(new Vector2(randomX, 0), ForceMode2D.Impulse);
        }
    }

    // Remove current ball from game
    public void DestroyCurrentBall()
    {
        if (currentBall != null)
        {
            Destroy(currentBall);
            currentBall = null;
        }
    }

    // Get reference to the currently active ball
    public GameObject GetCurrentBall()
    {
        return currentBall;
    }
}
