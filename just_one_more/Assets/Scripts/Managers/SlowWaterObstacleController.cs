using UnityEngine;

public class SlowWaterObstacleController : MonoBehaviour
{
    public PolygonCollider2D slowWaterCollider;

    // Script to slow down the player when they enter the collider
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.parent == null) return;
        GameObject parent = other.transform.parent.gameObject;
        if (parent.CompareTag("Player"))
        {
            parent.GetComponent<PlayerController>().slowMultiplier = 0.5f;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.parent == null) return;
        GameObject parent = other.transform.parent.gameObject;
        if (parent.CompareTag("Player"))
        {
            parent.GetComponent<PlayerController>().slowMultiplier = 1f;
        }
    }
}
