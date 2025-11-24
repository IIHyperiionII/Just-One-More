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
        // Send the bucket's multiplier to the plinkoManager when a ball enters
        PlinkoManager plinkoManager = FindAnyObjectByType<PlinkoManager>();
        if (plinkoManager != null)
        {
            plinkoManager.OnBallLanded(multiplier);
        }
        else
        {
            Debug.LogError("plinkoManager not found in scene!");
        }
    }
}
