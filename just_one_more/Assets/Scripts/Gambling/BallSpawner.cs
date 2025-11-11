using UnityEngine;
using UnityEngine.SceneManagement;

public class BallSpawner : MonoBehaviour 
{
    [Header("Ball Setup")]
    public GameObject ballPrefab;
    public Transform ballSpawnPoint;

    [Header("Physics Settings")]
    // Random sideways push
    public float initialRandomForce = 20f;

    private GameObject currentBall;

    public void DropBall()
    {
        if (currentBall != null)
            DestroyCurrentBall();

        // Spawn new ball in ball spawn point or at position 
        // of the GO the BallSpawner is attached to
        Vector3 spawnPos = ballSpawnPoint != null
            ? ballSpawnPoint.position
            : transform.position;

        // Instantiate ball, no rotation, parented to spawner
        currentBall = Instantiate(ballPrefab, spawnPos, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(currentBall, SceneManager.GetSceneByName("MiniGamePhysicsScene"));
        currentBall.transform.SetParent(transform);
        currentBall.transform.localScale = transform.localScale;

        // Setup physics
        Rigidbody2D rb = currentBall.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Reset velocity
            rb.linearVelocity = Vector2.zero;

            // Random sideways force (variety)
            float randomX = Random.Range(-initialRandomForce, initialRandomForce);
            //rb.linearVelocity = new Vector2(randomX, 0);
            rb.AddForce(new Vector2(randomX, 0), ForceMode2D.Impulse);
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
