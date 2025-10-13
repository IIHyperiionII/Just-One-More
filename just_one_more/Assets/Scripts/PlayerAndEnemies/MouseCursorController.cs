using UnityEngine;

public class MouseCursorController : MonoBehaviour
{
    Vector3 mousePosition;
    Vector3 playerPosition;
    private Transform player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    
    void LateUpdate()
    {
        float distanceZ = Mathf.Abs(Camera.main.transform.position.z);
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceZ));
        mousePosition.z = 0f;
        transform.position = mousePosition;
        if (player != null) {
        playerPosition = player.position;
        playerPosition.z = 0f;
        Vector3 direction = (mousePosition - playerPosition).normalized;

        // Calculate angle (in degrees)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Apply rotation around Z-axis
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        } else {
            Destroy(gameObject);
        }
    }
}
