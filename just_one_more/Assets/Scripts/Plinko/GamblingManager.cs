using UnityEngine;

public class GamblingManager : MonoBehaviour
{
    public static GamblingManager Instance;

    private bool isGameActive = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsGameActive()
    {
        return isGameActive;
    }

    public void StartGame()
    {
        isGameActive = true;
    }

    public void HandleBallScore(Ball ball, Bucket bucket)
    {
        int score = bucket.getScoreValue();
        EndGame(score);
    }

    public void EndGame(int score)
    {
        isGameActive = false;
    }
}
