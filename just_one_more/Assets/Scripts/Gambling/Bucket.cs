using UnityEngine;

public class Bucket : MonoBehaviour
{
    [SerializeField] private float multiplier = 1.0f;

    public float getMultiplier()
    {
        return multiplier;
    }

    public void OnBallEntered()
    {
        // Send the bucket's multiplier to the GamblingManager when a ball enters
        GamblingManager gamblingManager = FindAnyObjectByType<GamblingManager>();
        if (gamblingManager != null)
        {
            gamblingManager.OnBallLanded(multiplier);
        }
        else
        {
            Debug.LogError("GamblingManager not found in scene!");
        }
    }
}
