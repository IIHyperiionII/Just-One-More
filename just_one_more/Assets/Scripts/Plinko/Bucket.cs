using UnityEngine;

public class Bucket : MonoBehaviour
{
    [SerializeField] private int scoreValue = 10;

    public int getScoreValue()
    {
        return scoreValue;
    }

    public void OnBallEntered(Ball ball)
    {
        GamblingManager.Instance.HandleBallScore(ball, this);
    }
}
