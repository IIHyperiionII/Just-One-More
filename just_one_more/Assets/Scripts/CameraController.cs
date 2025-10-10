using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            Vector3 newPosition = player.position;
            newPosition.z = transform.position.z; // Keep the camera's current z position
            transform.position = newPosition;
        }
    }
}
