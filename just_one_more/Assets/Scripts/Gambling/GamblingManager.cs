using UnityEngine;
using System;

public class GamblingManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BallSpawner ballSpawner;

    private Action<float> onGameComplete;
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
            Debug.LogError("BallSpawner not assigned!");
        }
    }

    public void OnBallLanded(float multiplier)
    {
        if (gameActive && !resultSent)
        {
            resultSent = true;
            gameActive = false;

            Debug.Log($"Multiplier: {multiplier}x");

            onGameComplete?.Invoke(multiplier);
        }
    }

    public void ResetGame()
    {
        gameActive = false;
        resultSent = false;
    }
}
