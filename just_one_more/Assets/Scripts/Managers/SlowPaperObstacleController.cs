using UnityEngine;

public class SlowPaperObstacleController : MonoBehaviour
{
    public CapsuleCollider2D slowPaperCollider;

    // Update is called once per frame
    void Update()
    {
        
    }
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
