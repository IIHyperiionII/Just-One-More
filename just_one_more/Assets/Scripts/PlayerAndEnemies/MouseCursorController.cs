using UnityEngine;

public class MouseCursorController : MonoBehaviour
{
    Vector3 mousePosition;
    Vector3 playerPosition;
    private Transform player;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Need to be called after all player movement and rotation processes
    void LateUpdate()
    {
        transform.position = mousePosition;
        if (player == null)
        {
            Debug.LogError("Player does not exist in the scene.");
            Destroy(gameObject);
            return;
        }
        else
        {
            GetMousePosition();
            float angle = GetAngle();
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle)); // Rotate the cursor to face the direction from player to mouse
        }
    }
    void GetMousePosition()
    {
        float distanceZ = Mathf.Abs(Camera.main.transform.position.z); // Distance from camera to the XY plane (Z=0)
        mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distanceZ)); // Convert mouse position from screen space to world space
        mousePosition.z = 0f; // Set Z to 0 to be on the XY plane
    }
    float GetAngle()
    {
        playerPosition = player.position;
        playerPosition.z = 0f; // Ensure Z is 0 to be on the XY plane
        Vector3 direction = (mousePosition - playerPosition).normalized; // Get the normalized (value is 1, it does not affect speed) direction vector from player to mouse
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Transfering calculation of angle from radians to degrees
        return angle;
    }
}
