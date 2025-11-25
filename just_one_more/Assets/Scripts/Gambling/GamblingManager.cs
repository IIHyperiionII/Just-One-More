using UnityEngine;
using System;

public class GamblingManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BallSpawner ballSpawner;

    private Action<float> onGameComplete;
    // gameActive prepared for future use (e.g. multiple balls)
    private bool gameActive = false;
    private bool resultSent = false;

    public void StartNewGame(Action<float> onComplete)
    {
        gameActive = true;
        resultSent = false;
        onGameComplete = onComplete;

        if (ballSpawner != null)
        {
            ballSpawner.DropBall();
        }
        else
        {
            Debug.LogWarning("BallSpawner not assigned!");
        }
    }

    public void OnBallLanded(float multiplier)
    {
        if (gameActive && !resultSent)
        {
            resultSent = true;
            gameActive = false;

            //onGameComplete != null => Invoke (call) onGameComplete with multiplier
            onGameComplete?.Invoke(multiplier);
        }
    }

    public void ResetGame() 
    {
        gameActive = false;
        resultSent = false;
        if (ballSpawner != null)
            ballSpawner.DestroyCurrentBall();
    }
}
