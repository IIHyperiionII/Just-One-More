using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    void Update()
    {
        if (player == null)
        {
            Debug.LogError("Player transform is not assigned in CameraController.");
        } else {
            // Follow the player while maintaining the camera's z position
            Vector3 newPosition = player.position;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }
    }
}
