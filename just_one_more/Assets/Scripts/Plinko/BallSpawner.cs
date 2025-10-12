using UnityEngine;

public class BallSpawner : MonoBehaviour 
{
    [Header("Ball Setup")]
    public GameObject ballPrefab;           // Drag Ball prefab sem
    public Transform ballSpawnPoint;        // Spawn pozice

    [Header("Physics Settings")]
    // public float spawnRangeX = 2f; // Random left/right pozice
    public float initialRandomForce = 2f;  // Random sideways push
    
    [Header("Auto Cleanup")]
    public float destroyAfterSeconds = 10f;

    private GameObject currentBall;
    void Update() 
    {
        // Test: Spacebar pro drop ball
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentBall == null && !GamblingManager.Instance.IsGameActive())
            {
                DropBall();
            }
        }
    }

    public void DropBall()
    {
        GamblingManager.Instance.StartGame();

        // Spawn nový ball
        Vector3 spawnPos = ballSpawnPoint != null
            ? ballSpawnPoint.position
            : transform.position;

        // spawnPos.x += Random.Range(-spawnRangeX, spawnRangeX);

        currentBall = Instantiate(ballPrefab, spawnPos, Quaternion.identity, transform);

        // Setup physics
        Rigidbody2D rb = currentBall.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // Reset velocity

            // Random sideways force (variety)
            float randomX = Random.Range(-initialRandomForce, initialRandomForce);
            rb.AddForce(new Vector2(randomX, 0), ForceMode2D.Impulse);
        }

        // Auto cleanup
        Destroy(currentBall, destroyAfterSeconds);
    }

    // zavolat z Ball.OnDestroy(), aby spawner věděl že už není aktivní míč
    public void NotifyBallDestroyed(GameObject ball)
    {
        if (currentBall == ball)
            currentBall = null;

        // také zajistit, že hra se resetne, pokud chcete povolit další spawny
        if (GamblingManager.Instance != null)
            GamblingManager.Instance.EndGame(0);
    }
}
