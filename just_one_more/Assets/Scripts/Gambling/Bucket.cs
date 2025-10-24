using UnityEngine;
using UnityEngine.SceneManagement;

public class Bucket : MonoBehaviour
{
    [SerializeField] private float multiplier = 1.0f;

    public void Start()
    {
        this.transform.SetParent(null);
        SceneManager.MoveGameObjectToScene(this.gameObject, SceneManager.GetSceneByName("MiniGamePhysicsScene"));
        this.gameObject.transform.SetParent(GameObject.FindGameObjectWithTag("BucketParent").transform);
    }

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
